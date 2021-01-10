self.importScripts('/scripts/push-notifications-controller.js');

var pushNotificationTitle = 'EnglishPlatform';
var CACHE_NAME = 'eduso-cache-v1';
self.addEventListener('install', function (event) {
    //console.log(event.target.location.search);
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(function (cache) {
                console.log('Opened cache');
                return cache.addAll([event.target.location.search]);
            })
    );
})

self.addEventListener('push', function (event) {
    console.log(event);
    event.waitUntil(self.registration.showNotification(pushNotificationTitle, {
        body: event.data.text(),
        icon: '/images/push-notification-icon.png'
    }));
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
    event.notification.close();
});