/*
https://gist.github.com/HirbodBehnam/3bb3ae85e108153e7374b14d324cbaaf
*/
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

public class AESGCM
{
    /// <summary>
    /// Converts an <see cref="int"/> to byte array (big-endian)
    /// </summary>
    /// <param name="i">The number to convert</param>
    /// <returns>A array of 4 bytes</returns>
    public static byte[] IntToBytes(int i) => BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
    /// <summary>
    /// Converts a byte array of size 4 (big-endian) to <see cref="int"/>
    /// </summary>
    /// <param name="b">The byte array</param>
    /// <returns>The number</returns>
    public static int BytesToInt(byte[] b) => IPAddress.NetworkToHostOrder(BitConverter.ToInt32(b, 0));
    /// <summary>
    /// Generate a 96 bit nonce for Aes encryption
    /// </summary>
    /// <returns>nonce</returns>
    public static byte[] AesGenerateNonce()
    {
        byte[] nonce = new byte[12];
        new RNGCryptoServiceProvider().GetBytes(nonce);
        return nonce;
    }
    /// <summary>
    /// Decrypt a message with AES-GCM cipher; The nonce is first 12 bytes of payload
    /// </summary>
    /// <param name="payload">The message to decrypt</param>
    /// <param name="key">The key to decrypt it with it</param>
    /// <returns>Decrypted message</returns>
    public static byte[] AesGcmDecrypt(byte[] payload, byte[] key)
    {
        byte[] realPayload = new byte[payload.Length - 12],nonce = new byte[12];
        Buffer.BlockCopy(payload, 0, nonce, 0, 12); // get the first 12 bytes as nonce
        Buffer.BlockCopy(payload, 12, realPayload, 0, payload.Length - 12); // get the rest as the payload
        return AesGcmDecrypt(realPayload, key, nonce);
    }
    /// <summary>
    /// Decrypt a message with AES-GCM cipher
    /// </summary>
    /// <param name="payload">The message to decrypt</param>
    /// <param name="key">The key to decrypt it with it</param>
    /// <param name="nonce">The nonce (12 bytes)</param>
    /// <returns>Decrypted message</returns>
    public static byte[] AesGcmDecrypt(byte[] payload, byte[] key,byte[] nonce)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(false,new AeadParameters(new KeyParameter(key), 128, nonce));
        // https://github.com/SidShetye/BouncyBench/blob/master/BouncyBench/Ciphers/AesGcm.cs#L209
        var clearBytes = new byte[cipher.GetOutputSize(payload.Length)];
        int len = cipher.ProcessBytes(payload, 0, payload.Length, clearBytes, 0);
        cipher.DoFinal(clearBytes, len);
        return clearBytes;
    }
    /// <summary>
    /// Reads and decrypts a file THAT IS ENCRYPTED WITH <see cref="AesGcmEncrypt(FileInfo,FileInfo,byte[],int)"/>
    /// </summary>
    /// <param name="input">The input file to decrypt it</param>
    /// <param name="output">The output file to write the decrypted data into it</param>
    /// <param name="key">Encryption key</param>
    public static void AesGcmDecrypt(FileInfo input, FileInfo output, byte[] key)
    {
        using (Stream reader = input.OpenRead())
        {
            using (Stream writer = output.OpenWrite())
            {
                int bufferSize;
                // at first read the crypt buffer
                {
                    byte[] buffer = new byte[4];
                    reader.Read(buffer, 0, buffer.Length);
                    bufferSize = BytesToInt(buffer);
                    bufferSize += 12 + 16; // mac + nonce
                }
                // now read the input file; "bufferSize" bytes at a time
                int readCount;
                byte[] readBytes = new byte[bufferSize];
                while ((readCount = reader.Read(readBytes,0,readBytes.Length)) > 0)
                {
                    if(readBytes.Length > readCount)
                        Array.Resize(ref readBytes,readCount); // this is the last chunk of file; Do not encrypt all of the data in readBytes
                    byte[] decrypted = AesGcmDecrypt(readBytes, key);
                    writer.Write(decrypted,0,decrypted.Length);
                }
            }
        }
    }
    public static byte[] AesGcmEncrypt(byte[] payload, byte[] key)
    {
        return AesGcmEncrypt(payload, key, AesGenerateNonce());
    }
    /// <summary>
    /// Encrypt a byte array with AES-GCM; Nonce is created randomly
    /// </summary>
    /// <param name="payload">The array to encrypt</param>
    /// <param name="key">The key to encrypt it with</param>
    /// <param name="nonce">The nonce to encrypt it with (must be 12 bytes)</param>
    /// <returns>Encrypted bytes</returns>
    public static byte[] AesGcmEncrypt(byte[] payload, byte[] key, byte[] nonce)
    {
        var cipher = new GcmBlockCipher(new AesEngine());
        cipher.Init(true,new AeadParameters(new KeyParameter(key), 128, nonce));
        var cipherBytes = new byte[cipher.GetOutputSize(payload.Length)];
        int len = cipher.ProcessBytes(payload, 0, payload.Length, cipherBytes, 0);
        cipher.DoFinal(cipherBytes, len);
        return nonce.Concat(cipherBytes).ToArray();
    }
    /// <summary>
    /// Encrypts a file with AesGcm
    /// </summary>
    /// <param name="input">The input file to encrypt</param>
    /// <param name="output">Output file to write the encrypted file</param>
    /// <param name="key">The key to encrypt data with it</param>
    /// <remarks>
    /// This function breaks file into 1MB chunks, and encrypts each one separately
    /// After each full chunk the length of it becomes 1024 * 1024 + 28 (28 = 12 + 16) (nonce + hmac)
    /// This means that the file size increases about 0.002%
    /// Obviously the last block's size is not 1024 * 1024 + 28
    /// First 4 bytes of file is the buffer size
    /// </remarks>
    public static void AesGcmEncrypt(FileInfo input, FileInfo output, byte[] key)
    {
        AesGcmEncrypt(input, output, key, 1024 * 1024);
    }
    /// <summary>
    /// Encrypts a file with AesGcm
    /// </summary>
    /// <param name="input">The input file to encrypt</param>
    /// <param name="output">Output file to write the encrypted file</param>
    /// <param name="key">The key to encrypt data with it</param>
    /// <param name="bufferSize">The buffer size that the input is read and encrypted</param>
    public static void AesGcmEncrypt(FileInfo input, FileInfo output, byte[] key, int bufferSize)
    {
        using (Stream reader = input.OpenRead())
        {
            using (Stream writer = output.OpenWrite())
            {
                // at first write the buffer size to first of file
                writer.Write(IntToBytes(bufferSize),0,4); // int is 4 bytes
                // now read the input file; "bufferSize" bytes at a time
                int readCount;
                byte[] readBytes = new byte[bufferSize];
                while ((readCount = reader.Read(readBytes,0,readBytes.Length)) > 0)
                {
                    if(readBytes.Length > readCount)
                        Array.Resize(ref readBytes,readCount); // this is the last chunk of file; Do not encrypt all of the data in readBytes
                    byte[] crypted = AesGcmEncrypt(readBytes, key);
                    writer.Write(crypted,0,crypted.Length);
                }
            }
        }
    }
}
