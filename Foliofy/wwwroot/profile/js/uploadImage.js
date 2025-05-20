const uploadButton = document.querySelector(".change-icon-btn"); 
const imageInput = document.querySelector("#UploadedIcon");    
const profileIcon = document.querySelector("#iconPreview");     

uploadButton.addEventListener("click", () => {
    imageInput.click();
});

imageInput.addEventListener("change", () => {
    const file = imageInput.files[0];

    if (!file) return;

    const fileReader = new FileReader();
    fileReader.addEventListener("load", (e) => {
        profileIcon.src = e.target.result;
    });

    fileReader.readAsDataURL(file);
});
