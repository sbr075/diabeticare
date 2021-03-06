using System;
using SQLite;

namespace Diabeticare.Models
{
    // BGL model for local database
    public class BglModel
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int UserID { get; set; }
        public int ServerID { get; set; }
        public float BGLmeasurement { get; set; }
        public DateTime TimeOfMeasurment { get; set; }
    }
}
