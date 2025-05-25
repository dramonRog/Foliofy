const uploadButton = document.querySelector("#upload-cover");
const coverInput = document.querySelector("#uploadedCover");
const uploadedImage = document.querySelector(".uploaded-image");

uploadButton.addEventListener("click", () => coverInput.click());

coverInput.addEventListener("change", () => {
    if (coverInput.files[0]) {
        const fileReader = new FileReader();
        fileReader.addEventListener("load", (event) => {
            uploadedImage.src = event.target.result
        });

        fileReader.readAsDataURL(coverInput.files[0]);
    }
});