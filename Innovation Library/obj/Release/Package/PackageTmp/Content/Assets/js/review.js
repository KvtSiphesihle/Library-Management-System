$(document).ready(function () {
  $("#review").val("");
  //Variables Initialization
  let star1 = $("#star-1");
  let star2 = $("#star-2");
  let star3 = $("#star-3");
  let star4 = $("#star-4");
  let star5 = $("#star-5");
  let starCounter = $(".starCount");

  const stars = [star1, star2, star3, star4, star5];
  const activeStars = [];
  let starCount = 0;

  function checkClass(item) {
    if (item.hasClass("active-star")) {
      activeStars.push(item);
      starCount = activeStars.length;
    } else {
      if (!item.hasClass("active-star")) {
        activeStars.splice(0, item.index);
      }
    }
    console.log(starCount);
  }

  function loopStars() {
    for (let i = 0; i < stars.length; i++) {
      if (stars[i].hasClass("active-star")) {
        starCount += 1;
      }
    }
  }

  //Star 1 rating click event
  star1.on("click", function () {
    if (star1.hasClass("active-star")) {
      $(".rate-icon").removeClass("active-star");
      starCount = 0;
    } else {
      star1.addClass("active-star");
      starCount = 1;
    }

    starCounter.text(starCount);
  });

  //Star 2 rating click event
  star2.on("click", function () {
    if (!star1.hasClass("active-star") || !star2.hasClass("active-star")) {
      star1.addClass("active-star");
      star2.addClass("active-star");
      starCount = 2;
    } else if (star2.hasClass("active-star")) {
      $(star2.removeClass("active-star"));
      $(star3.removeClass("active-star"));
      $(star4.removeClass("active-star"));
      $(star5.removeClass("active-star"));
      if (starCount == 2) {
        starCount -= 1;
      } else {
        starCount -= 4;
      }
    } else {
      if (star2.hasClass("active-star")) {
        star2.removeClass("active-star");
        star3.removeClass("active-star");
        $(star4.removeClass("active-star"));
        $(star5.removeClass("active-star"));
        starCount -= 4;
      }
    }
    //loopStars()
    starCounter.text(starCount);
  });

  //Star 3 rating click event
  star3.on("click", function () {
    if (
      !star1.hasClass("active-star") ||
      !star2.hasClass("active-star") ||
      !star3.hasClass("active-star")
    ) {
      star1.addClass("active-star");
      star2.addClass("active-star");
      star3.addClass("active-star");
      starCount = 3;
    } else if (star3.hasClass("active-star")) {
      $(star3.removeClass("active-star"));
      $(star4.removeClass("active-star"));
      $(star5.removeClass("active-star"));
      if (starCount == 3) {
        starCount -= 1;
      } else {
        starCount -= 3;
      }
    } else {
      if (star3.hasClass("active-star")) {
        star3.removeClass("active-star");
        star4.removeClass("active-star");
        star5.removeClass("active-star");
        starCount -= 3;
      }
    }
    //loopStars()
    starCounter.text(starCount);
  });

  //Star 4 rating click event
  star4.on("click", function () {
    if (
      !star1.hasClass("active-star") ||
      !star2.hasClass("active-star") ||
      !star3.hasClass("active-star") ||
      !star4.hasClass("active-star")
    ) {
      star1.addClass("active-star");
      star2.addClass("active-star");
      star3.addClass("active-star");
      star4.addClass("active-star");
      starCount = 4;
    } else if (star4.hasClass("active-star")) {
      $(star4.removeClass("active-star"));
      $(star5.removeClass("active-star"));
      if (starCount == 4) {
        starCount -= 1;
      } else {
        starCount -= 2;
      }
    } else {
      if (star4.hasClass("active-star")) {
        star4.removeClass("active-star");
        star5.removeClass("active-star");
        starCount -= 2;
      }
    }
    //loopStars()
    starCounter.text(starCount);
  });

  //Star 5 rating click event
  star5.on("click", function () {
    if (
      !star1.hasClass("active-star") ||
      !star2.hasClass("active-star") ||
      !star3.hasClass("active-star") ||
      !star4.hasClass("active-star") ||
      !star5.hasClass("active-star")
    ) {
      star1.addClass("active-star");
      star2.addClass("active-star");
      star3.addClass("active-star");
      star4.addClass("active-star");
      star5.addClass("active-star");
      starCount = 5;
    } else if (star5.hasClass("active-star")) {
      $(star5.removeClass("active-star"));
      starCount -= 1;
    } else {
      if (star5.hasClass("active-star")) {
        star5.removeClass("active-star");
        starCount -= 1;
      }
    }
    //loopStars()
    starCounter.text(starCount);
  });
});
