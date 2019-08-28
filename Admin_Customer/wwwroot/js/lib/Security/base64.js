//// mã hóa
//function b64EncodeUnicode(str) {
//    if (str == null || str == void 0 || str == "") return "";
//    // first we use encodeURIComponent to get percent-encoded UTF-8,
//    // then we convert the percent encodings into raw bytes which
//    // can be fed into btoa.
//    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
//        function toSolidBytes(match, p1) {
//            return String.fromCharCode('0x' + p1);
//        }));
//}
//// giải mã
//function b64DecodeUnicode(str) {
//    if (str == null || str == void 0 || str == "") return "";
//    // Going backwards: from bytestream, to percent-encoding, to original string.
//    return decodeURIComponent(atob(str).split('').map(function (c) {
//        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
//    }).join(''));
//}
// base 64
var base64 = (function () {
    //contructor
    function base64() {

    }
    // function mã hóa
    base64.prototype.encode = function (str) {
        if (str == null || str == void 0 || str == "") return "";
        // first we use encodeURIComponent to get percent-encoded UTF-8,
        // then we convert the percent encodings into raw bytes which
        // can be fed into btoa.
        return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,function toSolidBytes(match, p1) {return String.fromCharCode('0x' + p1);}));
    }
    // function giải mã
    base64.prototype.decode = function (str) {
        if (str == null || str == void 0 || str == "") return "";
        // Going backwards: from bytestream, to percent-encoding, to original string.
        return decodeURIComponent(atob(str).split('').map(function (c) {return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);}).join(''));
    }
    return base64;
}())