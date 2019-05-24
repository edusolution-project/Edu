"use strict";
// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var user_model_1 = require("./user.model");
var UserEdit = /** @class */ (function (_super) {
    __extends(UserEdit, _super);
    function UserEdit(currentPassword, newPassword, confirmPassword) {
        var _this = _super.call(this) || this;
        _this.currentPassword = currentPassword;
        _this.newPassword = newPassword;
        _this.confirmPassword = confirmPassword;
        return _this;
    }
    return UserEdit;
}(user_model_1.User));
exports.UserEdit = UserEdit;
//# sourceMappingURL=user-edit.model.js.map