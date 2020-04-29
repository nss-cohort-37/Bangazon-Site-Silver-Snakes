// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function clickListener() {
    document.querySelector("#local-delivery-checkbox").addEventListener(
        "click",
        theClickEvent => {
            const contentTarget = document.querySelector("#city-input")
            if (contentTarget.classList.contains("hidden")) {
                contentTarget.classList.remove("hidden")
            }
            else {
                contentTarget.classList.add("hidden")
            }
        }
    )
}

clickListener()