const form = document.querySelector(".register-form");
const successModal = document.querySelector(".modal-success");
const close = successModal.querySelector(".close-btn");

if (form) {
    form.addEventListener("submit", (event) => {
        const username = form.querySelector("#username");
        const email = form.querySelector("#email");
        const password = form.querySelector("#password");
        const rptPassword = form.querySelector("#repeatPassword");
        const selectInput = form.querySelector("#creativeType");
        const selected = form.querySelector(".selected-value");
        const checkbox = form.querySelector(".custom-checkbox");

        clearErrors(form);
        event.preventDefault();
        validateUsername(username);
        validateEmail(email);
        validatePassword(password, rptPassword);
        validateOption(selectInput);
        validateCheckBox(checkbox);

        if (!Array.from(form.querySelectorAll(".register-field")).find(field => field.classList.contains("error"))) {
            selectInput.value = selected.textContent.trim();
            const formData = new FormData();
            const token = form.querySelector('input[name="__RequestVerificationToken"]').value;
            formData.append("__RequestVerificationToken", token);
            formData.append("User.Username", username.value);
            formData.append("User.Email", email.value);
            formData.append("User.Password", password.value);
            formData.append("CreativeType", selectInput.value);

            fetch("/AccountActions/register?handler=Register", {
                method: "POST",
                body: formData
            })
            .then(async response => {
                const userData = await response.json();

                if (!response.ok) {
                    if (userData.Username != undefined) {
                        displayError(username, userData.Username);
                    }
                    if (userData.Email != undefined) {
                        displayError(email, userData.Email);
                    }
                }
                else {
                    successModal.append(userData.message);
                    successModal.classList.add("open");
                    document.body.classList.add("passive");
                    setTimeout(() => {
                        close.click();
                    }, 4000);
                }
            })
            .catch(error => {
                console.error("Error: ", error);
                alert("An error occurred, please try again later");
            });
        }
    });

    function displayError(field, message) {
        let errorMessage = document.createElement("div");
        errorMessage.classList.add("error-message");
        errorMessage.textContent = message;

        field.parentElement.classList.add("error");
        field.parentElement.append(errorMessage);
    }
    function clearErrors(form) {
        Array.from(form.querySelectorAll(".error-message")).forEach(error => {
            error.remove();
        });
        Array.from(form.querySelectorAll(".register-field")).forEach(error => {
            if (error.classList.contains("error")) {
                error.classList.remove("error");
            }
        })
    }

    function validateUsername(username) {
        if (username.value.trim() === "") {
            displayError(username, "Username cannot be empty!");
        } else if (username.value.includes(" ")) {
            displayError(username, "Username cannot contain any whitespace!");
        } else if (username.value.length < 5) {
            displayError(username, "Username must contain at least 5 characters!");
        }
    }

    function validateEmail(email) {
        if (email.value.trim() === "") {
            displayError(email, "Email cannot be empty!");
        } else if (!/^[^\s@]+@[^\s@]+\.[A-Za-z]{2,}$/.test(email.value)) {
            displayError(email, "Enter a valid email please!");
        }
    }

    function validatePassword(password, rptPassword) {
        if (password.value.length < 8) {
            displayError(password, "Password must contain at least 8 symbols!");
        } else if (!/^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]))/.test(password.value)) {
            displayError(password, "Password must contain at least uppercase and lowercase letters, digit, and special symbol!");
        } else if (password.value !== rptPassword.value) {
            displayError(rptPassword, "Passwords do not match!");
        }

    }

    function validateOption(selectInput) {
        if (selectInput.value.trim() === "") {
            displayError(selectInput, "Please, choose the option!");
        }
    }

    function validateCheckBox(checkbox) {
        if (!checkbox.classList.contains("checked")) {
            displayError(checkbox.parentElement, "Checkbox must be checked!");
        }
    }
}
close.addEventListener("click", () => {
    setTimeout(() => {
        window.location.href = "/profile/profile";
    }, 500);
});

