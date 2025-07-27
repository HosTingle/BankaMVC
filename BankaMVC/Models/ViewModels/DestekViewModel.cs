
using BankaMVC.Models.Somut;

namespace BankaMVC.Models.ViewModels
{
    public class DestekViewModel
    {
        public List<DestekTalebi> Talepler { get; set; } = new List<DestekTalebi>();
        public string DurumFiltre { get; set; } = "tum";
        public string AramaMetni { get; set; } = "";
        public DestekTalebi YeniTalep { get; set; } = new DestekTalebi();
    }
}
