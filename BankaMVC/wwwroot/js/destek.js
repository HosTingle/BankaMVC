document.addEventListener("DOMContentLoaded", () => {
    // Talep kartlarına tıklama olayı
    const talepKartlari = document.querySelectorAll(".talep-card")

    talepKartlari.forEach((kart) => {
        kart.addEventListener("click", function () {
            const talepId = this.getAttribute("data-talep-id")
            // Talep detay sayfasına yönlendirme (ileride eklenebilir)
            console.log(`Talep ID: ${talepId} tıklandı`)
        })
    })

    // Form validasyonu
    const talepForm = document.querySelector(".talep-form")
    if (talepForm) {
        talepForm.addEventListener("submit", function (e) {
            const kategori = this.querySelector('[name="Kategori"]').value
            const konu = this.querySelector('[name="Konu"]').value
            const mesaj = this.querySelector('[name="Mesaj"]').value

            if (!kategori || !konu || !mesaj) {
                e.preventDefault()
                alert("Lütfen tüm alanları doldurun.")
                return false
            }

            if (mesaj.length < 10) {
                e.preventDefault()
                alert("Mesaj en az 10 karakter olmalıdır.")
                return false
            }
        })
    }

    // Arama formu otomatik gönderme
    const aramaInput = document.querySelector('input[name="arama"]')
    if (aramaInput) {
        let timeout
        aramaInput.addEventListener("input", function () {
            clearTimeout(timeout)
            timeout = setTimeout(() => {
                this.form.submit()
            }, 500)
        })
    }

})


document.head.appendChild(style)
