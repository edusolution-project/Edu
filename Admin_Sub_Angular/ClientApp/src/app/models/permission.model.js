"use strict";
// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================
Object.defineProperty(exports, "__esModule", { value: true });
var Permission = /** @class */ (function () {
    function Permission(name, value, groupName, description) {
        this.name = name;
        this.value = value;
        this.groupName = groupName;
        this.description = description;
    }
    Permission.viewUsersPermission = "users.view";
    Permission.manageUsersPermission = "users.manage";
    Permission.viewRolesPermission = "roles.view";
    Permission.manageRolesPermission = "roles.manage";
    Permission.assignRolesPermission = "roles.assign";
    return Permission;
}());
exports.Permission = Permission;
//# sourceMappingURL=permission.model.js.map