using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BankaMVC.Models.Somut
{
    public class Kart 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi zorunludur.")]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Kart numarası boş olamaz.")]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "Kart numarası 13 ile 16 karakter arasında olmalıdır.")]
        public string KartNumarasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart tipi boş olamaz.")]
        [StringLength(20, ErrorMessage = "Kart tipi en fazla 20 karakter olabilir.")]
        public string KartTipi { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV boş olamaz.")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV 3 veya 4 karakter olmalıdır.")]
        public string CVV { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Limit negatif olamaz.")]
        public decimal? Limit { get; set; }

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        public DateTime SonKullanma { get; set; }

        [StringLength(30, ErrorMessage = "Durum en fazla 30 karakter olabilir.")]
        public string? Durum { get; set; }

        public bool Aktif { get; set; } = true;

    }

}
