
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class KartDetayViewModel
    {
        public Kart Kart { get; set; }
        public List<KartIslem> KartIslemleri { get; set; } = new List<KartIslem>();
    }
}
