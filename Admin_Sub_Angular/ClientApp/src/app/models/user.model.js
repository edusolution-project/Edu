"use strict";
// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================
Object.defineProperty(exports, "__esModule", { value: true });
var User = /** @class */ (function () {
    // Note: Using only optional constructor properties without backing store disables typescript's type checking for the type
    function User(id, userName, fullName, email, jobTitle, phoneNumber, roles) {
        this.id = id;
        this.userName = userName;
        this.fullName = fullName;
        this.email = email;
        this.jobTitle = jobTitle;
        this.phoneNumber = phoneNumber;
        this.roles = roles;
    }
    Object.defineProperty(User.prototype, "friendlyName", {
        get: function () {
            var name = this.fullName || this.userName;
            if (this.jobTitle)
                name = this.jobTitle + " " + name;
            return name;
        },
        enumerable: true,
        configurable: true
    });
    return User;
}());
exports.User = User;
//# sourceMappingURL=user.model.js.map