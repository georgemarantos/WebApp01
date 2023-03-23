// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// This is only for the _Layout.cshtml file
"use strict"

$(document).ready(function () {

    // MOVE MENU TO SLIDING POSITION IF MOBILE SIZE, or move back
    $(window).resize(function (e) {
        if ((window.innerWidth < 601) && ($("header").hasClass("no-mobile"))) {
            $("header").removeClass("no-mobile");
            $("header").addClass("go-mobile");
            $("#utNav").insertAfter($("#catNav"));
            $('#slidemenu').addClass("collapse");
        } else {
            if ((window.innerWidth > 600) && ($("header").hasClass("go-mobile") === true)) {
                $("header").removeClass("go-mobile");
                $("header").addClass("no-mobile");
                $("#nav-toggle").removeClass("active");
                $(".mobile-only").html("Menu");
                $("#slidemenu").removeClass("collapse");
                $("header #utNav").insertAfter($("header section"));
            }
        }
    })

    // mobile drop down - class toggle  
    $(".navbar-toggle").on("click", function (e) {
        if ($("#nav-toggle").hasClass("active")) {
            $("#nav-toggle").removeClass("active");
            $(".mobile-only").html("Menu");
        } else {
            $("#nav-toggle").addClass("active");
            $(".mobile-only").html("Close");
        }
    })

    // ----  slide down menu  ---------
    $(window).trigger("resize");
})