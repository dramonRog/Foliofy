const form = document.querySelector(".project-form");
const projectName = form.querySelector("#project-name");
const projectDescription = form.querySelector("#project-description");
const projectStatus = form.querySelector("#status");
const projectCover = form.querySelector("#uploadedCover");
const uploadedFiles = form.querySelector("#uploadedFiles");
const userToken = document.querySelector('input[name="__RequestVerificationToken"]');

form.addEventListener("submit", (event) => {
    event.preventDefault();
    clearErrors(form);

    validateProjectInput(projectName);
    validateProjectInput(projectDescription);

    if (tags.length < 1) {
        displayError(tagsContainer, "Your project must contain at least one tag!");
    }

    if (selectedFiles.length < 1) {
        displayError(uploadedFiles, "Your project must contain at least one file with content!");
    }

    if (projectCover.value.trim() === "") {
        displayError(projectCover, "You must send the image!");
    }

    if (form.querySelector(".error-message") === null) {
        const formData = new FormData();
        formData.append("__RequestVerificationToken", userToken.value);
        formData.append("projectName", projectName.value);
        formData.append("projectDescription", projectDescription.value);
        formData.append("projectStatus", projectStatus.value);
        formData.append("projectTags", tags.join(','));
        formData.append("projectCoverImage", projectCover.files[0]);
        selectedFiles.forEach(file => {
            formData.append("projectFiles", file);
        });

        fetch("?handler=CreateProject", {
            method: "POST",
            body: formData
        })
            .then(async response => {
                let resultData = await response.json();
                if (response.ok) {
                    window.location.href = "profile";
                }
                else if (resultData.projectName !== undefined) {
                    displayError(projectName, resultData.projectName);
                }
            })
            .catch(error => {
                alert("An error occurred! Please, try again later");
                console.log("Error", error);
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
    Array.from(form.querySelectorAll(".field")).forEach(error => {
        if (error.classList.contains("error")) {
            error.classList.remove("error");
        }
    })
}

function validateProjectInput(projectInput) {
    if (projectInput.value.trim() === "") {
        displayError(projectInput, "This field cannot be empty!");
    } else if (projectInput.value.length < 5) {
        displayError(projectInput, "This field must contain at least 5 characters!");
    }
}