const removeProjectButton = document.querySelector(".remove-project");
const modal = document.querySelector(".modal-warning");

removeProjectButton.addEventListener("click", () => {
    modal.classList.add("open");
    document.body.classList.add("passive");
});

