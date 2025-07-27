using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class LimitTalebi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Müşteri adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
        public string Musteri { get; set; }

        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [StringLength(20, ErrorMessage = "Kart numarası en fazla 20 karakter olabilir.")]
        public string KartNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Mevcut limit negatif olamaz.")]
        public decimal MevcutLimit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Talep edilen limit negatif olamaz.")]
        public decimal TalepEdilenLimit { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public LimitTalepDurumu Durum { get; set; }

        [Required(ErrorMessage = "Gelir alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Gelir alanı en fazla 50 karakter olabilir.")]
        public string Gelir { get; set; }

        [Range(0, 100, ErrorMessage = "Kullanım oranı 0 ile 100 arasında olmalıdır.")]
        public int KullanimOrani { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Onaylanan limit negatif olamaz.")]
        public decimal? OnaylananLimit { get; set; }

        [StringLength(200, ErrorMessage = "Red nedeni en fazla 200 karakter olabilir.")]
        public string? RedNedeni { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }

    public enum LimitTalepDurumu
    {
        Bekliyor = 0,
        Inceleniyor = 1,
        Onaylandi = 2,
        Reddedildi = 3
    }
}
