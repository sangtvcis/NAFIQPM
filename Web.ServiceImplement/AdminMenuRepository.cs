using System.Data; 
using Web.Model.Common;
using Web.Model;
using Dapper;
using System.Text; 

namespace Web.Repository.Implement
{
    public class AdminMenuRepository : IAdminMenuRepository
    {
        private readonly DapperContext _context;
        private readonly ICacheRepository _cache;
        public AdminMenuRepository(DapperContext context, ICacheRepository cache)
        {
            _context = context;
            _cache = cache;
        }

        public bool Add(AdminMenu model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("Name", model.Name);
                        parameters.Add("ParentID", model.ParentID);
                        parameters.Add("Url", model.Url);
                        parameters.Add("ThuTu", model.ThuTu);
                        parameters.Add("Active", model.Active);
                        parameters.Add("Icon", model.Icon);
                        parameters.Add("Controller", model.Controller);
                        parameters.Add("NguoiTao", model.NguoiTao);
                        parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute("AdminMenu_Insert",
                            parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);
                        int newId = parameters.Get<int>("ID");

                        string sql = $"SELECT * FROM Roles WHERE MenuID = @MenuID";
                        DynamicParameters param = new DynamicParameters();
                        param.Add("MenuID", model.Controller);
                        var role = connection.QueryFirstOrDefault<Roles>(sql, param, transaction: tran);

                        if (role == null)
                        {
                            DynamicParameters paramRole = new DynamicParameters();
                            paramRole.Add("KeyRole", model.Controller);
                            paramRole.Add("MenuID", newId);
                            paramRole.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                            connection.Execute("Roles_Insert",
                                paramRole,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }

                        tran.Commit();
                    }
                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Edit(AdminMenu model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", model.Name);
                        parameters.Add("ParentID", model.ParentID);
                        parameters.Add("Url", model.Url);
                        parameters.Add("ThuTu", model.ThuTu);
                        parameters.Add("Active", model.Active);
                        parameters.Add("Controller", model.Controller);
                        parameters.Add("Icon", model.Icon);
                        parameters.Add("NguoiSua", model.NguoiSua);
                        parameters.Add("ID", model.ID);
                        connection.Execute("AdminMenu_Update",
                            parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: tran);

                        string sql = $"SELECT * FROM Roles WHERE MenuID = @MenuID";
                        DynamicParameters param = new DynamicParameters();
                        param.Add("MenuID", model.ID);

                        var role = connection.QueryFirstOrDefault<Roles>(sql, param, transaction: tran);

                        if (role == null)
                        {
                            DynamicParameters paramRole = new DynamicParameters();
                            paramRole.Add("KeyRole", model.Controller);
                            paramRole.Add("MenuID", model.ID);
                            paramRole.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                            connection.Execute("Roles_Insert",
                                paramRole,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }
                        else
                        {
                            DynamicParameters paramRole = new DynamicParameters();
                            paramRole.Add("KeyRole", model.Controller);
                            paramRole.Add("MenuID", model.ID);
                            connection.Execute("Roles_Update",
                                paramRole,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }

                        tran.Commit();
                    }
                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool Delete(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    connection.Execute(
                        "AdminMenu_Delete",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public AdminMenu Find(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("id", id);
                return connection.QueryFirstOrDefault<AdminMenu>(
                    "AdminMenu_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }


        public AdminMenu GetByName(string name)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Name", name);
                return connection.QueryFirstOrDefault<AdminMenu>("AdminMenu_GetByName",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public Roles GetRoleByMenuId(int menuid)
        {
            string sql = $"SELECT * FROM Roles WHERE MenuID = {menuid}";
            using (var connection = _context.CreateConnection())
            {
                return connection.QueryFirstOrDefault<Roles>(sql);
            }
        }

        public IEnumerable<AdminMenu> GetAllActive()
        {
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<AdminMenu>("AdminMenu_GetAllActive", commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<int> GetMenuIdUserId(int userId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT DISTINCT r.MenuID FROM Permission p JOIN Roles r ON p.RoleID = r.ID");
            stringBuilder.Append(" WHERE p.GroupID IN (SELECT GroupID FROM User_Group WHERE UserID = @UserID)");
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserID", userId);
            string sql = stringBuilder.ToString();
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<int>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<Permissions> GetRoleByUserId(int userId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT r.KeyRole,p.Action FROM Permission p JOIN Roles r ON p.RoleID = r.ID");
            stringBuilder.Append(" WHERE p.GroupID IN (SELECT GroupID FROM User_Group WHERE UserID = @UserID)");
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserID", userId);
            string sql = stringBuilder.ToString();
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<Permissions>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<AdminMenu> GetPageAdminMenu(int pageIndex, int pageSize, string keySearch, out int total)
        {
            try
            {
                total = 0;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Keyword", keySearch != null ? keySearch.Trim() : string.Empty);
                parameters.Add("PageIndex", pageIndex);
                parameters.Add("PageSize", pageSize);

                using (var connection = _context.CreateConnection())
                {
                    var lst = connection.Query<AdminMenu>("AdminMenu_GetPage", parameters, commandType: CommandType.StoredProcedure);
                    total = lst.FirstOrDefault() != null ? lst.FirstOrDefault().TotalCount : 0;
                    return lst;
                }
            }
            catch (Exception)
            { 
                throw;
            } 
        }
    }
}
