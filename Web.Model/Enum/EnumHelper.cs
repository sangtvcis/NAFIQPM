using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.Enum
{
    public class EnumHelper
    {
        public enum Action
        {
            View = 1,
            Add = 2,
            Edit = 3,
            Delete = 4, 
            Approved = 5
        }

        public enum UserType
        {
            Admin = 1,
            ChuyenVien = 2,
            ThanhVien = 3
        }
        
    }
}
