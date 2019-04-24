self.addEventListener('install', function (event) {
    console.log('V1 installing…');
    // cache a cat SVG
    var cache = caches.open("")
    event.waitUntil(caches.open("static-data"));
});

self.addEventListener('activate', function (event) {
    console.log('V1 now ready to handle fetches!', event);
});
self.addEventListener('fetch', function (event){
    const url = new URL(event.request.url);
    // serve the cat SVG from the cache if the request is
    // same-origin and the path is '/dog.svg'
    if (url.origin == location.origin && url.pathname == '/dog.svg') {
        event.respondWith(caches.match('/cat.svg'));
    }
});

self.addEventListener('push', function (event) {
    console.log('Received a push message', event);

    var title = 'Yay a message.';
    var body = 'We have received a push message.';
    var icon = '/images/icon-192x192.png';
    var tag = 'simple-push-demo-notification-tag';

    event.waitUntil(
        self.registration.showNotification(title, {
            body: body,
            icon: icon,
            tag: tag
        })
    );
});

self.addEventListener('notificationclick', function (event) {
    console.log('On notification click: ', event.notification.tag);
    // Android doesn’t close the notification when you click on it
    // See: http://crbug.com/463146
    event.notification.close();

    // This looks to see if the current is already open and
    // focuses if it is
    event.waitUntil(clients.matchAll({
        type: 'window'
    }).then(function (clientList) {
        for (var i = 0; i < clientList.length; i++) {
            var client = clientList[i];
            if (client.url === '/' && 'focus' in client) {
                return client.focus();
            }
        }
        if (clients.openWindow) {
            return clients.openWindow('/');
        }
    }));
});

