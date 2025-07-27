namespace BankaMVC.Models.DTOs
{
    public class LimitArtırmaDto
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }
        public string AdSoyad { get; set; }

        public string KartNo { get; set; }

        public decimal? MevcutLimit { get; set; }

        public decimal? TalepEdilenLimit { get; set; }

        public DateTime BasvuruTarihi { get; set; }

        public string Durum { get; set; }
    }
}
