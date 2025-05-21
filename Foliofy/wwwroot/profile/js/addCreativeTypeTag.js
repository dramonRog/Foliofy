const addTagButton = document.querySelector("#add-creative-type-tag");
const removeButtons = document.querySelectorAll(".remove-btn");
const creativeType = document.querySelector("#creative-type-tags");

window.addCreativeTag = [];
window.removeCreativeTag = [];

addTagButton.addEventListener("click", () => {
    clearWarning(addTagButton);
    if (creativeType.value.trim() === "")
        displayWarning(addTagButton, "Please, choose the creative type");
    else if (addCreativeTag.includes(creativeType.value.trim()))
        displayWarning(addTagButton, "This creative type is already taken!");
    else {
        const formData = new FormData();
        formData.append("__RequestVerificationToken", token.value);
        formData.append("CreativeType", creativeType.value.trim());
        formData.append("removeCreativeTagsList", removeCreativeTag.join(','));

        fetch("/profile/EditProfile?handler=CheckCreativeType", {
            method: "POST",
            body: formData
        })
        .then(response => response.json())
        .then(resultData => {
            if (resultData) {
                addCreativeTag.push(creativeType.value.trim());
                removeCreativeTag = removeCreativeTag.filter(tag => tag !== creativeType.value.trim());
                addTag(creativeType.value.trim());
                creativeType.value = "";
            } else {
                displayWarning(addTagButton, "That creative type is already taken!");
            }
        })
        .catch(error => {
            alert("Something went wrong! Please, try again later.")
            console.error("Error", error);
        });
    }
});

removeButtons.forEach(removeButton => removeButton.addEventListener("click", () => removeButtonClick(removeButton)));

function addTag(tag) {
    const tagContainer = document.querySelector(".creative-tags-container");

    const creativeTag = document.createElement("div");
    creativeTag.classList.add("creation-type-tag");
    creativeTag.innerHTML = `<span class="creative-tag">
									<img src="/assets/images/icons/creativeMinds/people.svg" />
									${tag}
							</span>
							<button class="remove-btn" type="button"><img src="/assets/images/icons/edit/remove-icon.svg" /></button>`;
    const removeBtn = creativeTag.querySelector(".remove-btn");
    removeBtn.addEventListener("click", () => { removeButtonClick(removeBtn, tag) });
    tagContainer.appendChild(creativeTag);
}

function removeButtonClick(button) {
    let tag = button.parentElement.querySelector(".creative-tag").textContent.trim();
    removeCreativeTag.push(tag);
    addCreativeTag = addCreativeTag.filter(creativeTag => creativeTag !== tag);

    button.parentElement.remove();
}