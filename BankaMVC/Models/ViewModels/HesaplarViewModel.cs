
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class HesaplarViewModel
    {
        
        public List<Hesap> Hesaplar { get; set; } = new List<Hesap>();
        public string AktifSekme { get; set; } = "tum";
    }
}
