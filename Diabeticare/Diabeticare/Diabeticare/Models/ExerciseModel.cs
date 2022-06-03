using System;
using SQLite;

namespace Diabeticare.Models
{
    public class ExerciseModel
    {
        // Exercise model for local database
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int UserID { get; set; }
        public int ServerID { get; set; }
        public string Name { get; set; }
        public DateTime ExerciseStart { get; set; }
        public DateTime ExerciseEnd { get; set; }
    }
}
