using Newtonsoft.Json;

namespace Algo.Runtime.Build.Parser
{
    public class CodeDocument
    {
        #region Properties

        [JsonProperty]
        public string DocumentName { get; set; }

        [JsonProperty]
        public string Code { get; set; }

        #endregion

        #region Constructors

        public CodeDocument(string code, string documentName = "{unnamed}")
        {
            Code = code;
            DocumentName = documentName;
        }

        #endregion
    }
}
