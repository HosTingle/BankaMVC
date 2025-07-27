namespace BankaMVC.Models.Somut
{
    namespace BankaYonetimPaneli.Models
    {

        // Enum tanımlamaları
        public enum IstekDurumu
        {
            Beklemede,
            Onaylandi,
            Reddedildi
        }
         
        public enum DestekDurumu
        {
            Açık,  
            Islemde,
            Çözüldü,
            Kapatildi
        }

        public enum KartTipi
        {
            Kredi,
            Banka
        }
    }

}
