(function(){
    this.Text = function(){

    }
    Text.prototype.ClearUnicode = function(str,isLowCase,whiteSpace){
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
        str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
        str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
        str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
        str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
        str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
        str = str.replace(/Đ/g, "D");
        if(whiteSpace){
            str = str
            .replace(/[&]/g, "-and-")
            .replace(/[^a-zA-Z0-9._-]/g, "-")
            .replace(/[-]+/g, "-")
            .replace(/-$/, "");
        }
        return isLowCase ? str.toLowerCase(): str;
        
    }
    return Text;
}());
(function () {
    var _request=null;
    function Ajax() {
        _request = new XMLHttpRequest();
        this._request = _request;
    }
    window.Ajax = Ajax;
    Ajax.prototype.proccess = function (method, url, data, async) {
        var request = this._request;
        return new Promise(function (resolve, reject) {
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    // Process the response
                    if (request.status >= 200 && request.status < 300) {
                        // If successful
                        resolve(request.response);
                    } else {
                        reject({
                            status: request.status,
                            statusText: request.statusText
                        });
                    }
                }
            }
            request.open(method || 'POST', url, async || true);
            //request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;multipart/form-data;application/json');
            // Send the request
            try {
                request.send(data);
            } catch (err) {
                request.setRequestHeader('Content-type', 'application/json; charset=utf-8');
                request.send(data);
            }
        });
    }
    Ajax.prototype.proccessWithDataHeader = function (method, url, dataheader, async) {
        var request = this._request;
        var data = null;
        return new Promise(function (resolve, reject) {
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    // Process the response
                    if (request.status >= 200 && request.status < 300) {
                        // If successful
                        resolve(request.response);
                    } else {
                        reject({
                            status: request.status,
                            statusText: request.statusText,
                            request : request
                        });
                    }
                }
            }
            request.open(method || 'POST', url, async || true);

            if(dataheader){
                var keys = Object.keys(dataheader);
                if(keys != null && keys.length > 0){
                    for(var i = 0; i < keys.length ; i++){
                        if(dataheader[keys[i]]){
                            request.setRequestHeader(keys[i],dataheader[keys[i]]);
                        }
                    }
                }
            }
            //request.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;multipart/form-data;application/json');
            // Send the request
            try {
                request.send(data);
            } catch (err) {
                request.setRequestHeader('Content-type', 'application/json; charset=utf-8');
                request.send(data);
            }
        });
    }

    Ajax.prototype.proccessData = function (method, url, data ,dataheader, async) {
        var request = this._request;
        return new Promise(function (resolve, reject) {
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    // Process the response
                    if (request.status >= 200 && request.status < 300) {
                        // If successful
                        resolve(request.response);
                    } else {
                        reject({
                            status: request.status,
                            statusText: request.statusText
                        });
                    }
                }
            }
            request.open(method || 'POST', url, async || true);
            if(dataheader){
                var keys = Object.keys(dataheader);
                if(keys != null && keys.length > 0){
                    for(var i = 0; i < keys.length ; i++){
                        if(dataheader[keys[i]]){
                            request.setRequestHeader(keys[i],dataheader[keys[i]]);
                        }
                    }
                }
            }
            request.send(data);
        });
    }
    Ajax.prototype.creatFormData = function (obj, frm) {
        var data = frm == void 0 || frm == null || typeof (frm) != "object"
            ? new FormData()
            : new FormData(frm);
        for (var key in obj) {
            if (data.hasOwnProperty(key)) {
                data[key] = obj[key];
            } else {
                data.append(key, obj[key]);
            }
        }
        return data;
    }
    Ajax.prototype.getResponseHeader = function(url){
        var client = new XMLHttpRequest();
        return new Promise(function (resolve, reject) {
            console.log(client.readyState);
            client.onreadystatechange = function () {
                console.log(client.readyState);
                if(client.readyState == 2){
                    var contentType = client.getResponseHeader("location");
                    client.abort();
                    console.log(contentType);
                    resolve(contentType);
                }
            }
            client.open("GET", url, true);
            client.setRequestHeader("Access-Control-Allow-Origin","*");
            client.send();
        });
    }
    return Ajax;
}());
(function(){
    var keyCodes = {
        0: 'That key has no keycode',
        3: 'break',
        8: 'backspace / delete',
        9: 'tab',
        12: 'clear',
        13: 'enter',
        16: 'shift',
        17: 'ctrl',
        18: 'alt',
        19: 'pause/break',
        20: 'caps lock',
        21: 'hangul',
        25: 'hanja',
        27: 'escape',
        28: 'conversion',
        29: 'non-conversion',
        32: 'spacebar',
        33: 'page up',
        34: 'page down',
        35: 'end',
        36: 'home',
        37: 'left arrow',
        38: 'up arrow',
        39: 'right arrow',
        40: 'down arrow',
        41: 'select',
        42: 'print',
        43: 'execute',
        44: 'Print Screen',
        45: 'insert',
        46: 'delete',
        47: 'help',
        48: '0',
        49: '1',
        50: '2',
        51: '3',
        52: '4',
        53: '5',
        54: '6',
        55: '7',
        56: '8',
        57: '9',
        58: ':',
        59: 'semicolon (firefox), equals',
        60: '<',
        61: 'equals (firefox)',
        63: 'ß',
        64: '@ (firefox)',
        65: 'a',
        66: 'b',
        67: 'c',
        68: 'd',
        69: 'e',
        70: 'f',
        71: 'g',
        72: 'h',
        73: 'i',
        74: 'j',
        75: 'k',
        76: 'l',
        77: 'm',
        78: 'n',
        79: 'o',
        80: 'p',
        81: 'q',
        82: 'r',
        83: 's',
        84: 't',
        85: 'u',
        86: 'v',
        87: 'w',
        88: 'x',
        89: 'y',
        90: 'z',
        91: 'Windows Key / Left ⌘ / Chromebook Search key',
        92: 'right window key',
        93: 'Windows Menu / Right ⌘',
        95: 'sleep',
        96: 'numpad 0',
        97: 'numpad 1',
        98: 'numpad 2',
        99: 'numpad 3',
        100: 'numpad 4',
        101: 'numpad 5',
        102: 'numpad 6',
        103: 'numpad 7',
        104: 'numpad 8',
        105: 'numpad 9',
        106: 'multiply',
        107: 'add',
        108: 'numpad period (firefox)',
        109: 'subtract',
        110: 'decimal point',
        111: 'divide',
        112: 'f1',
        113: 'f2',
        114: 'f3',
        115: 'f4',
        116: 'f5',
        117: 'f6',
        118: 'f7',
        119: 'f8',
        120: 'f9',
        121: 'f10',
        122: 'f11',
        123: 'f12',
        124: 'f13',
        125: 'f14',
        126: 'f15',
        127: 'f16',
        128: 'f17',
        129: 'f18',
        130: 'f19',
        131: 'f20',
        132: 'f21',
        133: 'f22',
        134: 'f23',
        135: 'f24',
        136: 'f25',
        137: 'f26',
        138: 'f27',
        139: 'f28',
        140: 'f29',
        141: 'f30',
        142: 'f31',
        143: 'f32',
        144: 'num lock',
        145: 'scroll lock',
        151: 'airplane mode',
        160: '^',
        161: '!',
        162: '؛ (arabic semicolon)',
        163: '#',
        164: '$',
        165: 'ù',
        166: 'page backward',
        167: 'page forward',
        168: 'refresh',
        169: 'closing paren (AZERTY)',
        170: '*',
        171: '~ + * key',
        172: 'home key',
        173: 'minus (firefox), mute/unmute',
        174: 'decrease volume level',
        175: 'increase volume level',
        176: 'next',
        177: 'previous',
        178: 'stop',
        179: 'play/pause',
        180: 'e-mail',
        181: 'mute/unmute (firefox)',
        182: 'decrease volume level (firefox)',
        183: 'increase volume level (firefox)',
        186: 'semi-colon / ñ',
        187: 'equal sign',
        188: 'comma',
        189: 'dash',
        190: 'period',
        191: 'forward slash / ç',
        192: 'grave accent / ñ / æ / ö',
        193: '?, / or °',
        194: 'numpad period (chrome)',
        219: 'open bracket',
        220: 'back slash',
        221: 'close bracket / å',
        222: 'single quote / ø / ä',
        223: '`',
        224: 'left or right ⌘ key (firefox)',
        225: 'altgr',
        226: '< /git >, left back slash',
        230: 'GNOME Compose Key',
        231: 'ç',
        233: 'XF86Forward',
        234: 'XF86Back',
        235: 'non-conversion',
        240: 'alphanumeric',
        242: 'hiragana/katakana',
        243: 'half-width/full-width',
        244: 'kanji',
        251: 'unlock trackpad (Chrome/Edge)',
        255: 'toggle touchpad',
    };
      
    var keyLocations = {
        0: 'General keys',
        1: 'Left-side modifier keys',
        2: 'Right-side modifier keys',
        3: 'Numpad',
      };
      
    var spaceDescription = '(Space character)';

    
      
}());

