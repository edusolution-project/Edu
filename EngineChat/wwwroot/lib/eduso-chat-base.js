import { ChatProvider } from './chat-provider';
import { Requester } from './request';
var EdusoChatBase = /** @class */ (function () {
    function EdusoChatBase(config) {
        this._config = config;
        var header = new Headers();
        header.append("security", this._config.current.id || "");
        this._requester = new Requester(header);
        this._chatProvider = new ChatProvider(this._config.url, this._config.current, this._requester);
    }
    EdusoChatBase.prototype.getConfig = function () {
        return this._config;
    };
    EdusoChatBase.prototype.getRequest = function () {
        return this._requester;
    };
    EdusoChatBase.prototype.getChatProvider = function () {
        return this._chatProvider;
    };
    return EdusoChatBase;
}());
export { EdusoChatBase };
