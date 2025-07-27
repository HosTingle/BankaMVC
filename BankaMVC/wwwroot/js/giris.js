document.addEventListener('DOMContentLoaded', function() {
    // Telefon numarası formatlaması
    const telefonInput = document.getElementById('telefon-input');
    if (telefonInput) {
        telefonInput.addEventListener('input', function(e) {
            // Sadece rakamları al
            let value = e.target.value.replace(/\D/g, '');
            
            // 10 haneli sınırlama
            if (value.length > 10) {
                value = value.slice(0, 10);
            }
            
            // Formatla: 5XX XXX XX XX
            let formatted = '';
            if (value.length > 0) {
                formatted = value.slice(0, 3);
                if (value.length > 3) {
                    formatted += ' ' + value.slice(3, 6);
                }
                if (value.length > 6) {
                    formatted += ' ' + value.slice(6, 8);
                }
                if (value.length > 8) {
                    formatted += ' ' + value.slice(8, 10);
                }
            }
            
            e.target.value = formatted;
            
            // Hidden field'a sadece rakamları kaydet (form submit için)
            e.target.setAttribute('data-raw-value', value);
        });
        
        // Form submit edilirken sadece rakamları gönder
        telefonInput.closest('form').addEventListener('submit', function() {
            const rawValue = telefonInput.getAttribute('data-raw-value') || telefonInput.value.replace(/\D/g, '');
            telefonInput.value = rawValue;
        });
    }
    
    // Şifre göster/gizle
    const passwordToggle = document.getElementById('password-toggle');
    const passwordInput = document.getElementById('sifre-input');
    const eyeOpen = document.getElementById('eye-open');
    const eyeClosed = document.getElementById('eye-closed');
    
    if (passwordToggle && passwordInput) {
        passwordToggle.addEventListener('click', function() {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                eyeOpen.style.display = 'none';
                eyeClosed.style.display = 'block';
            } else {
                passwordInput.type = 'password';
                eyeOpen.style.display = 'block';
                eyeClosed.style.display = 'none';
            }
        });
    }
    
    // Form validasyonu
});