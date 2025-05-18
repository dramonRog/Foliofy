const registerLink = document.getElementById("register-link");
const modalRegister = document.querySelector(".modal-register");
const closeButton = modalRegister.querySelector(".close-btn");
const registerForm = modalRegister.querySelector(".register-form");

const loginModal = document.querySelector(".modal-login");
const close = loginModal.querySelector(".close-btn");
const loginLink = document.querySelector("#login-link");
const loginForm = loginModal.querySelector(".login-form");

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


