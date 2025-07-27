document.addEventListener("DOMContentLoaded", () => {
    // Kart numaralarını formatla
    const kartNumaralari = document.querySelectorAll(".kart-numara")

    kartNumaralari.forEach((kartNumara) => {
        const numara = kartNumara.textContent.trim()
        const formatliNumara = numara.replace(/(\d{4})(?=\d)/g, "$1 ")
        kartNumara.textContent = formatliNumara
    })

    // Kart kartlarına tıklama olayı
    const kartKartlari = document.querySelectorAll(".kart-card")

    kartKartlari.forEach((kart) => {
        kart.addEventListener("click", function () {
            // Kart ID'sini al
            const kartId = this.getAttribute("data-kart-id")

            // Kart detay sayfasına yönlendirme (ileride eklenebilir)
            console.log(`Kart ID: ${kartId} tıklandı`)
        })
    })

    // Hover efektleri
    kartKartlari.forEach((kart) => {
        kart.addEventListener("mouseenter", function () {
            this.style.transform = "translateY(-8px)"
        })

        kart.addEventListener("mouseleave", function () {
            this.style.transform = "translateY(0)"
        })
    })
})
document.addEventListener("DOMContentLoaded", function () {
    const dialog = document.getElementById("limitDialog");
    const kartIdInput = document.getElementById("dialogKartId");
    const mevcutLimitText = document.getElementById("dialogMevcutLimit");
    const mevcutLimitHiddenInput = document.getElementById("hiddenMevcutLimit"); // ✅ Burası eklendi

    // Butonlara tıklayınca dialog'u aç
    document.querySelectorAll(".limit-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            const kartId = btn.getAttribute("data-kart-id");
            const mevcutLimit = parseFloat(btn.getAttribute("data-mevcut-limit")) || 0;

            kartIdInput.value = kartId;
            mevcutLimitText.textContent = mevcutLimit.toLocaleString("tr-TR", {
                style: "currency",
                currency: "TRY"
            });
            mevcutLimitHiddenInput.value = mevcutLimit; // ✅ Forma veri ekleniyor

            dialog.showModal();
        });
    });

    // Kapatma butonları
    document.getElementById("closeDialogBtn").addEventListener("click", () => dialog.close());
    document.getElementById("closeDialogBtnFooter").addEventListener("click", () => dialog.close());
});



