document.addEventListener("DOMContentLoaded", function () {
    const header = document.querySelector(".header");
    const scrollThreshold = 10;

    function handleScroll() {
        if (window.scrollY > scrollThreshold) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled")
        }
    }

    window.addEventListener('scroll', handleScroll);

    //slider
    const slider = document.querySelector(".hero-slider-bg");
    if (slider) {
        const slides = document.querySelectorAll(".slide")
        let currentSlideIndex = 0;
        const intervalTime = 5000;

        function nextSlide() {
            slides[currentSlideIndex].classList.remove('active');
            currentSlideIndex = (currentSlideIndex + 1) % slides.length;
            slides[currentSlideIndex].classList.add('active');
        }
        setInterval(nextSlide, intervalTime);
    }
});