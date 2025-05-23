const selectField = document.querySelector(".filter-field.select");
const optionsList = selectField.querySelector(".project-type");
const filterOptions = selectField.querySelectorAll(".filter-option");
const selectedOption = selectField.querySelector(".selected-value");
const selectInputOption = selectField.querySelector("#projectsFilter");

selectField.addEventListener("click", () => {
    optionsList.classList.toggle("clicked");
});

filterOptions.forEach(option => {
    option.addEventListener("click", () => {
        selectedOption.innerHTML = option.innerHTML;
        if (!selectedOption.classList.contains("chosen"))
            selectedOption.classList.add("chosen");
        selectInputOption.value = option.textContent;
    })
})