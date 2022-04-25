using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{
    public class ExerciseModel
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan BGLtime { get; set; }
        public string Note { get; set; }
    }
}
