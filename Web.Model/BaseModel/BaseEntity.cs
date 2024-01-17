using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Model.BaseModel
{
    public class BaseEntity
    {
        public int NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public int NguoiSua { get; set; }
        public DateTime NgaySua { get; set; }
        public int TrangThai { get; set; }
        public int ThuTu { get; set; }
    }
}
