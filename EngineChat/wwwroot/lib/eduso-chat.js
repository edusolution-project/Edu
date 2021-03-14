import { __extends } from "tslib";
import { EdusoChatBase } from "./eduso-chat-base";
var ChatEngine = /** @class */ (function (_super) {
    __extends(ChatEngine, _super);
    function ChatEngine(master, url, root) {
        var _this = this;
        var configuration = {
            current: master,
            debugger: false,
            url: url,
            element: root
        };
        _this = _super.call(this, configuration) || this;
        return _this;
    }
    return ChatEngine;
}(EdusoChatBase));
export { ChatEngine };
