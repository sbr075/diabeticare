using System;
<<<<<<< HEAD
=======
using System.Collections.Generic;
using System.Text;
>>>>>>> a5b6c1855a678a217245ed1422ab1dd5951f4ef7
using SQLite;

namespace Diabeticare.Models
{   
    public class BglModel
    {
<<<<<<< HEAD
        /*
        public Bgl()
        {
            CreatedAt = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
        }
        */

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int ServerID { get; set; }
        public float BGLmeasurement { get; set; }
        public DateTime TimeOfMeasurment { get; set; }
    }
=======

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public float BGLmeasurement { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan BGLtime { get; set; }

    }

>>>>>>> a5b6c1855a678a217245ed1422ab1dd5951f4ef7
}
