document.addEventListener("DOMContentLoaded", () => {
    let aktifHesapIndex = 0
    let aktifKartIndex = 0

    const hesapContainer = document.getElementById("hesap-container")
    const kartContainer = document.getElementById("kart-container")
    const hesapCounter = document.getElementById("hesap-counter")
    const kartCounter = document.getElementById("kart-counter")

    const hesapSayisi = hesapContainer.children.length
    const kartSayisi = kartContainer.children.length

    // Menü işlemleri
    const menuBtn = document.getElementById("menu-btn")
    const closeMenuBtn = document.getElementById("close-menu")
    const sideMenu = document.getElementById("side-menu")
    const menuOverlay = document.getElementById("menu-overlay")

    menuBtn.addEventListener("click", () => {
        sideMenu.classList.add("active")
        menuOverlay.classList.add("active")
    })

    function closeMenu() {
        sideMenu.classList.remove("active")
        menuOverlay.classList.remove("active")
    }

    closeMenuBtn.addEventListener("click", closeMenu)
    menuOverlay.addEventListener("click", closeMenu)

  


    // Hesap kaydırma
    function updateHesapPosition() {
        const translateX = -aktifHesapIndex * 296 // 280px + 16px margin
        hesapContainer.style.transform = `translateX(${translateX}px)`
        hesapCounter.textContent = `${aktifHesapIndex + 1} / ${hesapSayisi}`

        // Dots güncelle
        document.querySelectorAll('[data-type="hesap"]').forEach((dot, index) => {
            dot.classList.toggle("active", index === aktifHesapIndex)
        })
    }

    // Kart kaydırma
    function updateKartPosition() {
        const translateX = -aktifKartIndex * 296 // 280px + 16px margin
        kartContainer.style.transform = `translateX(${translateX}px)`
        kartCounter.textContent = `${aktifKartIndex + 1} / ${kartSayisi}`

        // Dots güncelle
        document.querySelectorAll('[data-type="kart"]').forEach((dot, index) => {
            dot.classList.toggle("active", index === aktifKartIndex)
        })
    }

    // Dot tıklama olayları
    document.querySelectorAll(".dot").forEach((dot) => {
        dot.addEventListener("click", function () {
            const index = Number.parseInt(this.dataset.index)
            const type = this.dataset.type

            if (type === "hesap") {
                aktifHesapIndex = index
                updateHesapPosition()
            } else if (type === "kart") {
                aktifKartIndex = index
                updateKartPosition()
            }
        })
    })

    // Touch olayları
    let startX = 0
    let currentX = 0
    let isDragging = false

    function handleTouchStart(e) {
        startX = e.touches[0].clientX
        isDragging = true
    }

    function handleTouchMove(e) {
        if (!isDragging) return
        currentX = e.touches[0].clientX
    }

    function handleTouchEnd(e) {
        if (!isDragging) return
        isDragging = false

        const diffX = startX - currentX
        const threshold = 50

        if (Math.abs(diffX) > threshold) {
            const target = e.target
            const hesapSection = target.closest(".accounts-section")
            const kartSection = target.closest(".cards-section")

            if (hesapSection) {
                if (diffX > 0 && aktifHesapIndex < hesapSayisi - 1) {
                    aktifHesapIndex++
                    updateHesapPosition()
                } else if (diffX < 0 && aktifHesapIndex > 0) {
                    aktifHesapIndex--
                    updateHesapPosition()
                }
            } else if (kartSection) {
                if (diffX > 0 && aktifKartIndex < kartSayisi - 1) {
                    aktifKartIndex++
                    updateKartPosition()
                } else if (diffX < 0 && aktifKartIndex > 0) {
                    aktifKartIndex--
                    updateKartPosition()
                }
            }
        }
    }


    updateHesapPosition()
    updateKartPosition()
})
