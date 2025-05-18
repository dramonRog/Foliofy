const sliderContainer = document.querySelector('.slider-container');
const leftArrow = document.querySelector('.left-arrow');
const rightArrow = document.querySelector('.right-arrow');
const gap = 35;

const sliderItems = Array.from(sliderContainer.querySelectorAll('.slider-item'));
const dots = Array.from(document.querySelectorAll('.dot'));

const totalItems = sliderItems.length; 
let isSliding = false;

sliderItems.forEach((item, index) => {
    item.style.order = index;
});

function updateDots() {
    const activeIndex = sliderItems.findIndex(
        item => parseInt(item.style.order, 10) === 0
    );

    dots.forEach((dot, index) => {
        dot.classList.toggle('active', index === activeIndex);
    });
}

function animateSlide(steps, direction) {
    if (isSliding) return;
    isSliding = true;
    const moveDistance = (sliderItems[0].offsetWidth + gap) * steps;

    sliderItems.forEach(item => {
        item.style.transition = `transform .65s ease`;
        item.style.transform = `translateX(${direction * -moveDistance}px)`;
    });

    let completed = 0;

    function onTransitionEnd() {
        completed++;
        if (completed < sliderItems.length) return;

        sliderItems.forEach(item => {
            item.style.transition = 'none';
            item.style.transform = 'none';
            item.removeEventListener('transitionend', onTransitionEnd);
        });

        sliderItems.forEach(item => {
            let currentOrder = parseInt(item.style.order, 10);
            if (direction === 1) {
                currentOrder = (currentOrder - steps + totalItems) % totalItems;
            } else {
                currentOrder = (currentOrder + steps) % totalItems;
            }
            item.style.order = currentOrder;
        });

        updateDots();
        isSliding = false;
    }

    sliderItems.forEach(item => {
        item.addEventListener('transitionend', onTransitionEnd);
    });
}

let autoSlideInterval;

function resetAutoSlide() {
    clearInterval(autoSlideInterval);
    autoSlideInterval = setInterval(() => {
        animateSlide(1, 1);
    }, 5000);
}


rightArrow.addEventListener('click', () => {
    animateSlide(1, 1);
    resetAutoSlide();
});

leftArrow.addEventListener('click', () => {
    animateSlide(1, -1);
    resetAutoSlide();
});

dots.forEach((dot, dotIndex) => {
    dot.addEventListener('click', () => {
        const currentIndex = sliderItems.findIndex(
            item => parseInt(item.style.order, 10) === 0
        );

        let diff = dotIndex - currentIndex;
        if (diff === 0) return;

        if (Math.abs(diff) > totalItems / 2) {
            if (diff > 0) diff -= totalItems;
            else diff += totalItems; 
        }

        const steps = Math.abs(diff);
        const direction = diff > 0 ? 1 : -1; 

        animateSlide(steps, direction);
        resetAutoSlide();
    });
});

updateDots();
resetAutoSlide();
