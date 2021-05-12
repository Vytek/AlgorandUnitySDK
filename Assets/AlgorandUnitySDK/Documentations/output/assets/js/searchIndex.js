
var camelCaseTokenizer = function (builder) {

  var pipelineFunction = function (token) {
    var previous = '';
    // split camelCaseString to on each word and combined words
    // e.g. camelCaseTokenizer -> ['camel', 'case', 'camelcase', 'tokenizer', 'camelcasetokenizer']
    var tokenStrings = token.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
      var current = cur.toLowerCase();
      if (acc.length === 0) {
        previous = current;
        return acc.concat(current);
      }
      previous = previous.concat(current);
      return acc.concat([current, previous]);
    }, []);

    // return token for each string
    // will copy any metadata on input token
    return tokenStrings.map(function(tokenString) {
      return token.clone(function(str) {
        return tokenString;
      })
    });
  }

  lunr.Pipeline.registerFunction(pipelineFunction, 'camelCaseTokenizer')

  builder.pipeline.before(lunr.stemmer, pipelineFunction)
}
var searchModule = function() {
    var documents = [];
    var idMap = [];
    function a(a,b) { 
        documents.push(a);
        idMap.push(b); 
    }

    a(
        {
            id:0,
            title:"GenerateWalletTest",
            content:"GenerateWalletTest",
            description:'',
            tags:''
        },
        {
            url:'/api/global/GenerateWalletTest',
            title:"GenerateWalletTest",
            description:""
        }
    );
    a(
        {
            id:1,
            title:"GetWalletAmount",
            content:"GetWalletAmount",
            description:'',
            tags:''
        },
        {
            url:'/api/global/GetWalletAmount',
            title:"GetWalletAmount",
            description:""
        }
    );
    a(
        {
            id:2,
            title:"StartGame",
            content:"StartGame",
            description:'',
            tags:''
        },
        {
            url:'/api/global/StartGame',
            title:"StartGame",
            description:""
        }
    );
    a(
        {
            id:3,
            title:"Singleton",
            content:"Singleton",
            description:'',
            tags:''
        },
        {
            url:'/api/global/Singleton_1',
            title:"Singleton<T>",
            description:""
        }
    );
    a(
        {
            id:4,
            title:"RSAEncryption",
            content:"RSAEncryption",
            description:'',
            tags:''
        },
        {
            url:'/api/UnityCipher/RSAEncryption',
            title:"RSAEncryption",
            description:""
        }
    );
    a(
        {
            id:5,
            title:"ConnectToNode",
            content:"ConnectToNode",
            description:'',
            tags:''
        },
        {
            url:'/api/global/ConnectToNode',
            title:"ConnectToNode",
            description:""
        }
    );
    a(
        {
            id:6,
            title:"CompleteScript",
            content:"CompleteScript",
            description:'',
            tags:''
        },
        {
            url:'/api/global/CompleteScript',
            title:"CompleteScript",
            description:""
        }
    );
    a(
        {
            id:7,
            title:"AlgorandManager",
            content:"AlgorandManager",
            description:'',
            tags:''
        },
        {
            url:'/api/global/AlgorandManager',
            title:"AlgorandManager",
            description:""
        }
    );
    a(
        {
            id:8,
            title:"RijndaelEncryption",
            content:"RijndaelEncryption",
            description:'',
            tags:''
        },
        {
            url:'/api/UnityCipher/RijndaelEncryption',
            title:"RijndaelEncryption",
            description:""
        }
    );
    a(
        {
            id:9,
            title:"BackgroundTasksProcessor",
            content:"BackgroundTasksProcessor",
            description:'',
            tags:''
        },
        {
            url:'/api/MHLab.Utilities/BackgroundTasksProcessor',
            title:"BackgroundTasksProcessor",
            description:""
        }
    );
    var idx = lunr(function() {
        this.field('title');
        this.field('content');
        this.field('description');
        this.field('tags');
        this.ref('id');
        this.use(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
        documents.forEach(function (doc) { this.add(doc) }, this)
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
