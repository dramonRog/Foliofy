const addCustomTag = document.querySelector("#add-custom-tag");
const customTagName = document.querySelector("#tagName");
const token = document.querySelector('input[name="__RequestVerificationToken"]');
const tagContainer = document.querySelector(".tags-container");
const removeCustomTags = document.querySelector(".remove-tag");

window.addedTags = [];
window.removedTags = [];

addCustomTag.addEventListener("click", () => {
    clearWarning(addCustomTag);
    if (customTagName.value === "") {
        displayWarning(addCustomTag, "Tag cannot be empty!");
    } else if (addedTags.find(tag => tag === customTagName.value) !== undefined) {
        displayWarning(addCustomTag, "Tag already exists!")
    } else {
        const formData = new FormData();
        formData.append("__RequestVerificationToken", token.value);
        formData.append("CustomTagName", customTagName.value.trim());
        formData.append("removedTagsList", removedTags.join(","));
        fetch("/profile/EditProfile?handler=CheckTag", {
            method: "POST",
            body: formData
        })
        .then(response => response.json())
        .then(resultData => {
            if (resultData) {
                addedTags.push(customTagName.value.trim());
                uppendTagToDocument(customTagName.value);
                removedTags = removedTags.filter(removeTag => removeTag !== customTagName.value.trim());
                customTagName.value = "";
            } else {
                displayWarning(addCustomTag, "That tag already exists!");
            }
        })
        .catch(error => {
            console.error("Error: ", error);
            alert("Something went wrong. Please, try again later");
        });
    }
});

function clearWarning(element) {
    element.parentElement.classList.remove("warning");
    if (element.parentElement.querySelector(".warning-message"))
        element.parentElement.querySelector(".warning-message").remove();
}

function displayWarning(field, message) {
    let warningMessage = document.createElement("div");
    warningMessage.classList.add("warning-message");
    warningMessage.textContent = message;

    field.parentElement.classList.add("warning");
    field.parentElement.append(warningMessage);

    setTimeout(() => clearWarning(field),3000)
}

removeCustomTags.addEventListener("click", () => {
    const tagContainer = document.querySelectorAll(".tag-wrapper");

    for (let child of tagContainer) {
        if (child.querySelector(".custom-checkbox").classList.contains("checked")) {
            removedTags.push(child.querySelector(".tag").textContent.trim());
            child.remove();
        }
    }

    for (let removeTag of removedTags) {
        addedTags = addedTags.filter(tag => tag != removeTag);
    }
})

function uppendTagToDocument(tag) {
    const tagContainer = document.querySelector(".tags-container");
    const tagElement = document.createElement("div");
    tagElement.classList.add("tag-wrapper");
    tagElement.innerHTML = `<span class=\"custom-checkbox\"><img src=\"/assets/images/icons/register/daw.svg\" /></span>
						<span class="tag">${tag}</span>`

    const checkbox = tagElement.querySelector(".custom-checkbox");
    checkbox.addEventListener("click", () => {
        checkbox.classList.toggle("checked");
    })
    tagContainer.appendChild(tagElement);
}
