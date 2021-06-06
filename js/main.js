(() => {
    let $burger = document.querySelector('.navbar-burger');
    let $menu = document.querySelector('.navbar-menu');
    $burger.addEventListener('click', () => {
        console.log("we clickin");
        $burger.classList.toggle('is-active');
        $menu.classList.toggle('is-active');
    });

    let $pre = document.querySelectorAll('pre');
    $pre.forEach(element => element.classList = "hljs");
})();
