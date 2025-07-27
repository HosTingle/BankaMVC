using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BankaMVC.Models.Somut
{
    public class SubeDto  
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Şube adı boş olamaz.")]
        [StringLength(100, ErrorMessage = "Şube adı en fazla 100 karakter olabilir.")]
        public string SubeAdi { get; set; } = string.Empty;

    }
}
