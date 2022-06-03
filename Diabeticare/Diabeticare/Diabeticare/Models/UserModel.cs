using System;
using SQLite;

namespace Diabeticare.Models
{
    public class UserModel
    {
        // User model for local database, all models (except GroupModel) is tied to a user model
        public UserModel()
        {
            CreatedAt = DateTime.Now;
        }

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        [Unique] public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AutoLogIn { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}