// Toastr helper functions
function showSuccessToast(message) {
    toastr.success(message);
}

function showErrorToast(message) {
    toastr.error(message);
}

// Form validation helper
function validateForm(form) {
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;
    let errorMessages = [];
    
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            isValid = false;
            field.classList.add('border-red-500');
            const label = field.previousElementSibling?.textContent || 'Bu alan';
            errorMessages.push(`${label} zorunludur.`);
        } else {
            field.classList.remove('border-red-500');
            
            // Email validation
            if (field.type === 'email' && !isValidEmail(field.value)) {
                isValid = false;
                field.classList.add('border-red-500');
                errorMessages.push('Geçerli bir e-posta adresi giriniz.');
            }
            
            // Password validation
            if (field.type === 'password' && field.value.length < 6) {
                isValid = false;
                field.classList.add('border-red-500');
                errorMessages.push('Şifre en az 6 karakter olmalıdır.');
            }
            
            // Phone number validation
            if (field.type === 'tel' && !isValidPhoneNumber(field.value)) {
                isValid = false;
                field.classList.add('border-red-500');
                errorMessages.push('Geçerli bir telefon numarası giriniz.');
            }
            
            // TC Identity Number validation
            if (field.id === 'tc_no' && !isValidTCIdentityNumber(field.value)) {
                isValid = false;
                field.classList.add('border-red-500');
                errorMessages.push('Geçerli bir T.C. Kimlik Numarası giriniz.');
            }
        }
    });
    
    if (!isValid) {
        errorMessages.forEach(message => {
            showErrorToast(message);
        });
    }
    
    return isValid;
}

// Email validation
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Phone number validation
function isValidPhoneNumber(phone) {
    const phoneRegex = /^(\+90|0)?[0-9]{10}$/;
    return phoneRegex.test(phone.replace(/\D/g, ''));
}

// TC Identity Number validation
function isValidTCIdentityNumber(tcNo) {
    if (tcNo.length !== 11) return false;
    
    const digits = tcNo.split('').map(Number);
    if (digits.some(isNaN)) return false;
    
    const first10Sum = digits.slice(0, 10).reduce((sum, digit) => sum + digit, 0);
    const lastDigit = first10Sum % 10;
    
    return lastDigit === digits[10];
}

// Phone number formatter
function formatPhoneNumber(phoneNumber) {
    if (!phoneNumber) return phoneNumber;

    // Remove all non-digit characters
    phoneNumber = phoneNumber.replace(/\D/g, '');

    if (phoneNumber.length === 10) {
        return "+90" + phoneNumber;
    } else if (phoneNumber.length === 11 && phoneNumber.startsWith('0')) {
        return "+9" + phoneNumber;
    } else if (phoneNumber.length === 12 && phoneNumber.startsWith('90')) {
        return "+" + phoneNumber;
    } else if (!phoneNumber.startsWith('+')) {
        return "+" + phoneNumber;
    }

    return phoneNumber;
}

// Real-time validation
function setupRealTimeValidation() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, select, textarea');
        
        inputs.forEach(input => {
            // Input validation on blur
            input.addEventListener('blur', function() {
                if (this.hasAttribute('required') && !this.value.trim()) {
                    this.classList.add('border-red-500');
                    const label = this.previousElementSibling?.textContent || 'Bu alan';
                    showErrorToast(`${label} zorunludur.`);
                } else {
                    this.classList.remove('border-red-500');
                }
                
                // Email validation
                if (this.type === 'email' && this.value && !isValidEmail(this.value)) {
                    this.classList.add('border-red-500');
                    showErrorToast('Geçerli bir e-posta adresi giriniz.');
                }
                
                // Phone validation
                if (this.type === 'tel' && this.value && !isValidPhoneNumber(this.value)) {
                    this.classList.add('border-red-500');
                    showErrorToast('Geçerli bir telefon numarası giriniz.');
                }
                
                // TC Identity Number validation
                if (this.id === 'tc_no' && this.value && !isValidTCIdentityNumber(this.value)) {
                    this.classList.add('border-red-500');
                    showErrorToast('Geçerli bir T.C. Kimlik Numarası giriniz.');
                }
            });
            
            // Clear error on focus
            input.addEventListener('focus', function() {
                this.classList.remove('border-red-500');
            });
        });
    });
}

// Handle form submissions
document.addEventListener('DOMContentLoaded', function() {
    // Setup real-time validation
    setupRealTimeValidation();
    
    // Handle form submissions
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!validateForm(this)) {
                e.preventDefault();
                return;
            }

            // Format phone numbers before submission
            const phoneInputs = this.querySelectorAll('input[type="tel"]');
            phoneInputs.forEach(input => {
                if (input.value) {
                    input.value = formatPhoneNumber(input.value);
                }
            });
        });
    });

    // Handle server-side validation errors
    const validationSummary = document.querySelector('.validation-summary-errors');
    if (validationSummary) {
        const errors = validationSummary.querySelectorAll('li');
        errors.forEach(error => {
            showErrorToast(error.textContent);
        });
    }

    // Handle success messages
    const successMessage = document.querySelector('.alert-success');
    if (successMessage) {
        showSuccessToast(successMessage.textContent);
    }
    
    // Handle ViewBag messages
    if (typeof ViewBag !== 'undefined') {
        if (ViewBag.Success) {
            showSuccessToast('İşlem başarıyla tamamlandı.');
        }
        if (ViewBag.Error) {
            showErrorToast(ViewBag.Error);
        }
    }
}); 