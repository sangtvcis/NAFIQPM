using Dapper;
using System.Data;
using System.Text;
using Web.Model;
using Web.Model.Common; 

namespace Web.Repository.Implement
{
    public class GroupUserRepository : IGroupUserRepository
    {
        private readonly DapperContext _context;
        public GroupUserRepository(DapperContext context)
        {
            _context = context;
        }

        public void Add(GroupUser model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@Name", model.Name);
                    parameters.Add("@Status", model.Status);
                    parameters.Add("@IsAdministrator", model.IsAdministrator);
                    parameters.Add("@NguoiTao", model.NguoiTao);
                    connection.Execute("GroupUser_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Edit(GroupUser model)
        {
            try
            {  
                using (var connection = _context.CreateConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@ID", model.ID);
                    parameters.Add("@Name", model.Name);
                    parameters.Add("@IsAdministrator", model.IsAdministrator);
                    parameters.Add("@NguoiSua", model.NguoiSua);
                    connection.Execute("GroupUser_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 

        public void AddUserToGroup(int[] to, int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        string sql = $"DELETE User_Group WHERE GroupID ={id}"; 
                        connection.Execute(sql, 
                            commandType: CommandType.Text,
                            transaction: tran);

                        for (int i = 0; i < to.Length; i++)
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("UserID", to[i]);
                            parameters.Add("GroupID", id);
                            connection.Execute("User_Group_Insert",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }  
                        tran.Commit();
                    }
                    connection.Close();
                } 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeletePermission(int groupid, int roleid)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string sql = $"DELETE Permission WHERE GroupID ={groupid} AND RoleID ={roleid}";
                    connection.Execute(sql,
                        commandType: CommandType.Text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ChangePermission(int groupId, List<Permissions> permissions)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        string sql = $"DELETE Permission WHERE GroupID ={groupId}";
                        connection.Execute(sql,
                            commandType: CommandType.Text,
                            transaction: tran);

                        foreach (var item in permissions)
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("GroupID", item.GroupID);
                            parameters.Add("RoleID", item.RoleID);
                            parameters.Add("Action", item.Action);
                            connection.Execute("Permission_Insert",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: tran);
                        }
                         
                        tran.Commit();
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public GroupUser Find(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ID", id);
                return connection.QueryFirstOrDefault<GroupUser>(
                    "GroupUser_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async void Delete(int id)
        {
            string sql = $"DELETE tbl_GroupUser WHERE ID ={id}";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql);
            }
        }

        public IEnumerable<GroupUser> GetAll()
        {
            string sql = $"SELECT * FROM tbl_GroupUser";
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<GroupUser>(sql);
            }
        }

        public IEnumerable<tbl_User> GetUserInGroup(int id)
        {
            string sql = $"SELECT u.ID,u.UserName,u.FullName FROM User_Group g LEFT JOIN UserAdmin u ON u.ID = g.UserID WHERE g.GroupID = @GroupID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("GroupID", id);
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<tbl_User>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<tbl_User> GetUserNotInGroup(int id)
        {
            string sql = $"SELECT ID,UserName,FullName FROM UserAdmin WHERE ID NOT IN(SELECT UserID FROM User_Group WHERE GroupID = @GroupID)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("GroupID", id);
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<tbl_User>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<Permissions> GetMenuByGroupId(int id)
        {
            string sql = $"SELECT p.Action,r.MenuID FROM Permission p JOIN Roles r ON p.RoleID = r.ID WHERE p.GroupID = @GroupID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("GroupID", id); 
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<Permissions>(sql, parameters, commandType: CommandType.Text);
            } 
        }

        public IEnumerable<Permissions> GetPermissionByGroupId(int id)
        {
            string sql = $"SELECT * FROM Permission WHERE GroupID = @GroupID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("GroupID", id);
            using (var connection = _context.CreateConnection())
            {
                return connection.Query<Permissions>(sql, parameters, commandType: CommandType.Text);
            }
        }

        public IEnumerable<GroupUser> GetPageGroupUser(int pageIndex, int pageSize, string keySearch, out int total)
        {
            total = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(*) FROM tbl_GroupUser");
            stringBuilder.Append(" SELECT * FROM tbl_GroupUser WHERE 1=1");
            if (!string.IsNullOrEmpty(keySearch))
                stringBuilder.Append($" AND Name LIKE N'%{keySearch}%'");
            stringBuilder.Append(" ORDER BY ID");
            stringBuilder.Append($" OFFSET (({pageIndex} - 1) * {pageSize}) ROWS");
            stringBuilder.Append($" FETCH NEXT {pageSize} ROWS ONLY;");
            using (var connection = _context.CreateConnection())
            {
                using (var multi = connection.QueryMultiple(stringBuilder.ToString()))
                {
                    total = multi.Read<int>().Single();
                    return multi.Read<GroupUser>();
                }
            }
        }
    }
}
