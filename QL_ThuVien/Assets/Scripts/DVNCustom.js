// insert commas as thousands separators 
function addCommas(n) {
    var rx = /(\d+)(\d{3})/;
    return String(n).replace(/^\d+/, function (w) {
        while (rx.test(w)) {
            w = w.replace(rx, '$1,$2');
        }
        return w;
    });
}
// return integers and decimal numbers from input
// optionally truncates decimals- does not 'round' input
function validDigits(n, dec) {
    n = n.replace(/[^\d\.]+/g, '');
    var ax1 = n.indexOf('.'), ax2 = -1;
    if (ax1 != -1) {
        ++ax1;
        ax2 = n.indexOf('.', ax1);
        if (ax2 > ax1) n = n.substring(0, ax2);
        if (typeof dec === 'number') n = n.substring(0, ax1 + dec);
    }
    return n;
}
window.onload = function () {
    var n1 = document.getElementById('number');
    n1.value = '';

    n1.onkeyup = n1.onchange = function (e) {
        e = e || window.event;
        var who = e.target || e.srcElement, temp;
        if (who.id === 'number') temp = validDigits(who.value, 2);
        else temp = validDigits(who.value);
        who.value = addCommas(temp);
    }
    n1.onblur = function () {
        var temp = parseFloat(validDigits(n1.value));
        if (temp) n1.value = addCommas(temp);
    }
}