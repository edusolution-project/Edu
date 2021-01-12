self.importScripts('/scripts/push-notifications-controller.js');

var pushNotificationTitle = 'EnglishPlatform';
var CACHE_NAME = 'eduso-cache-v1';
self.addEventListener('install', function (event) {
    //console.log(event.target.location.search);
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(function (cache) {
                return cache.addAll([event.target.location.search]);
            })
    );
})

self.addEventListener('push', function (event) {
    var dataObj;
    try {
        var data = event.data;
        var text = data.text();
        dataObj = JSON.parse(text);
    }
    catch {

    }
    if (dataObj) {
        var text, url;
        text = dataObj.content;
        url = dataObj.url;
        self.registration.showNotification(pushNotificationTitle, {
            body: text,
            data: url,
            icon: '/images/push-notification-icon.png'
        });
    }
    else {
        self.registration.showNotification(pushNotificationTitle, {
            body: event.data.text(),
            icon: '/images/push-notification-icon.png'
        });
    }
});

self.addEventListener('pushsubscriptionchange', function (event) {
    var handlePushSubscriptionChangePromise = Promise.resolve();

    if (event.oldSubscription) {
        handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {
            return PushNotificationsController.discardPushSubscription(event.oldSubscription);
        });
    }

    if (event.newSubscription) {
        handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {
            return PushNotificationsController.storePushSubscription(event.newSubscription);
        });
    }

    if (!event.newSubscription) {
        handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {
            return PushNotificationsController.retrievePublicKey().then(function (applicationServerPublicKey) {
                return pushServiceWorkerRegistration.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: applicationServerPublicKey
                }).then(function (pushSubscription) {
                    return PushNotificationsController.storePushSubscription(pushSubscription);
                });
            });
        });
    }

    event.waitUntil(handlePushSubscriptionChangePromise);
});

self.addEventListener('notificationclick', function (event) {
    var url = event.notification.data;
    event.waitUntil(clients.matchAll({ type: "window" })
        .then(function (clientList) {
            for (var i = 0; i < clientList.length; i++) {
                var client = clientList[i];
                if (client.url == self.registration.scope && 'focus' in client) {
                    return client.focus();
                }
            }
            if (clients.openWindow) {
                return clients.openWindow(url);
            }
        })
    );
    event.notification.close();
    //window.open(event.notification.url, '_blank');
});