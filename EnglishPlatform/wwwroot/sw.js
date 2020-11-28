var __PUBLIC_KEY = "BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk";
var __PRIVATE_KEY = "ERIZmc5T5uWGeRxedxu92k3HnpVwy_RCnQfgek1x2Y4";
var __SERVER_URL = "https://localhost/api/EasyChat/GetClassList";
var urlB64ToUint8Array = base64String => {
    var padding = "=".repeat((4 - (base64String.length % 4)) % 4);
    var base64 = (base64String + padding)
        .replace(/\-/g, "+")
        .replace(/_/g, "/");
    var rawData = atob(base64);
    var outputArray = new Uint8Array(rawData.length);
    for (var i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
};

self.addEventListener("install", function (e) {
    console.log("install");
});

self.addEventListener("activate", function (e) {
    console.log("activate");
});

var saveSubscription = function (subscription) {
    return fetch(__SERVER_URL, {
        method: 'post',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(subscription),
    });
}

self.addEventListener("fetch", function (e) {
    console.log("fetch");
});
self.addEventListener('push', function (event) {
    if (event.data) {
        console.log('Push event!! ', event.data.text());
        self.registration.showNotification(event.data.text(), { requireInteraction: false });
    } else {
        console.log('Push event but no data');
        self.registration.showNotification('Push event but no data', { requireInteraction: false });
    }
});
self.addEventListener('sync', function (event) {
    if (event.tag == 'myFirstSync') {
        event.waitUntil(function (e) {
            console.log("syncying", e);
        });
    }
});

 //var applicationServerKey = urlB64ToUint8Array(__PUBLIC_KEY);
//var options = { applicationServerKey, userVisibleOnly: true };
//var pub = self.registration.pushManager;
//pub.subscribe(options).then(function (pushSubscription) {
//    saveSubscription(pushSubscription).then(function (response) {
//        console.log(response)
//        self.registration.showNotification(response.url, { requireInteraction: false });
//    }).catch(function (err) {
//        console.log('Error', err)
//    })
//});