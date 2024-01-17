using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Helpers;
using Web.Core;
using Web.Extensions;
using Web.Model;
using Web.Repository; 

namespace Web.Areas.Admin.Controllers
{ 
    [Authorize]
    public class AdminMenuController : ModuleController
    {
        private readonly IAdminMenuRepository _adminMenuRepository;
        private readonly IGroupUserRepository _groupUserRepository;

        public AdminMenuController(
            IAdminMenuRepository adminMenuRepository, 
            IGroupUserRepository groupUserRepository)
        {
            _adminMenuRepository = adminMenuRepository;
            _groupUserRepository = groupUserRepository;
        } 


        [Authorize(Roles = "AdminMenu.View,Admin")]
        public IActionResult Index()
        { 
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return View();
        }


        [Authorize(Roles = "AdminMenu.View,Admin")]
        [HttpGet]
        public async Task<IActionResult> ListData(int pageIndex, int pageSize, string keySearch)
        {
            var model = _adminMenuRepository.GetPageAdminMenu(pageIndex, pageSize, keySearch, out int total);
            int offset = (pageIndex - 1) * pageSize;
            double pageTotal = Math.Ceiling((double)total / pageSize);
            var adminMenus = new List<AdminMenu>();
            var lstParents = model.Where(g => g.ParentID == 0).OrderBy(g => g.ThuTu).ToList();
            if (lstParents.Count > 0)
            {
                foreach (var tblAdminMenu in lstParents)
                {
                    adminMenus.Add(tblAdminMenu);
                    var lstChild = model.Where(g => g.ParentID == tblAdminMenu.ID).OrderBy(g => g.ThuTu).ToList();
                    if (lstChild.Count > 0)
                    {
                        adminMenus.AddRange(lstChild);
                    }
                }
                adminMenus = Common.CreateLevel(adminMenus);
            }
            else
            {
                adminMenus.AddRange(model);
            }
            ViewBag.STT = offset;
            ViewBag.IsAdmin = LoginProfile.IsAdmin;
            ViewBag.Roles = UserRoles;
            return Json(new
            {
                viewContent = await ControllerExtensions.RenderViewAsync(this, "_ListData", adminMenus),
                totalPages = pageTotal
            });
        }


        [Authorize(Roles = "AdminMenu.Add,Admin")]
        public IActionResult Add()
        {
            ViewBag.AdminMenu = _adminMenuRepository.GetAllActive().ToList();
            return View();
        }


        [Authorize(Roles = "AdminMenu.Add,Admin")]
        [HttpPost]
        public IActionResult Add(AdminMenu model)
        {
            try
            {
                var lstMenuAdmin = _adminMenuRepository.GetAllActive();
                var menu = lstMenuAdmin.Any(x => x.Name.Equals(model.Name));
                if (menu)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên menu đã tồn tại"
                    });
                }
                string controller = "";
                if (!string.IsNullOrEmpty(model.Url) && model.Url.Contains("/"))
                {
                    var arrUrl = model.Url.Split('/');
                    controller = arrUrl[2];
                    model.Controller = controller;
                }
                model.NgayTao = DateTime.Now;
                var result = _adminMenuRepository.Add(model);
                if (result)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Thêm mới thành công"
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Thêm mới thất bại"
                    });
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Thêm mới thất bại " + ex
                });
            }
        }


        [Authorize(Roles = "AdminMenu.Edit,Admin")]
        public IActionResult Edit(int id)
        {
            ViewBag.AdminMenu = _adminMenuRepository.GetAllActive().ToList();
            var menu = _adminMenuRepository.Find(id);
            return View(menu);
        }


        [Authorize(Roles = "AdminMenu.Edit,Admin")]
        [HttpPost]
        public IActionResult Edit(AdminMenu model)
        {
            try
            {
                var lstMenuAdmin = _adminMenuRepository.GetAllActive(); 
                var menu = lstMenuAdmin.Any(x => x.Name.Equals(model.Name) && x.ID != model.ID);
                if (menu)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Tên menu đã tồn tại"
                    });
                }
                string controller = "";
                if (!string.IsNullOrEmpty(model.Url) && model.Url.Contains("/"))
                {
                    var arrUrl = model.Url.Split('/');
                    controller = arrUrl[2];
                    model.Controller = controller;
                }
                var result = _adminMenuRepository.Edit(model);
                if (result)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Messenger = "Cập nhật thành công"
                    });
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Messenger = "Cập nhật thất bại "
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Cập nhật thất bại " + ex
                });
            }
        }


        [Authorize(Roles = "AdminMenu.Delete,Admin")]
        public IActionResult Delete(int id)
        { 
            var result = _adminMenuRepository.Delete(id);
            if (result)
            {
                return Json(new
                {
                    IsSuccess = true,
                    Messenger = "Xóa bản ghi thành công"
                });
            }
            else
            {
                return Json(new
                {
                    IsSuccess = false,
                    Messenger = "Xóa bản ghi thất bại"
                });
            }
        }
         

        public ActionResult MenuLeft(string url)
        {  
            ViewBag.Url = url != null ? url.ToLower() : string.Empty; 
            var lstMenuAdmin = new List<AdminMenu>();
            if(LoginProfile != null)
            {
                lstMenuAdmin = _adminMenuRepository.GetAllActive().ToList();
                if (LoginProfile.IsAdmin)
                {
                    return Json(new
                    {
                        viewContent = ControllerExtensions.RenderViewAsync(this, "_MenuLeft", lstMenuAdmin),
                    });
                }
                else
                {
                    var lstId = _adminMenuRepository.GetMenuIdUserId(UserID);
                    var lstChucNangId = lstId != null ? lstId.ToList() : new List<int>();
                    if (lstChucNangId.Count > 0)
                    {
                        lstMenuAdmin = lstMenuAdmin.Where(g => lstChucNangId.Contains(g.ID)).ToList();
                    }
                }
            }  
           
            return Json(new
            {
                viewContent = ControllerExtensions.RenderViewAsync(this, "_MenuLeft", lstMenuAdmin),
            });
        }
    }
}
