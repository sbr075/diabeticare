using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{
    public class SleepModel
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
