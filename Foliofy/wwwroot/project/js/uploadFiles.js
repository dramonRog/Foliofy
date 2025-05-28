const fileInput = document.querySelector("#uploadedFiles");
const uploadBtn = document.querySelector(".uploaded-files .upload-file");
const filesContainer = document.querySelector(".uploaded-files-container");
const removeFiles = Array.from(document.querySelectorAll(".remove-file"));

if (removeFiles) {
    removeFiles.forEach(button => {
        button.addEventListener("click", () => {
            const fileBlock = button.closest(".file-content");
            const fileName = fileBlock.querySelector("a").textContent.trim();

            selectedFiles = selectedFiles.filter(f => f.name !== fileName);
            fileBlock.remove();
            updateExistingFileNames();
        });
    });
}

const MAX_TOTAL_SIZE = 45 * 1024 * 1024;

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
    fileBlock.className = "file-content";

    const objectUrl = URL.createObjectURL(file)

    fileBlock.innerHTML = `
        <div class="file-entry">
            <img src="/assets/images/icons/file-icon.svg">
            <a href="${objectUrl}" target="_blank">${file.name}</a>
        </div>
        <button class="remove-file" type="button">
            <span></span>
            <span></span>
        </button>
    `

    fileBlock.querySelector(".remove-file").addEventListener("click", () => {
        selectedFiles = selectedFiles.filter(f => f !== file);
        URL.revokeObjectURL(objectUrl);
        fileBlock.remove();
    });

    filesContainer.appendChild(fileBlock);
}

function updateExistingFileNames() {
    const existingFileNames = selectedFiles
        .filter(f => !(f instanceof File))
        .map(f => f.name);

    document.querySelector("#existingFileNames").value = existingFileNames.join(",");
}
