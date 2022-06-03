using System;
using SQLite;

namespace Diabeticare.Models
{
    public class SleepModel
    {
        // Sleep model for local database
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int UserID { get; set; }
        public int ServerID { get; set; }
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
    }
}
