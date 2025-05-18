const checkbox = document.querySelectorAll(".custom-checkbox");

checkbox.forEach(check => {
    check.addEventListener("click", () => {
        check.classList.toggle("checked");
    });
})
