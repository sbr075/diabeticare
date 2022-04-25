using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{
    public class SleepModel
    {
<<<<<<< HEAD
        /*
        public Sleep()
        {
            CreatedAt = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
        }
        */

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int ServerID { get; set; }
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
=======
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public DateTime SleepStart { get; set; }
        public DateTime SleepEnd { get; set; }
        public DateTime CreatedAt { get; set; }

>>>>>>> a5b6c1855a678a217245ed1422ab1dd5951f4ef7
    }
}
