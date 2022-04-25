using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{   
    public class BglModel
    {

        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public float BGLmeasurement { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan BGLtime { get; set; }

    }

}
