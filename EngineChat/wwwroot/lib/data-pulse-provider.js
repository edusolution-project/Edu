var DataPulseProvider = /** @class */ (function () {
    function DataPulseProvider() {
        /// danh sách thành viên của toàn bộ group 
        this._members = {};
        this._friends = {};
    }
    // chay khi add vào group
    DataPulseProvider.prototype.UpdateMember = function (idChat, member) {
        var listMembers = this._members[idChat];
        if (listMembers == undefined) {
            var data = [member];
            this._members[idChat] = data;
        }
    };
    //chay khi chấp thuận chat với người đó
    DataPulseProvider.prototype.UpdateFriend = function (friend) {
        var user = friend.id ? this._friends[friend.id] : undefined;
        // chưa tồn tại friend trong lists
        if (user == undefined && friend.id != undefined) {
            this._friends[friend.id] = friend;
        }
    };
    // lấy danh sách member để build giao diện
    DataPulseProvider.prototype.GetMembers = function (idChat) {
        return this._members[idChat];
    };
    // lấy danh sách bạn để buid giao diện
    DataPulseProvider.prototype.GetFriend = function (userId) {
        return this._friends[userId];
    };
    DataPulseProvider.prototype.GetFriends = function () {
        return this._friends;
    };
    return DataPulseProvider;
}());
export { DataPulseProvider };
