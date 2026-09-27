// Smooth scroll for anchor links
$(document).on('click', 'a[href^="#"]', function (e) {
    var target = $(this.getAttribute('href'));
    if (target.length) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: target.offset().top - 80
        }, 500);
    }
});

// Add stars to hero section
$(function () {
    var hero = $('.hero-stars');
    for (var i = 0; i < 50; i++) {
        var star = $('<div class="hero-star"></div>');
        star.css({
            top: Math.random() * 100 + '%',
            left: Math.random() * 100 + '%',
            width: (Math.random() * 4 + 1) + 'px',
            height: (Math.random() * 4 + 1) + 'px',
            animationDelay: (Math.random() * 5) + 's',
            animationDuration: (Math.random() * 3 + 3) + 's'
        });
        hero.append(star);
    }
});