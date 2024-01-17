using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model;

namespace Web.Repository
{
    public interface IUserAdminRepository
    {
        IEnumerable<tbl_User> GetPageUser(int pageIndex, int pageSize, string keySearch, out int total);
        IEnumerable<tbl_User> GetAll();
        IEnumerable<tbl_User> SearchName(string name);
        tbl_User Find(int id);
        bool Add(tbl_User obj);
        bool AddToGroupUser(tbl_User model);
        bool Edit(tbl_User obj);
        bool ChangeStatus(int id, bool active);
        bool ChangePassword(int id, string password);
        void Delete(int id);
        tbl_User Login(string username, string password);
        Task<bool> CheckExistEmail(string email);
        Task<bool> CheckExistUserName(string userName);
        bool CheckPermission(string controller, string acction);
    }
}
