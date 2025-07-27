document.addEventListener('DOMContentLoaded', function () {
    const odemeHesap = document.getElementById('odeme-hesap');
    const odemeKart = document.getElementById('odeme-kart');
    const hesapListesi = document.getElementById('hesap-listesi');
    const kartListesi = document.getElementById('kart-listesi');
    const secimBaslik = document.getElementById('secim-baslik');

    // Ödeme aracı değişikliği
    function updatePaymentMethod() {
        if (odemeHesap.checked) {
            hesapListesi.style.display = 'flex';
            kartListesi.style.display = 'none';
            secimBaslik.textContent = 'Hesap Seçimi';

            // Kart seçimlerini temizle
            document.querySelectorAll('.card-radio').forEach(radio => {
                radio.checked = false;
            });
        } else if (odemeKart.checked) {
            hesapListesi.style.display = 'none';
            kartListesi.style.display = 'flex';
            secimBaslik.textContent = 'Kredi Kartı Seçimi';

            // Hesap seçimlerini temizle
            document.querySelectorAll('.account-radio').forEach(radio => {
                radio.checked = false;
            });
        }
    }

    // Event listener'ları ekle
    odemeHesap.addEventListener('change', updatePaymentMethod);
    odemeKart.addEventListener('change', updatePaymentMethod);

    // Sayfa yüklendiğinde ilk durumu ayarla
    updatePaymentMethod();

    // Form validasyonu
    const form = document.querySelector('.transfer-form');
    if (form) {
        form.addEventListener('submit', function (e) {
            const odemeAraci = document.querySelector('input[name="OdemeAraci"]:checked');
            const tutar = document.querySelector('input[name="Tutar"]').value;
            const aliciHesapNo = document.querySelector('input[name="AliciHesapNo"]').value;

            // Ödeme aracı seçim kontrolü
            if (odemeAraci) {
                if (odemeAraci.value === 'hesap') {
                    const secilenHesap = document.querySelector('input[name="SecilenHesapId"]:checked');
                    if (!secilenHesap) {
                        e.preventDefault();
                        alert('Lütfen bir hesap seçiniz');
                        return false;
                    }
                } else if (odemeAraci.value === 'kart') {
                    const secilenKart = document.querySelector('input[name="SecilenKartId"]:checked');
                    if (!secilenKart) {
                        e.preventDefault();
                        alert('Lütfen bir kredi kartı seçiniz');
                        return false;
                    }
                }
            }

            // Tutar kontrolü
            if (parseFloat(tutar) < 1) {
                e.preventDefault();
                alert('Transfer tutarı en az 1 TL olmalıdır');
                return false;
            }

            // Hesap numarası kontrolü
            if (aliciHesapNo.length < 5) {
                e.preventDefault();
                alert('Lütfen geçerli bir hesap numarası giriniz');
                return false;
            }

            // Onay mesajı
            const onay = confirm(`${tutar} TL tutarında transfer işlemini onaylıyor musunuz?`);
            if (!onay) {
                e.preventDefault();
                return false;
            }
        });
    }

    // Tutar formatlaması
    const tutarInput = document.querySelector('input[name="Tutar"]');
    if (tutarInput) {
        tutarInput.addEventListener('input', function (e) {
            let value = e.target.value;
            // Sadece rakam ve nokta kabul et
            value = value.replace(/[^0-9.]/g, '');
            // Birden fazla nokta varsa sadece ilkini bırak
            const parts = value.split('.');
            if (parts.length > 2) {
                value = parts[0] + '.' + parts.slice(1).join('');
            }
            e.target.value = value;
        });
    }

    // Hesap numarası formatlaması
    const hesapNoInput = document.querySelector('input[name="AliciHesapNo"]');
    if (hesapNoInput) {
        hesapNoInput.addEventListener('input', function (e) {
            // Sadece rakam kabul et
            e.target.value = e.target.value.replace(/\D/g, '');
        });
    }
});