import { clearErrors } from './validateRegister.js';

const registerLink = document.getElementById("register-link");
const modalRegister = document.querySelector(".modal-register");
const closeButton = modalRegister.querySelector(".close-btn");
const registerForm = modalRegister.querySelector(".register-form");

if (registerLink) {
    registerLink.addEventListener("click", (event) => {
        event.preventDefault();
        modalRegister.classList.add("open");
        document.body.classList.add("passive");
    });

    closeButton.addEventListener("click", () => {
        modalRegister.classList.remove("open");
        document.body.classList.remove("passive");
        clearErrors(registerForm);
    });
}