/*!
 * Lazy Load Images without jQuery
 * http://kaizau.github.com/Lazy-Load-Images-without-jQuery/
 *
 * Original by Mike Pulaski - http://www.mikepulaski.com
 * Modified by Kai Zau - http://kaizau.com
 */
(function() {
    var addEventListener =  window.addEventListener || function(n,f) { window.attachEvent('on'+n, f); },
        removeEventListener = window.removeEventListener || function(n,f,b) { window.detachEvent('on'+n, f); };
  
    var lazyLoader = {
      cache: [],
      mobileScreenSize: 500,
      //tinyGif: 'data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==',
  
      addObservers: function() {
        addEventListener('scroll', lazyLoader.throttledLoad);
        addEventListener('resize', lazyLoader.throttledLoad);
      },
  
      removeObservers: function() {
        removeEventListener('scroll', lazyLoader.throttledLoad, false);
        removeEventListener('resize', lazyLoader.throttledLoad, false);
      },
  
      throttleTimer: new Date().getTime(),
  
      throttledLoad: function() {
        var now = new Date().getTime();
        if ((now - lazyLoader.throttleTimer) >= 200) {
          lazyLoader.throttleTimer = now;
          lazyLoader.loadVisibleImages();
        }
      },
  
      loadVisibleImages: function() {
        var scrollY = window.pageYOffset || document.documentElement.scrollTop;
        var pageHeight = window.innerHeight || document.documentElement.clientHeight;
        var range = {
          min: scrollY - 200,
          max: scrollY + pageHeight + 200
        };
  
        var i = 0;
        while (i < lazyLoader.cache.length) {
          var image = lazyLoader.cache[i];
          var imagePosition = getOffsetTop(image);
          var imageHeight = image.height || 0;
  
          if ((imagePosition >= range.min - imageHeight) && (imagePosition <= range.max)) {
            var mobileSrc = image.getAttribute('data-src-mobile');
  
            image.onload = function() {
              this.className = this.className.replace(/(^|\s+)lazy-load(\s+|$)/, '$1lazy-loaded$2');
            };
  
            if (mobileSrc && screen.width <= lazyLoader.mobileScreenSize) {
              image.src = mobileSrc;
            }
            else {
              image.src = image.getAttribute('data-src');
            }
  
            image.removeAttribute('data-src');
            image.removeAttribute('data-src-mobile');
  
            lazyLoader.cache.splice(i, 1);
            continue;
          }
  
          i++;
        }
  
        if (lazyLoader.cache.length === 0) {
          lazyLoader.removeObservers();
        }
      },
  
      init: function() {
        // Patch IE7- (querySelectorAll)
        if (!document.querySelectorAll) {
          document.querySelectorAll = function(selector) {
            var doc = document,
                head = doc.documentElement.firstChild,
                styleTag = doc.createElement('STYLE');
            head.appendChild(styleTag);
            doc.__qsaels = [];
            styleTag.styleSheet.cssText = selector + "{x:expression(document.__qsaels.push(this))}";
            window.scrollBy(0, 0);
            return doc.__qsaels;
          }
        }
  
        addEventListener('load', function _lazyLoaderInit() {
          var imageNodes = document.querySelectorAll('img[data-src]');
  
          for (var i = 0; i < imageNodes.length; i++) {
            var imageNode = imageNodes[i];
  
            // Add a placeholder if one doesn't exist
            //imageNode.src = imageNode.src || lazyLoader.tinyGif;
  
            lazyLoader.cache.push(imageNode);
          }
  
          lazyLoader.addObservers();
          lazyLoader.loadVisibleImages();
  
          removeEventListener('load', _lazyLoaderInit, false);
        });
      }
    }
  
    // For IE7 compatibility
    // Adapted from http://www.quirksmode.org/js/findpos.html
    function getOffsetTop(el) {
      var val = 0;
      if (el.offsetParent) {
        do {
          val += el.offsetTop;
        } while (el = el.offsetParent);
        return val;
      }
    }
  
    lazyLoader.init();
})();

