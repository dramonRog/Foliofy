import { clearErrors } from './validateLogin.js';

const loginModal = document.querySelector(".modal-login");
const close = loginModal.querySelector(".close-btn");
const loginLink = document.querySelector("#login-link");
const loginForm = loginModal.querySelector(".login-form");

loginLink.addEventListener("click", (event) => {
    event.preventDefault();
    loginModal.classList.add("open");
    document.body.classList.add("passive");
});

close.addEventListener("click", () => {
    loginModal.classList.remove("open");
    document.body.classList.remove("passive");
    clearErrors(loginForm);
});


