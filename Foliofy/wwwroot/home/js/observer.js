const appearingComments = Array.from(document.querySelectorAll(".comment__item"));

const observer = new IntersectionObserver((elems) => {
    elems.forEach(elem => {
        if (elem.isIntersecting) {
            elem.target.classList.add("visible");
        }
    });
}, { threshold: 0.25 });

appearingComments.forEach(comment => {
    observer.observe(comment);
});
