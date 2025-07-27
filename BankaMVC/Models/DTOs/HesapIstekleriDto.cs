namespace BankaMVC.Models.DTOs
{
    public class HesapIstekleriDto
    {
        public int Id { get; set; }

        public string AdSoyad { get; set; }

        public string HesapNo { get; set; }

        public DateTime BasvuruTarihi { get; set; }

        public string Durum { get; set; }

        public string Telefon { get; set; }

        public string Eposta { get; set; } 
    }
}
