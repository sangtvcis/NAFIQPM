using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Model; 

namespace Web.Repository
{
    public interface IAdminMenuRepository
    {
        IEnumerable<AdminMenu> GetPageAdminMenu(int pageIndex, int pageSize, string keySearch, out int total);
        IEnumerable<AdminMenu> GetAllActive();
        Roles GetRoleByMenuId(int menuid);
        AdminMenu Find(int id);
        AdminMenu GetByName(string name);
        IEnumerable<int> GetMenuIdUserId(int userId);
        IEnumerable<Permissions> GetRoleByUserId(int userId);
        bool Add(AdminMenu obj);
        bool Edit(AdminMenu obj);
        bool Delete(int id);
    }
}
