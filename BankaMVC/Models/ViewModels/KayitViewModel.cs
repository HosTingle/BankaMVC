using BankaMVC.Models.Somut;
using System.ComponentModel.DataAnnotations;

namespace BankaMVC.Models.ViewModels
{
    public class KayitViewModel
    {
        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        [StringLength(10, ErrorMessage = "Telefon 10 haneli olmalıdır.")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Şube kodu zorunludur.")]

        public int SecilenSubeId { get; set; }
        public List<SubeDto>? Subeler { get; set; } 
    }

}
