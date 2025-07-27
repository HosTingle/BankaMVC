namespace BankaMVC.Models.Result
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
