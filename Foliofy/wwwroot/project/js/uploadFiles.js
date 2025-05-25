const fileInput = document.querySelector("#uploadedFiles");
const uploadBtn = document.querySelector(".uploaded-files .upload-file");
const filesContainer = document.querySelector(".uploaded-files-container");

const MAX_TOTAL_SIZE = 45 * 1024 * 1024;

window.selectedFiles = [];

uploadBtn.addEventListener("click", () => fileInput.click());

fileInput.addEventListener("change", () => {

    for (const file of fileInput.files) {
        const currentTotal = selectedFiles.reduce((sum, f) => sum + f.size, 0);
        if (currentTotal + file.size > MAX_TOTAL_SIZE) {
            displayError(fileInput, "The total file size cannot exceed 45 MB!");
            setTimeout(() => { 
                fileInput.parentElement.classList.remove("error");
                fileInput.parentElement.querySelector(".error-message").remove();
            }, 3000);
        }
        else if (!selectedFiles.some(f => f.name === file.name && f.size === file.size)) {
            selectedFiles.push(file);
            addFileToDOM(file);
        }
    }

    fileInput.value = "";
});

function addFileToDOM(file) {
    const fileBlock = document.createElement("div");
    fileBlock.className = "file-entry";

    const fileName = document.createElement("a");
    fileName.textContent = file.name;
    fileName.href = URL.createObjectURL(file);
    fileName.target = "_blank";

    const removeBtn = document.createElement("button");
    removeBtn.className = "remove-file";
    removeBtn.innerHTML = `<span></span>
                           <span></span>`;

    removeBtn.addEventListener("click", () => {
        selectedFiles = selectedFiles.filter(f => f !== file);
        URL.revokeObjectURL(fileName.href);
        fileBlock.remove();
    });

    fileBlock.appendChild(fileName);
    fileBlock.appendChild(removeBtn);
    filesContainer.appendChild(fileBlock);
}


