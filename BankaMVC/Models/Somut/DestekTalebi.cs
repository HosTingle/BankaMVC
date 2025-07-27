using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.Somut
{
    public class DestekTalebi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Id zorunludur.")]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Konu boş olamaz.")]
        [StringLength(150, ErrorMessage = "Konu en fazla 150 karakter olabilir.")]
        public string Konu { get; set; }

        [Required(ErrorMessage = "Mesaj boş olamaz.")]
        [StringLength(1000, ErrorMessage = "Mesaj en fazla 1000 karakter olabilir.")]
        public string Mesaj { get; set; }

        [Required(ErrorMessage = "Durum boş olamaz.")]
        [StringLength(50, ErrorMessage = "Durum en fazla 50 karakter olabilir.")]
        public string Durum { get; set; }

        [Required(ErrorMessage = "Kategori boş olamaz.")]
        [StringLength(100, ErrorMessage = "Kategori en fazla 100 karakter olabilir.")]
        public string Kategori { get; set; }

        public string? Yanit { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }

}
