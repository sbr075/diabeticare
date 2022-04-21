using System;
using SQLite;

namespace Diabeticare.Models
{   
    public class Bgl
    {
        /*
        public Bgl()
        {
            CreatedAt = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
        }
        */

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public float BGLmeasurement { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan BGLtime { get; set; }

    }

}
