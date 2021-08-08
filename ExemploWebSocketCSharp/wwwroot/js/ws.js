
/**
* @param {string} url 
* @param {{ 
*      onConnected: (ev: Event, socket: WebSocket) => void,
*      onDisconnected: (ev: CloseEvent) => void, 
*      onReconnected: (ev: Event, socket: WebSocket) => void,
*      onMessage: (ev: Event) => void, 
*      onError: (ev: Event) => void,
*      autoReconnectTime?: number = 2000,
*  }} events 
*/
function startWebSocket(url, events) {
    var socket = new WebSocket(url);

    socket.onopen = (e) => {
        if (typeof(events.onConnected) == 'function') {
            events.onConnected(e, socket);
        }

        if (events._reconnecting && (typeof(events.onReconnected) == 'function')) {
            events.onReconnected(e, socket);
        }
    };

    socket.onclose = (e) => {
        if (typeof(events.onDisconnected) == 'function') {
            events.onDisconnected(e);
        }

        events.autoReconnectTime ??= 100;

        if (events.autoReconnectTime > 0) {
            setTimeout(() => {
                events._reconnecting = true;
                startWebSocket(url, events);
            }, events.autoReconnectTime);
        }
    };

    socket.onmessage = events.onMessage;
    socket.onerror = events.onError;
}
