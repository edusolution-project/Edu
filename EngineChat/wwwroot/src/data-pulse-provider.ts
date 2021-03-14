import { IMember } from "./eduso-api";

// cung cap du lieu chung
interface IDataMembers{
    [guid:string]:IMember[]
}
interface IFriend{
    [guid:string]:IMember
}
export class DataPulseProvider{
    /// danh sách thành viên của toàn bộ group 
    private readonly _members : IDataMembers = {}
    private readonly _friends : IFriend = {}
    protected constructor(){

    }
    // chay khi add vào group
    public UpdateMember(idChat:string,member:IMember):void{
        var listMembers : IMember[] | undefined = this._members[idChat];
        if(listMembers == undefined){
            var data : IMember[] = [member];
            this._members[idChat] = data; 
        }
    }
    //chay khi chấp thuận chat với người đó
    public UpdateFriend(friend:IMember):void{
        var user : IMember | undefined = friend.id ? this._friends[friend.id] : undefined;
        // chưa tồn tại friend trong lists
        if(user == undefined && friend.id != undefined){
            this._friends[friend.id] = friend;
        }
    }
    // lấy danh sách member để build giao diện
    public GetMembers(idChat:string):IMember[] | undefined{
        return this._members[idChat]
    }
    // lấy danh sách bạn để buid giao diện
    public GetFriend(userId:string):IMember | undefined{
        return this._friends[userId]
    }
    public GetFriends():IFriend | undefined{
        return this._friends;
    }
}