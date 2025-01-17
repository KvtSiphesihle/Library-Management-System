let backDrop = document.querySelector(".backdrop");
let close = document.querySelector(".close");
let buttonOptions = document.querySelector(".option-buttons");
let openOptions = document.querySelectorAll(".open-options");

//openOptions.onclick = () => {
//  manageBackDrop("open");
//};

//openOptions.forEach((element) => {
//    element.onclick = () => {
//        var bckDropId = backDrop.getAttribute("id");
//        console.log(bckDropId)
//    }
//})

//close.onclick = () => {
//  manageBackDrop("close");
//};
function manageBackDrop(state) {
  if (state == "open") {
    backDrop.style.display = "block";
    buttonOptions.style.display = "flex";
  } else if (state == "close") {
    backDrop.style.display = "none";
    buttonOptions.style.display = "none";
  }
}
