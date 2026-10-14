$(document).ready(function (){
    let timeout;
    let isUserActive = true;

    function startTimeout() {
        // Clear any existing timeout
        clearTimeout(timeout);

        // Only set timeout if user is not active
        if (!isUserActive) {
            timeout = setTimeout(() => {
                $.ajax({
                    url: '/Account/Sign_Out/',
                    method: 'POST',
                    success: function (response) {
                        location.reload();
                    }
                });
            }, 3600000 ); // 60 minutes = 3600000 milliseconds
        }
    }

    function resetTimeout() {
        // Reset the timeout whenever there's user activity
        isUserActive = true;
        startTimeout();
    }

    // Event listeners for detecting user movement or activity
    window.addEventListener('mousemove', resetTimeout);
    window.addEventListener('keydown', resetTimeout);
    window.addEventListener('scroll', resetTimeout);
    window.addEventListener('click', resetTimeout);

    // Set user as inactive when they leave the page
    window.addEventListener('blur', function() {
        isUserActive = false;
        startTimeout();
    });

    // Set user as active when they return to the page
    window.addEventListener('focus', function() {
        isUserActive = true;
        clearTimeout(timeout);
    });

    // Start the timeout when the page loads
    startTimeout();
})