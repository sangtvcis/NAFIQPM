namespace Web.Areas.Admin.Models
{
    public class AccountProfileModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string GroupUserID { get; set; }  
        public bool IsAdmin { get; set; }
    }
}
