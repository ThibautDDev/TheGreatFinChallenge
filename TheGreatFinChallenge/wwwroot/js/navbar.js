//Function to make the navbar stick to the top of the page on scroll
$(function () {
    $(window).on('scroll', function () {
        if ($(window).scrollTop() > ($(window).height() - 75)) {
            $('.navbar').addClass('active');
        } else {
            $('.navbar').removeClass('active');
        }
    });
});
