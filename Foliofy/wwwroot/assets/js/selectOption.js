
const selects = document.querySelectorAll('.register-field.select');

selects.forEach(select => {
    const optionList = select.querySelector('.creative-type');
    select.addEventListener('click', () => {
        optionList.classList.toggle('clicked');
    });

    const options = select.querySelectorAll(".register-option");
    const selected = select.querySelector(".selected-value");
    const selectInput = select.parentElement.querySelector("#creativeType");

    options.forEach(option => {
        option.addEventListener("click", () => {
            selected.innerHTML = option.innerHTML;
            if (!selected.classList.contains("chosen"))
                selected.classList.add("chosen");
            selectInput.value = option.textContent;
        });
    });
});
