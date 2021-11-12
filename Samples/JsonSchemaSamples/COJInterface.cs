using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public interface ICojHeader
    {
        string objectId { get; set; }
        string objectType { get; set; }
        string docNo { get; set; }
        string createdBy { get; set; }
        string createdAt { get; set; }
        string updatedBy { get; set; }
        string updatedAt { get; set; }
        int revision { get; set; }
        bool isRelesed { get; set; }
        bool isDeleted { get; set; }
        string division { get; set; }
        string site { get; set; }
        string procno { get; set; }
        string plantcd { get; set; }
        string macno { get; set; }
        string typecd { get; set; }
        string lotno { get; set; }
        string magno { get; set; }
        string description { get; set; }
    }

    public interface IObjHeader
    {
        bool hasValues { get; set; }
        bool isClosed { get; set; }
    }

    public interface IMmStatus
    {
        string inpStatus { get; set; }
        bool hasValues { get; set; }
        bool isClosed { get; set; }
    }

    public interface IInStatus
    {
        string inpStatus { get; set; }
        bool hasValues { get; set; }
        bool isClosed { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial interface IFd01_csv
    {

        [Newtonsoft.Json.JsonProperty("filepath", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        string Filepath { get; set; }

        [Newtonsoft.Json.JsonProperty("header", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        int Header { get; set; }

        [Newtonsoft.Json.JsonProperty("encoding", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        string Encoding { get; set; }

    }


    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial interface IFd02_csv
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("colno", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        int Colno { get; set; }

        [Newtonsoft.Json.JsonProperty("unit", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        string Unit { get; set; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        string Type { get; set; }

    }


}
