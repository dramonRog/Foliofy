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
        selectInputOption.value = option.textContent.trim();

        filterProjects(selectInputOption.value);
    })
})

function filterProjects(filterValue) {
	const projectItems = document.querySelectorAll(".project-item");

	projectItems.forEach(project => {
		const status = project.dataset.status.trim();

		if (filterValue === "All" || filterValue === status) {
			project.style.display = "flex"; 
		} else {
			project.style.display = "none";
		}
	});
}
