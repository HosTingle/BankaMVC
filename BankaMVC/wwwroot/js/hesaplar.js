document.addEventListener("DOMContentLoaded", () => {
    // Hesap kartlarına tıklama olayı
    const hesapKartlari = document.querySelectorAll(".hesap-card")

    hesapKartlari.forEach((kart) => {
        kart.addEventListener("click", function () {
            // Hesap ID'sini al (data attribute olarak eklenebilir)
            const hesapId = this.getAttribute("data-hesap-id")

            // Hesap detay sayfasına yönlendirme (ileride eklenebilir)
            // window.location.href = `/Hesap/Detay/${hesapId}`;

            // Şimdilik konsola yazdıralım
            console.log(`Hesap ID: ${hesapId} tıklandı`)
        })
    })

    // Hesap numaralarını formatla (4'lü gruplar halinde)
    const hesapNolari = document.querySelectorAll(".hesap-no")

    hesapNolari.forEach((hesapNo) => {
        const numara = hesapNo.textContent.trim()
        const formatliNumara = numara.replace(/(\d{4})(?=\d)/g, "$1 ")
        hesapNo.textContent = formatliNumara
    })
})
