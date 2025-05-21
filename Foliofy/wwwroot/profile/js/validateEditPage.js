const form = document.querySelector(".modal-edit");

form.addEventListener("submit", (e) => {
    e.preventDefault();
    clearErrors(form);

    const username = form.querySelector("#username");
    const email = form.querySelector("#email");
    const about = form.querySelector("#aboutInfo");
    const uploadedIcon = form.querySelector('#UploadedIcon').files[0];
    const userToken = document.querySelector('input[name="__RequestVerificationToken"]');

    validateUsername(username);
    validateEmail(email);

    if (form.querySelectorAll(".field.error").length === 0) {
        const formData = new FormData();
        formData.append("__RequestVerificationToken", userToken.value);
        formData.append("Username", username.value);
        formData.append("Email", email.value);
        formData.append("UserDescription", about.value);
        formData.append("AddCustomTags", addedTags.join(','));
        formData.append("RemoveCustomTags", removedTags.join(','));
        formData.append("AddCreativeTypes", addCreativeTag.join(','));
        formData.append("RemoveCreativeTypes", removeCreativeTag.join(','));

        if (uploadedIcon)
            formData.append("UploadedIcon", uploadedIcon);
        fetch("/profile/EditProfile?handler=UpdateProfile", {
            method: "POST",
            body: formData
        })
            .then(async response => {
                const data = await response.json(); 
                if (!response.ok) {
                    if (data.Email != undefined) {
                        displayError(email, data.Email);
                    }
                    if (data.Username != undefined) {
                        displayError(username, data.Username);
                    }
                }
                else
                    window.location.href = "profile";
            })
            .catch(error => {
                alert("Something went wrong! Please, try again later.");
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
function clearErrors(form) {
    Array.from(form.querySelectorAll(".error-message")).forEach(error => {
        error.remove();
    });
    Array.from(form.querySelectorAll(".field")).forEach(error => {
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
