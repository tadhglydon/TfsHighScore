$(document).ready(
    function () {
        setInterval(pulse, 1000);
        $("#gameOver").hide().fadeIn(10);
        $('#hsTitle').hide().fadeIn(13000);
    }
);
function pulse() {
    $('#gameOver').fadeIn(1000);
    $('#gameOver').fadeOut(1000);
}