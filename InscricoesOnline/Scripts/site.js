function SubPagina(p) {
    $('.conteudo_subpag').css('display', 'none');
    $('#' + p).css('display', 'block');
    $('#sidenav01 li').removeClass("slc_submenu");
    $('#li_' + p).addClass("slc_submenu");
}

function SubPagina2(p) {
    $('.conteudo_subpag').css('display', 'none');
    $('#' + p).css('display', 'block');
    $('#sidenav02 li').removeClass("slc_submenu2");
    $('#li_' + p).addClass("slc_submenu2");
}

jQuery(document).ready(function ($) {
    $(".parceiros-carousel").owlCarousel({
        autoplay: true,
        dots: true,
        loop: true,
        responsive: {
            0: { items: 1 }, 450: { items: 2 }, 768: { items: 2 }, 900: { items: 2 }
        }
    });

    $(".clients-carousel").owlCarousel({
        autoplay: true,
        dots: true,
        loop: true,
        responsive: {
            0: { items: 2 }, 768: { items: 4 }, 900: { items: 6 }
        }
    });

    $('.flexslider').flexslider({
        animation: "fade",
        controlNav: false
    });

    if ($(window).width() <= 719) {
        $(".wow").removeClass("wow");
    }
    new WOW().init();

    $(".cta").hover(
    function () {
        $(this).addClass("animated pulse");
    },
    function () {
        $(this).removeClass("animated pulse");
    }
  );;
});