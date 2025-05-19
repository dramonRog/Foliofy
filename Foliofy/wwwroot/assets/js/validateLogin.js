const form = document.querySelector(".login-form");
const modalSuccess = document.querySelector(".modal-success");

form.addEventListener("submit", (event) => {
    const username = form.querySelector("#username");
    const password = form.querySelector("#password");
    const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

    clearErrors(form);
    event.preventDefault();

    if (username.value.trim() === "") {
        displayError(username, "This field cannot be empty!");
    }
    if (password.value.trim() === "") {
        displayError(password, "This field cannot be empty!");
    }

    if (!Array.from(document.querySelectorAll(".login-field")).find(field => field.classList.contains("error"))) {
        const formData = new FormData();
        formData.append("__RequestVerificationToken", token);
        formData.append("User.Username", username.value);
        formData.append("User.Password", password.value);

        fetch("/AccountActions/account?handler=Login", {
            method: "POST",
            body: formData
        })
        .then(async response => {
            const userData = await response.json();

            if (!response.ok) {
                displayError(username, userData.Error);
            }
            else {
                modalSuccess.append(userData.message);
                modalSuccess.classList.add("open");
                document.body.classList.add("passive");
                setTimeout(() => {
                    document.location.reload();
                }, 4000);
            }
        })
        .catch(error => {
            alert("Error occured. Please, try again later");
            console.error("Error", error);
        })
    }
});

function displayError(field, message) {
    let errorMessage = document.createElement("div");
    errorMessage.classList.add("error-message");
    errorMessage.textContent = message;

    field.parentElement.classList.add("error");
    field.parentElement.append(errorMessage);
}

export function clearErrors(form) {
    Array.from(form.querySelectorAll(".error-message")).forEach(error => {
        error.remove();
    });
    Array.from(form.querySelectorAll(".login-field")).forEach(error => {
        if (error.classList.contains("error")) {
            error.classList.remove("error");
        }
    })
}