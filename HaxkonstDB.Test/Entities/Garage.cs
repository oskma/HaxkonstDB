using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Test.Entities
{
    public class Garage
    {
		public Garage(){
			Cars = new List<Car>();
		}

		public int SquareMeters { get; set; }
		public DateTime Built { get; set; }
		public List<Car> Cars { get; set; }
    }
}
