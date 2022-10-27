$(function () {

    $(".top_icon a.close").click(function () {
        $(this).transition({ rotate: 180 }, 1000);
        $(".top_icon").animate({ height: 0 }, 500);
        $("#ac-gn-menustate").css("top", "0px");
    });

    $(".header dl").click(function () {
        $(".bg_s").fadeIn();

        document.ontouchmove = function (e) {
            return false;
        }
    })

    $(".bg_s dd").click(function () {
        $(".bg_s").fadeOut();
        document.ontouchmove = function (e) {
            return true;
        }
    })


    if ($(".top_icon").css({ height: 0 })) {
        $("#ac-gn-menustate").css("top", "0px");
    }

    $(window).scroll(function () {
        if ($(window).scrollTop() > 400) {
            $(".SideMenu_bot").css("display", "block");
        } else {
            $(".SideMenu_bot").css("display", "none");
        }
    });


    $(".SideMenu_bot tt").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 1000);
    });

    $(".SideMenu_bg").click(function () {
        $("#ac-gn-menustate").attr("checked", false);
        $(this).fadeOut();
    });

    $(".day_menu").click(function () {
        $(".Calendar").slideToggle();
        $(this).find("em").toggleClass("hover");
    })


    $(".flotage a.close").click(function () {
        $(".flotage").fadeOut();
    });



    $(".foot dt i").click(function () {
        var i = $(".foot dt i").index($(this));
        $(".foot dd").eq(i).slideToggle(500, function () {
            if ($(".foot dd").eq(i).is(":visible")) {
                $(".foot dt a").eq(i + 3).addClass("hover");
                $(".foot dt i").eq(i).find("img").transition({ rotate: '45deg' });
            } else {
                $(".foot dt a").eq(i + 3).removeClass("hover");
                $(".foot dt i").eq(i).find("img").transition({ rotate: '0deg' });
            }
        });

    });

    $(".about dl.about01:eq(3)").find("dd").css("border-bottom", "0");




});
function check() {
    if (this.checked) {
        document.ontouchmove = function (e) {
            return false;
        }
    }
    else {
        $(".SideMenu_bg").css("display", "block");
        return true;
        //  document.ontouchmove = function (e) {

        //    return true;
        //}		
    };
}




