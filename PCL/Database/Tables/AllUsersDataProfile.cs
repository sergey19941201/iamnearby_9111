using SQLite;

namespace PCL.Database.Tables
{
    [Table("AllUsersDataProfile")]
    public class AllUsersDataProfile
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string user_profile { get; set; }
    }
}