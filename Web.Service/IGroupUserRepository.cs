using System;
using Web.Model; 

namespace Web.Repository
{
    public interface IGroupUserRepository
    {
        IEnumerable<GroupUser> GetPageGroupUser(int pageIndex, int pageSize, string keySearch, out int total);
        IEnumerable<GroupUser> GetAll();
        IEnumerable<tbl_User> GetUserInGroup(int id);
        IEnumerable<tbl_User> GetUserNotInGroup(int id);
        GroupUser Find(int id);
        void Add(GroupUser obj);
        void Edit(GroupUser obj);
        void AddUserToGroup(int[] to, int id); 
        void ChangePermission(int groupId, List<Permissions> permissions);
        void Delete(int id);
        void DeletePermission(int groupid, int roleid);
        IEnumerable<Permissions> GetMenuByGroupId(int id);
        IEnumerable<Permissions> GetPermissionByGroupId(int id);
    }
}
