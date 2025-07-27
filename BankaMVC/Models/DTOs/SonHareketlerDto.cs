namespace BankaMVC.Models.DTOs
{
    public class SonHareketlerDto
    {
        public decimal GuncelBakiye { get; set; }
        public decimal Tutar { get; set; }

        public string IslemTipi { get; set; }

        public DateTime Tarih { get; set; }

        public string? Aciklama { get; set; }

        public string Durum { get; set; }
    }
}
