using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{
    public class CarbohydrateModel
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public float Carbohydrates { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }
    }
}
