// Start of Passwoerd Visible/Invisible on Login Page
const togglePassword = document.querySelector('#togglePassword');
const password = document.querySelector('#Password');
togglePassword.addEventListener('click', function (e) {
    // toggle the type attribute
    const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
    password.setAttribute('type', type);
    // toggle the eye slash icon
    this.classList.toggle('fa-eye-slash');
});
// End of Passwoerd Visible/Invisible on Login Page


// Start to Denie Copy and Paste password on Login Page
let input = document.querySelector('#Password');
input.addEventListener('copy', (e) => {
    e.preventDefault(); // This is what prevents pasting.
    $('#errMsg').show();
    $('#errMsg').text("Password can not be Copy.");
});
password.addEventListener('click', function (e) {
    $('#errMsg').hide();
});
// End to Denie Copy and Paste password on Login Page