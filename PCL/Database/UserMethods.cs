using PCL.Database.Tables;
using SQLite;
using System.IO;

namespace PCL.Database
{
    public class UserMethods
    {
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");

        public void InsertUser(string auth_token, string email)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<User>();
                var user_table = db.Table<User>();

                //clearing user table
                foreach (var user in user_table)
                {
                    RemoveUser(user.Id);
                }

                db.Insert(new User { auth_token = auth_token, email = email });
            }
        }
        public void RemoveUser(int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                var item = db.Get<User>(id);
                db.Delete(item);
            }
        }
        public void ClearTable()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<User>();
                var user_table = db.Table<User>();
                foreach (var user in user_table)
                {
                    db.Delete(user);
                }
            }
        }
        public bool UserExists()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<User>();
                var user_table = db.Table<User>();
                foreach (var user in user_table)
                {
                    return true;
                }
                return false;
            }
        }
        public string GetUsersAuthToken()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<User>();
                var user_table = db.Table<User>();
                foreach (var user in user_table)
                {
                    return user.auth_token;
                }
                return "token not exists";
            }
        }
        public string GetUsersEmail()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<User>();
                var user_table = db.Table<User>();
                foreach (var user in user_table)
                {
                    return user.email;
                }
                return "email not exists";
            }
        }
        public void InsertProfileData(string userProfile)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<AllUsersDataProfile>();
                var user_table = db.Table<AllUsersDataProfile>();

                //clearing user table
                foreach (var user_data in user_table)
                {
                    RemoveUserData(user_data.Id);
                }

                db.Insert(new AllUsersDataProfile { user_profile = userProfile });
            }
        }
        public string GetUserData()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<AllUsersDataProfile>();
                var user_table = db.Table<AllUsersDataProfile>();

                foreach (var user_data in user_table)
                {
                    return user_data.user_profile;
                }
                return null;
            }
        }
        public void RemoveUserData(int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                var item = db.Get<AllUsersDataProfile>(id);
                db.Delete(item);
            }
        }
        public void ClearUsersDataTable()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<AllUsersDataProfile>();
                var users_data_table = db.Table<AllUsersDataProfile>();
                foreach (var user in users_data_table)
                {
                    db.Delete(user);
                }
            }
        }


        public void InsertNotif(bool on)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<NotificationsTable>();
                var user_table = db.Table<NotificationsTable>();

                //clearing user table
                foreach (var user in user_table)
                {
                    RemoveNotif(user.Id);
                }

                db.Insert(new NotificationsTable { on = on });
            }
        }
        public void RemoveNotif(int id)
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                var item = db.Get<NotificationsTable>(id);
                db.Delete(item);
            }
        }
        public void ClearTableNotif()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<NotificationsTable>();
                var user_table = db.Table<NotificationsTable>();
                foreach (var user in user_table)
                {
                    db.Delete(user);
                }
            }
        }
        public bool GetNotifData()
        {
            using (SQLiteConnection db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<NotificationsTable>();
                var user_table = db.Table<NotificationsTable>();

                foreach (var user_data in user_table)
                {
                    return user_data.on;
                }
                return true;
            }
        }
    }
}