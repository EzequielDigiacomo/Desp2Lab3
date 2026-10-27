document.addEventListener("DOMContentLoaded", function () {
    var toggle = document.getElementById("navToggle");
    var menu = document.getElementById("navMenu");

    if (toggle && menu) {
        toggle.addEventListener("click", function () {
            menu.classList.toggle("abierto");
        });
    }
});
