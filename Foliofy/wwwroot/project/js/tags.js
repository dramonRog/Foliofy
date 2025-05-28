const tagNameInput = document.querySelector("#tagName");
const addTag = document.querySelector("#add-custom-tag");
const tagsContainer = document.querySelector(".tags-container");
const removeButton = document.querySelector(".remove-tag");

addTag.addEventListener("click", () => {
    clearWarning(addTag);
    if (tagNameInput.value.trim() === "") {
        displayWarning(addTag, "Tag cannot be empty!");
    } else if (tags.find(tag => tagNameInput.value.trim() === tag) !== undefined) {
        displayWarning(addTag, "Tag already exists!");
    } else if (tagNameInput.value.trim().length > 30) {
        displayWarning(addTag, "Tag cannot contain more than 30 characters!")
    }
    else {
        tags.push(tagNameInput.value.trim());
        uppendTagToDocument(tagNameInput.value.trim());
        tagNameInput.value = "";
    }
});

removeButton.addEventListener("click", () => {
    const tagContainer = document.querySelectorAll(".tag-wrapper");

    for (let child of tagContainer) {
        if (child.querySelector(".custom-checkbox").classList.contains("checked")) {
            tags = tags.filter(tag => tag !== child.querySelector(".tag").textContent.trim());
            child.remove();
        }
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

    setTimeout(() => clearWarning(field), 3000)
}

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
