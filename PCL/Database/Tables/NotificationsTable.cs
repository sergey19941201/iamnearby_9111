using SQLite;

namespace PCL.Database.Tables
{
    [Table("NotificationsTable")]
    public class NotificationsTable
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public bool on { get; set; }
    }
}