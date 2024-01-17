using Dapper; 
using System.Data; 
using System.Text; 
using Web.Model;
using Web.Model.Common;

namespace Web.Repository.Implement
{
    public class UserAdminRepository : IUserAdminRepository
    {
        private readonly DapperContext _context;
        private readonly ICacheRepository _cache;
        public UserAdminRepository(
            DapperContext context,
            ICacheRepository cache)
        {
            _context = context;
            _cache = cache;
        }
        public bool Add(tbl_User model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("UserName", model.UserName);
                    parameters.Add("Image", model.Image);
                    parameters.Add("Email", model.Email);
                    parameters.Add("FullName", model.FullName);
                    parameters.Add("Address", model.Address);
                    parameters.Add("Active", model.Active);
                    parameters.Add("Password", model.Password);
                    parameters.Add("Phone", model.Phone);
                    parameters.Add("Description", model.Description);
                    parameters.Add("GroupUserID", model.GroupUserID);
                    parameters.Add("isAdmin", model.isAdmin);
                    parameters.Add("Gender", model.Gender);
                    parameters.Add("ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    connection.Execute("UserAdmin_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    int newId = parameters.Get<int>("ID");

                    if (!string.IsNullOrEmpty(model.GroupUserID))
                    {
                        DynamicParameters pGroup = new DynamicParameters();
                        pGroup.Add("UserID", newId);
                        pGroup.Add("GroupID", model.GroupUserID);
                        connection.Execute("User_Group_Insert",
                            pGroup,
                            commandType: CommandType.StoredProcedure);
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

        public bool Edit(tbl_User model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("UserName", model.UserName);
                    parameters.Add("Image", model.Image);
                    parameters.Add("Email", model.Email);
                    parameters.Add("FullName", model.FullName);
                    parameters.Add("Address", model.Address);
                    parameters.Add("Active", model.Active);
                    parameters.Add("Phone", model.Phone);
                    parameters.Add("Description", model.Description);
                    parameters.Add("GroupUserID", model.GroupUserID);
                    parameters.Add("isAdmin", model.isAdmin);
                    parameters.Add("Gender", model.Gender);
                    parameters.Add("ID", model.ID);
                    connection.Execute("UserAdmin_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    if (!string.IsNullOrEmpty(model.GroupUserID))
                    {
                        DynamicParameters pGroup = new DynamicParameters();
                        pGroup.Add("UserID", model.ID);
                        pGroup.Add("GroupID", model.GroupUserID);
                        connection.Execute("User_Group_Insert",
                            pGroup,
                            commandType: CommandType.StoredProcedure);
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


        public bool AddToGroupUser(tbl_User model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("GroupUserID", model.GroupUserID);
                    parameters.Add("ID", model.ID);
                    connection.Execute("UserAdmin_AddGroupUser",
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

        public void Delete(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string sql1 = $"DELETE UserAdmin WHERE ID =@ID";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@ID", id);
                    connection.ExecuteAsync(sql1,
                        parameters,
                        commandType: CommandType.Text);
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ChangeStatus(int id, bool active)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                string sql = "UPDATE UserAdmin SET Active=@Active WHERE ID = @ID";
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Active", active);
                    parameters.Add("@ID", id);
                    connection.Execute(sql, parameters);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangePassword(int id, string password)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                string sql = "UPDATE UserAdmin SET Password=@Password WHERE ID = @ID";
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Password", password);
                    parameters.Add("@ID", id);
                    connection.Execute(sql, parameters);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public tbl_User Find(int id)
        {
            string sql = $"SELECT * FROM UserAdmin WHERE ID =@ID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            using (var connection = _context.CreateConnection())
            {
                return connection.QueryFirstOrDefault<tbl_User>(sql, parameters);
            }
        }

        public IEnumerable<tbl_User> GetAll()
        {
            var data = _cache.Get<IEnumerable<tbl_User>>(nameof(tbl_User));

            if (data == null)
            {
                string sql = $"SELECT * FROM UserAdmin WHERE Active =1";
                using (var connection = _context.CreateConnection())
                {
                    IEnumerable<tbl_User> lst = connection.Query<tbl_User>(sql);
                    _cache.Set<IEnumerable<tbl_User>>(lst, nameof(tbl_User));
                    return lst;
                }
            }
            else
                return data;

        }

        public IEnumerable<tbl_User> SearchName(string name)
        {
            string key = string.IsNullOrEmpty(name) ? "UserAdmin" : name;
            var data = _cache.Get<IEnumerable<tbl_User>>(key);

            if (data == null)
            {
                using (var connection = _context.CreateConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("Name", name);
                    IEnumerable<tbl_User> lst = connection.Query<tbl_User>("UserAdmin_SearchByName",
                        parameters,
                        commandType: CommandType.StoredProcedure
                        );
                    _cache.Set<IEnumerable<tbl_User>>(lst, key);
                    return lst;
                }
            }
            else
            {
                return data;
            }
        }

        public tbl_User Login(string username, string password)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserName", username);
                parameters.Add("@Password", password);
                return connection.QueryFirstOrDefault<tbl_User>("UserAdmin_Login", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<tbl_User> GetPageUser(int pageIndex, int pageSize, string keySearch, out int total)
        {
            total = 0;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("KeyWord", keySearch != null ? keySearch.Trim() : string.Empty);
            parameters.Add("PageIndex", pageIndex);
            parameters.Add("PageSize", pageSize);

            using (var connection = _context.CreateConnection())
            {
                var lst = connection.Query<tbl_User>("UserAdmin_GetPage", parameters, commandType: CommandType.StoredProcedure);
                total = lst.FirstOrDefault() != null ? lst.FirstOrDefault().TotalCount : 0;
                return lst;
            }
        }

        public async Task<bool> CheckExistEmail(string email)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Email", email);
                return await connection.QueryFirstOrDefaultAsync<bool>("UserAdmin_CheckExistEmail", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> CheckExistUserName(string userName)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserName", userName);
                return await connection.QueryFirstOrDefaultAsync<bool>("UserAdmin_CheckExistUserName", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public bool CheckPermission(string controller, string acction)
        {
            return true;
        }
    }
}
