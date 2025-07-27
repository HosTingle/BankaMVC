using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class KartTalebi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kart tipi zorunludur.")]
        [StringLength(50, ErrorMessage = "Kart tipi en fazla 50 karakter olabilir.")]
        public string Tip { get; set; }

        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
        public string Musteri { get; set; }

        [Required(ErrorMessage = "Hesap numarası zorunludur.")]
        [StringLength(50, ErrorMessage = "Hesap numarası en fazla 50 karakter olabilir.")]
        public string HesapNo { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public KartTalepDurumu Durum { get; set; }

        [Required(ErrorMessage = "Gelir bilgisi zorunludur.")]
        [StringLength(50, ErrorMessage = "Gelir bilgisi en fazla 50 karakter olabilir.")]
        public string Gelir { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Limit negatif olamaz.")]
        public decimal Limit { get; set; }

        [StringLength(200, ErrorMessage = "Red nedeni en fazla 200 karakter olabilir.")]
        public string? RedNedeni { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }

    public enum KartTalepDurumu
    {
        Bekliyor = 0,
        Inceleniyor = 1,
        Onaylandi = 2,
        Reddedildi = 3
    }
}
