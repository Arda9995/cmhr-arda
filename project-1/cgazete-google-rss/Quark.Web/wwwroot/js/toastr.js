class Toastr {
    constructor() {
        this.container = document.createElement('div');
        this.container.className = 'fixed top-4 left-1/2 transform -translate-x-1/2 z-50 space-y-4';
        document.body.appendChild(this.container);
    }

    show(message, type = 'success', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = `flex items-center p-4 w-full max-w-md text-gray-500 bg-white rounded-lg shadow-lg border-l-4 ${
            type === 'success' ? 'border-green-500' : 'border-red-500'
        } transform transition-all duration-300 translate-y-[-100%] opacity-0`;

        const icon = document.createElement('div');
        icon.className = `inline-flex items-center justify-center flex-shrink-0 w-6 h-6 ${
            type === 'success' ? 'text-green-500 bg-green-100' : 'text-red-500 bg-red-100'
        } rounded-lg`;

        const iconSvg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        iconSvg.setAttribute('class', 'w-4 h-4');
        iconSvg.setAttribute('fill', 'currentColor');
        iconSvg.setAttribute('viewBox', '0 0 20 20');
        
        const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute('fill-rule', 'evenodd');
        path.setAttribute('d', type === 'success' 
            ? 'M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z'
            : 'M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z');
        path.setAttribute('clip-rule', 'evenodd');
        iconSvg.appendChild(path);
        icon.appendChild(iconSvg);

        const content = document.createElement('div');
        content.className = 'ml-3 text-sm font-normal';
        content.textContent = message;

        const closeButton = document.createElement('button');
        closeButton.className = 'ml-auto -mx-1.5 -my-1.5 bg-white text-gray-400 hover:text-gray-900 rounded-lg focus:ring-2 focus:ring-gray-300 p-1 hover:bg-gray-100 inline-flex h-6 w-6';
        closeButton.setAttribute('type', 'button');
        closeButton.innerHTML = '<span class="sr-only">Kapat</span><svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path></svg>';

        toast.appendChild(icon);
        toast.appendChild(content);
        toast.appendChild(closeButton);
        this.container.appendChild(toast);

        // Trigger reflow
        toast.offsetHeight;

        // Add show class
        toast.classList.remove('translate-y-[-100%]', 'opacity-0');
        toast.classList.add('translate-y-0', 'opacity-100');

        // Auto remove after duration
        setTimeout(() => {
            this.hide(toast);
        }, duration);

        // Close button click handler
        closeButton.addEventListener('click', () => {
            this.hide(toast);
        });
    }

    hide(toast) {
        toast.classList.remove('translate-y-0', 'opacity-100');
        toast.classList.add('translate-y-[-100%]', 'opacity-0');
        
        setTimeout(() => {
            toast.remove();
        }, 300);
    }

    success(message, duration) {
        this.show(message, 'success', duration);
    }

    error(message, duration) {
        this.show(message, 'error', duration);
    }
}

// Create global toastr instance
window.toastr = new Toastr(); 