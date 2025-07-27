using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class KartlarViewModel
    {
        public List<Kart> Kartlar { get; set; } = new List<Kart>();
        public string AktifSekme { get; set; } = "tum";
    }
}
