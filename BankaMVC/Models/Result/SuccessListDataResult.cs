using Newtonsoft.Json;

namespace BankaMVC.Models.Result
{
    public class SuccessListDataResult<T> 
    {
        [JsonProperty("data")]
        public List<T> Data { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