(function () {
    var __SCRIPT_URL = "";
    function AlertMessage(scriptUrl) {
        __SCRIPT_URL = scriptUrl;
        init();
    }
    window.AlertMessage = AlertMessage;
    var init = function () {
        if ("Notification" in window) {
            if (Notification.permission != 'granted') {
                Notification.requestPermission(function (status) {
                    if (status == "granted") {
                        navigator.serviceWorker.getRegistration().then(function (reg) {
                            new Notification("Hello world");
                        });
                    }
                });
            }
        }
        else {
            throw "Browser not support Notification";
        }
    }
    //{
    //    body: "longht",
    //    icon: "https://eduso.vn/images/Logo.png",
    //    //image: "https://eduso.vn/images/Logo.png",
    //    badge: "badge",
    //    requireInteraction: requireInteraction,
    //}
    AlertMessage.prototype.Show = function (title, options, eventClick) {
        if (!options.icon) {
            options.icon = "https://eduso.vn/images/Logo.png";
        }
        options.badge = "https://eduso.vn/images/Logo.png";
        var noti = new Notification(title, options);
        if (eventClick) {
            noti.addEventListener("click", eventClick);
        }
        return noti;
    }
    var serviceWorkerRegister = function () {
        if (!'serviceWorker' in navigator) {
            throw new Error('No Service Worker support!');
        }
        if (!'PushManager' in window) {
            throw new Error('No Push API Support!');
        }
        navigator.serviceWorker.addEventListener('controllerchange', () => {
            // This fires when the service worker controlling this page
            // changes, eg a new worker has skipped waiting and become
            // the new active worker.
            console.log("controllerchange");
        });

        navigator.serviceWorker.addEventListener('message', event => {
            // event is a MessageEvent object
            console.log(`The service worker sent me a message: ${event.data}`);
        });
        navigator.serviceWorker.register(__SCRIPT_URL).then(function (reg) {
            reg.installing; // the installing worker, or undefined
            reg.waiting; // the waiting worker, or undefined
            reg.active; // the active worker, or undefined

            reg.addEventListener('updatefound', () => {
                // A wild service worker has appeared in reg.installing!
                const newWorker = reg.installing;

                newWorker.state;
                // "installing" - the install event has fired, but not yet complete
                // "installed"  - install complete
                // "activating" - the activate event has fired, but not yet complete
                // "activated"  - fully active
                // "redundant"  - discarded. Either failed install, or it's been
                //                replaced by a newer version

                newWorker.addEventListener('statechange', () => {
                    // newWorker.state has changed
                    console.log(newWorker.state);
                });
            });

            navigator.serviceWorker.ready.then(function (swRegistration) {
                swRegistration.active.postMessage("Hi service worker");
                return swRegistration.sync.register('myFirstSync');
            });
        });
    }
    AlertMessage.prototype.RegisServiceWorker = serviceWorkerRegister;
    return AlertMessage();
}());

var alertMessage = new AlertMessage('/sw.js');
alertMessage.RegisServiceWorker();
