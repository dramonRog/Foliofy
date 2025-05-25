const status = document.querySelector("#status");
const radios = Array.from(document.querySelectorAll(".status"));

radios.forEach(radio => {
    radio.addEventListener("click", () => {
        if (!radio.classList.contains("active")) {
            radios.find(radioEl => radioEl.classList.contains("active")).classList.remove("active");
            radio.classList.add("active");
            status.value = radio.textContent.trim();
            console.log(status.value);
        }
    });
});