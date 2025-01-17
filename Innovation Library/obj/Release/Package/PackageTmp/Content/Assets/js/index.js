//Variables
let menuToggler = document.querySelector(".menu-toggler-icon");
let cancelToggler = document.querySelector(".back-icon");
let formBackdrop = document.querySelector(".form-backdrop");
let formSubmit = document.querySelector(".form-submit");
let menuCollapse = document.querySelector(".collapse-menu");

menuToggler.onclick = () => {
  toggleIcon(menuToggler, cancelToggler);
  menuCollapse.style.display = "block";
  menuCollapse.style.top = "105%";
};

cancelToggler.onclick = () => {
  toggleIcon(cancelToggler, menuToggler);
  menuCollapse.style.display = "none";
};

function toggleIcon(activeIcon, newIcon) {
  activeIcon.classList.add("d-none");
  newIcon.classList.remove("d-none");
}
