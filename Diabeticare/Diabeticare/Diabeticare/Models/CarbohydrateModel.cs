using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Diabeticare.Models
{
    public class CarbohydrateModel
    {
        [PrimaryKey, AutoIncrement] public int ID { get; set; }
        public int ServerID { get; set; }
        public float Carbohydrates { get; set; }
        public DateTime DateOfInput { get; set; }
        public string FoodName { get; set; }
    }
}
