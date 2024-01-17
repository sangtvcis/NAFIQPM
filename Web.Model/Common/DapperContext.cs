using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Web.Model.Common
{
    public partial class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectString; 
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectString = _configuration.GetConnectionString("dbConnection"); 
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectString); 

    }
}
