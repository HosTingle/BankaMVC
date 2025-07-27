namespace BankaMVC.Models.DTOs
{
    public class KartIstekleriDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }

        public string KartTipi { get; set; }

        public decimal? Limit { get; set; }

        public DateTime Tarih { get; set; }

        public string Durum { get; set; }

    }
}
