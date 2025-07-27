using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class HesapTalebi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tip boş olamaz.")]
        [StringLength(50, ErrorMessage = "Tip en fazla 50 karakter olabilir.")]
        public string Tip { get; set; } = string.Empty;

        [Required(ErrorMessage = "Müşteri adı boş olamaz.")]
        [StringLength(100, ErrorMessage = "Müşteri adı en fazla 100 karakter olabilir.")]
        public string Musteri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public HesapTalepDurumu Durum { get; set; }

        [Required(ErrorMessage = "TC Kimlik boş olamaz.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik 11 karakter olmalıdır.")]
        public string TcKimlik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon boş olamaz.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Telefon numarası 10 ile 15 karakter arasında olmalıdır.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email boş olamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [StringLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres boş olamaz.")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string Adres { get; set; } = string.Empty;

        public string? RedNedeni { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }

    public enum HesapTalepDurumu
    {
        Bekliyor = 0,
        Inceleniyor = 1,
        Onaylandi = 2,
        Reddedildi = 3
    }
}
