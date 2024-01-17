using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using Web.Areas.Admin.Helpers;
using Web.Areas.Admin.Models;
using Web.BaseSecurity;
using Web.Core;
using Web.Extensions;
using Web.Model;
using Web.Model.Constanst;
using Web.Repository; 

namespace Web.Areas.Admin.Controllers
{ 
    public class AccountController : ModuleController
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IAdminMenuRepository _adminMenuRepository;
        private readonly IUserAdminRepository _userRepository;
        private SecurityManager securityManager = new SecurityManager();

        public AccountController(
            IGroupUserRepository groupUserRepository,
            IAdminMenuRepository adminMenuRepository,
            IUserAdminRepository userRepository)
        {
            _groupUserRepository = groupUserRepository;
            _adminMenuRepository = adminMenuRepository;
            _userRepository = userRepository;
        }


        [Authorize(Roles = "Account.View,Admin")]
        public IActionResult Index()
        {
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return View();
        }

         
        public async Task<IActionResult> ListData(int pageIndex, int pageSize, string keySearch)
        {
            var lstUser = _userRepository.GetPageUser(pageIndex, pageSize, keySearch, out int total);
            int offset = (pageIndex - 1) * pageSize;
            double pageTotal = Math.Ceiling((double)total / pageSize);
            ViewBag.STT = offset;
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return Json(new
            {
                viewContent = await ControllerExtensions.RenderViewAsync(this, "_ListData", lstUser),
                totalPages = pageTotal
            });
        }


        [Authorize(Roles = "Account.Add,Admin")]
        public ActionResult Add()
        { 
            ViewBag.GroupUser = _groupUserRepository.GetAll();
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Account.Add,Admin")]
        public ActionResult Add(tbl_User obj)
        {
            try
            { 
                var user = _userRepository.GetAll().Where(x=>x.UserName == obj.UserName.Trim());
                if (user.Any())
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên đăng nhập đã tồn tại"
                    });
                }
                obj.Password = HelperEncryptor.Md5Hash(obj.Password);
                _userRepository.Add(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới người dùng thành công",
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới người dùng thất bại")
                });
            }
        }


        [Authorize(Roles = "Account.Edit,Admin")]
        public ActionResult Edit(int id)
        {  
            var objUser = _userRepository.Find(id);
            ViewBag.GroupUser = _groupUserRepository.GetAll();
            return View(objUser);
        }

       
        [HttpPost]
        public ActionResult Edit(tbl_User obj)
        {
            try
            {
                _userRepository.Edit(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật người dùng thành công",
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Cập nhật người dùng thất bại"
                });
            }
        }


        [Authorize(Roles = "Delete.Edit,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _userRepository.Delete(id);
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Xóa danh người dùng thất bại"
                });
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa người dùng thành công",
            });
        }


        [Authorize(Roles = "Delete.Edit,Admin")]
        [HttpPost]
        public ActionResult DeleteAll(string lstid)
        {
            var arrid = lstid.Split(',');
            var count = 0;
            foreach (var item in arrid)
            {
                try
                {
                    _userRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} người dùng", count),
            });
        }

        public IActionResult Login()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Login(LoginModel obj)
        {
            if (string.IsNullOrEmpty(obj.UserName) || string.IsNullOrWhiteSpace(obj.Password))
            {

                return View();
            }
            var user = _userRepository.Login(obj.UserName, HelperEncryptor.Md5Hash(obj.Password));
            if (user != null)
            {
                AccountProfileModel accountProfile = new AccountProfileModel
                {
                    ID = user.ID,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    GroupUserID = user.GroupUserID,
                    IsAdmin = user.isAdmin
                };
                var user_info = JsonConvert.SerializeObject(accountProfile);
                var base64User = Convert.ToBase64String(Encoding.UTF8.GetBytes(user_info));
                CookieOptions option_user = new CookieOptions();
                option_user.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Response.Cookies.Append(Constants.USER_INFO, base64User, option_user);

                //HttpContext.Session.SetAccountProfile(accountProfile);

                List<string> roles = Roles(user.isAdmin, user.ID);
                roles.Add("Home"); 
                securityManager.SignIn(this.HttpContext, user.UserName, roles);

                var role_info = JsonConvert.SerializeObject(roles);
                var base64Role = Convert.ToBase64String(Encoding.UTF8.GetBytes(role_info));
                CookieOptions option_role = new CookieOptions();
                option_role.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Response.Cookies.Append(Constants.ROLES, base64Role, option_role);

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                TempData["Errors"] = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            } 
        }


        private List<string> Roles(bool isAdmin, int userId)
        {
            List<string> lstRoles = new List<string>(); 
            if (!isAdmin)
            {
                var lstPermission = _adminMenuRepository.GetRoleByUserId(userId);
                foreach (var item in lstPermission)
                {
                    if (!string.IsNullOrEmpty(item.KeyRole))
                    {
                        string roleName = item.KeyRole + "." + item.Action;
                        lstRoles.Add(roleName);
                    } 
                }  
            }
            else
            {
                lstRoles.Add("Admin");
            }
            return lstRoles;
        }

        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            securityManager.SignOut(this.HttpContext);
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
         
    }
}
