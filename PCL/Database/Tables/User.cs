using SQLite;

namespace PCL.Database.Tables
{
    [Table("UserData")]
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string email { get; set; }
        public string auth_token { get; set; }
    }
}