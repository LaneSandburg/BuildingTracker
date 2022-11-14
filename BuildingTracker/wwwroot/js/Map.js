/// <reference path="jquery.rwdimagemaps.js" />
var flip = true;
function changeImage(UL, LL) {
    
    if (flip == true) {
        document.getElementById("img2").src = UL;
        document.getElementById("img1").src = LL;
        rwdImageMaps();
        flip = false;
    } else{
        document.getElementById("img1").src = UL;
        document.getElementById("img2").src = LL;
        rwdImageMaps();
        flip = true;
    }
    
} $(document).ready(function () {
    $('img[usemap]').rwdImageMaps();
});