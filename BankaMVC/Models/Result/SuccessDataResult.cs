using Newtonsoft.Json;

namespace BankaMVC.Models.Result
{
    public class SuccessDataResult<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
