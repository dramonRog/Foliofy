const profileIcon = document.getElementById("profile-link");
const profileToggle = document.querySelector(".profile__toggle");
let hideTimeout;

function hideItem(item) {
    item.style.visibility = "hidden";
    item.style.opacity = 0;
}

function showItem(item) {
    item.style.visibility = "visible";
    item.style.opacity = 1;
}

const isHoverDevice = window.matchMedia("(hover: hover)").matches;

if (isHoverDevice) {
    [profileIcon, profileToggle].forEach(profile => {
        profile.addEventListener("mouseenter", () => {
            clearTimeout(hideTimeout);
            showItem(profileToggle);
        });

        profile.addEventListener("mouseleave", () => {
            hideTimeout = setTimeout(() => {
                if (!profileToggle.matches(":hover") && !profileIcon.matches(":hover")) 
                    hideItem(profileToggle);
            }, 300);
        });
    });

} else {
    profileIcon.addEventListener("click", () => {
        if (profileToggle.style.visibility === "visible") {
            hideItem(profileToggle);
        } else {
            showItem(profileToggle);
        }
    });
}



const burger = document.querySelector(".burger-menu");
const linkList = document.querySelector(".link__list");

burger.addEventListener("click", () => {
    burger.classList.toggle("active");
    linkList.classList.toggle("active");
});

document.addEventListener("keydown", (event) => {
    if (event.key === "Escape" && burger.classList.contains("active")) {
        burger.classList.remove("active");
        linkList.classList.remove("active");
    }
});