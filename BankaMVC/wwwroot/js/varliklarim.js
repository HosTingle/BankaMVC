const urlParams = new URLSearchParams(window.location.search);
let currentCurrency = urlParams.get("paraBirimi") || "TRY";
let currentSection = urlParams.get("bolum") || "varliklar";
// Para birimi sembolleri
const currencySymbols = {
    TRY: "₺",
    USD: "$",
    EUR: "€",
}

document.addEventListener("DOMContentLoaded", () => {
    currentCurrency = document.getElementById("paraBirimiSelect").value

    // URL'den aktif bölümü al
    const urlParams = new URLSearchParams(window.location.search)
    const bolum = urlParams.get("bolum")
    if (bolum) {
        currentSection = bolum
    }
})


function updateCurrency() {
    const select = document.getElementById("paraBirimiSelect");
    currentCurrency = select.value;

    // Sayfayı yeni para birimiyle yeniden yükle
    window.location.href = `/varliklarim?paraBirimi=${currentCurrency}&bolum=${currentSection}`;
}

function selectSection(section) {
    currentSection = section;

    // Buton durumlarını güncelle
    document.querySelectorAll(".bolum-btn").forEach((btn) => {
        btn.classList.remove("active");
    });
    event.target.closest(".bolum-btn").classList.add("active");

    // Sayfayı yeni bölümle ve mevcut para birimiyle yeniden yükle
    window.location.href = `/varliklarim?paraBirimi=${currentCurrency}&bolum=${currentSection}`;
}