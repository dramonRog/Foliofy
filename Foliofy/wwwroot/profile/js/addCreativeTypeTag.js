const addTagButton = document.querySelector("#add-creative-type-tag");
const removeButtons = document.querySelectorAll(".remove-btn");
const creativeType = document.querySelector("#creative-type-tags");

addTagButton.addEventListener("click", () => {
    clearWarning(addTagButton);
    if (creativeType.value.trim() === "")
        displayWarning(addTagButton, "Please, choose the creative type");
    else if (creativeTypesArray.includes(creativeType.value.trim()))
        displayWarning(addTagButton, "This creative type is already taken!");
    else {
        creativeTypesArray.push(creativeType.value.trim());
        addTag(creativeType.value.trim());
        creativeType.value = "";
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
    creativeTypesArray = creativeTypesArray.filter(creativeTag => creativeTag !== tag);

    button.parentElement.remove();
}