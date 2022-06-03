using System;
using SQLite;

namespace Diabeticare.Models
{
    public class MoodModel
    {
        // Mood model for local database
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int UserID { get; set; }
        public int ServerID { get; set; }
        public int MoodValue { get; set; }
        public DateTime Date { get; set; }
    }
}
