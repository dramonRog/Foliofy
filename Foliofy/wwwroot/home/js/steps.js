const steps = document.querySelectorAll(".creative-step");

steps.forEach(step => {
    step.addEventListener("click", () => {
        if (!step.classList.contains("active")) {
            const activeElement = document.querySelector(".creative-step.active");
            if (activeElement) {
                activeElement.classList.remove("active");
                setTimeout(() => { step.classList.add("active") }, 400);
            } else {
                step.classList.add("active");
            }
        } else {
            step.classList.remove("active");
        }
    });
});
