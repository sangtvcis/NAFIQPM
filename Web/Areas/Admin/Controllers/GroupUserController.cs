using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Helpers;
using Web.Core;
using Web.Extensions;
using Web.Model;
using Web.Model.Enum;
using Web.Repository; 

namespace Web.Areas.Admin.Controllers
{ 
    public class GroupUserController : ModuleController
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IAdminMenuRepository _adminMenuRepository;
        private readonly IUserAdminRepository _userRepository;

        public GroupUserController(
            IGroupUserRepository groupUserRepository,
            IAdminMenuRepository adminMenuRepository,
            IUserAdminRepository userRepository)
        {
            _groupUserRepository = groupUserRepository;
            _adminMenuRepository = adminMenuRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "GroupUser.View,Admin")]
        public IActionResult Index()
        {
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return View();
        }


        [Authorize(Roles = "GroupUser.View,Admin")]
        [HttpPost]
        public async Task<IActionResult> ListData(int page)
        {
            string keySearch = "";
            int pageSize = 20;
            var model = _groupUserRepository.GetPageGroupUser(page, pageSize, keySearch, out int total);
            int offset = (page - 1) * pageSize;
            double pageTotal = Math.Ceiling((double)total / pageSize);
            
            ViewBag.STT = offset;
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return Json(new
            {
                viewContent = await ControllerExtensions.RenderViewAsync(this, "_ListData", model),
                totalPages = pageTotal
            });
        }


        [Authorize(Roles = "GroupUser.Add,Admin")]
        public ActionResult Add()
        {
            return View();
        }


        [Authorize(Roles = "GroupUser.Add,Admin")]
        [HttpPost] 
        public ActionResult Add(GroupUser obj)
        {
            try
            {
                _groupUserRepository.Add(obj);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Thêm mới nhóm thành công",
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Thêm mới nhóm thất bại")
                });
            }
        }


        [Authorize(Roles = "GroupUser.Edit,Admin")]
        public ActionResult Edit(int id)
        {
            var objGroupUser = _groupUserRepository.Find(id);
            return View(objGroupUser);
        }


        [Authorize(Roles = "GroupUser.Edit,Admin")]
        [HttpPost]
        public ActionResult Edit(GroupUser obj)
        {
            try
            {
                var objOld = _groupUserRepository.Find(obj.ID);
                objOld.Name = obj.Name;
                objOld.Status = obj.Status;
                _groupUserRepository.Edit(objOld);
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Cập nhật danh mục thành công",
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Cập nhật danh mục thất bại",
                });
            } 
        }


        [Authorize(Roles = "GroupUser.Delete,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var obj = _groupUserRepository.Find(id);
                _groupUserRepository.Delete(id); 
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = string.Format("Xóa danh mục thất bại")
                });
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Xóa danh mục thành công",
            });
        }


        [Authorize(Roles = "GroupUser.Delete,Admin")]
        [HttpPost]
        public ActionResult DeleteAll(string lstid)
        {
            var arrid = lstid.Split(',');
            var count = 0;
            foreach (var item in arrid)
            {
                try
                {
                    _groupUserRepository.Delete(Convert.ToInt32(item));
                    count++;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return Json(new
            {
                Messenger = string.Format("Xóa thành công {0} danh mục", count),
            });
        }


        [Authorize(Roles = "GroupUser.Edit,Admin")]
        public ActionResult Permission(int id)
        {
            var lstOrder = new List<AdminMenu>();
            var lstAdminMenu = _adminMenuRepository.GetAllActive().ToList();
            var lstParents = lstAdminMenu.Where(g => g.ParentID == 0).OrderBy(g => g.ThuTu).ToList();
            foreach (var tblAdminMenu in lstParents)
            {
                lstOrder.Add(tblAdminMenu);
                var lstChild = lstAdminMenu.Where(g => g.ParentID == tblAdminMenu.ID).OrderBy(g => g.ThuTu).ToList();
                if (lstChild.Count > 0)
                {
                    lstOrder.AddRange(lstChild);
                }
            }
            var permissions = _groupUserRepository.GetMenuByGroupId(id);
            lstOrder = Common.CreateLevel(lstOrder);
            ViewBag.GroupUserID = id;
            ViewBag.Permissions = permissions;
            return View(lstOrder);
        }


        [HttpPost]
        [Authorize(Roles = "GroupUser.Edit,Admin")]
        public ActionResult Permission(int groupuserId, string roles)
        {
            try
            {
                var arrRoles = roles.Split(':');
                if (arrRoles != null)
                {
                    int menuId = Convert.ToInt32(arrRoles[0]);
                    var role = _adminMenuRepository.GetRoleByMenuId(menuId);
                    int roleId = role != null ? role.ID : 0;

                    if (arrRoles.Length < 2)
                    {
                        _groupUserRepository.DeletePermission(groupuserId, roleId);
                        return Json(new
                        {
                            IsSuccess = true,
                            Messenger = "Phân quyền cho nhóm thành công"
                        });
                    }
                    else
                    {
                        var permissions = _groupUserRepository.GetPermissionByGroupId(groupuserId);
                        List<Permissions> lstPermissions = permissions != null ? permissions.ToList() : new List<Permissions>();
                        string[] arrAction = arrRoles[1].Split(','); 
                        for (int i = 0; i < arrAction.Length; i++)
                        {
                            EnumHelper.Action action = (EnumHelper.Action)Enum.Parse(typeof(EnumHelper.Action), arrAction[i]); 
                            lstPermissions.Add(new Permissions
                            {
                                GroupID = groupuserId,
                                RoleID = roleId,
                                Action = action.ToString()
                            });
                        }
                        _groupUserRepository.ChangePermission(groupuserId, lstPermissions);
                    }
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Phân quyền cho nhóm thành công"
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Phân quyền thất bại"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Phân quyền thất bại"
                });
            }
        }


        [Authorize(Roles = "GroupUser.Add,Admin")]
        public ActionResult AddUserToGroup(int id)
        {
            //Lấy thông tin nhóm theo id 
            var objGroupUser = _groupUserRepository.Find(id); 
            //Lấy những user đã được thêm vào nhóm
            var lstUserAdded = _groupUserRepository.GetUserInGroup(id);

            //Danh sách user chưa được thêm vào nhóm
            var lstUserUnAdd = _groupUserRepository.GetUserNotInGroup(id);
            ViewBag.LstUserAdded = lstUserAdded;
            ViewBag.LstUserUnAdd = lstUserUnAdd; 
            return View(objGroupUser);
        }


        [Authorize(Roles = "GroupUser.Add,Admin")]
        [HttpPost]
        public ActionResult AddUserToGroup(int[] to, int id)
        {
            //Thêm danh sách user vào nhóm
            if (to != null)
            {
                try
                {
                    _groupUserRepository.AddUserToGroup(to, id);
                }
                catch (Exception)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = string.Format("Có lỗi xảy ra")
                    });
                }
            }
            return Json(new
            {
                IsSuccess = true,
                Messenger = "Thêm người dùng vào nhóm thành công"
            });
        }
    }
}
