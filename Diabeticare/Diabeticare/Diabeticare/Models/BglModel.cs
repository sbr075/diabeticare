using System;
using SQLite;

namespace Diabeticare.Models
{   
    public class BglModel
    {
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
}
