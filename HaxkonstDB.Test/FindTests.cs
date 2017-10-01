using HaxkonstDB.Exceptions;
using HaxkonstDB.Test.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaxkonstDB.Test
{
    [TestClass]
    public class FindTests
    {

        private string dir = @"C:\Temp\db-test\findtest\";
        private Database database;

        [TestInitialize]
        public void Init() {
            database = new Database(dir);
        }

        [TestCleanup]
        public void Cleanup() {
            database = null;
            Directory.Delete(dir, true);
        }


        [TestMethod]
        public void FindDiffernetTypeSameBasePropertiesTest() {

            var car = new Car {
                Weight = 500,
                IsInUse = true
            };

            var boat = new Boat {
                Weight = 500,
                IsInUse = false
            };

            database.Create(car);
            database.Create(boat);

            var dbObjects = database.Find<Boat>(x => x.Weight == 500);
            Assert.AreEqual(1, dbObjects.Count());
            Assert.AreEqual(false, dbObjects.First().IsInUse);
        }


        [TestMethod]
        public void FindBaseObjectTest() {

            var car = new Car {
                Driver = "Fat Mike",
                Weight = 600,
                IsInUse = true
            };

            database.Create(car);

            var dbObjects = database.Find<Vehicle>(x => x.Weight == 600);
            Assert.AreEqual(1, dbObjects.Count());
            Assert.AreEqual(true, dbObjects.First().IsInUse);
        }

        [TestMethod]
        public void FindCastableBaseObjectTest() {

            var car = new Car {
                Driver = "Johnny Rotten",
                Weight = 700,
                IsInUse = true
            };

            database.Create(car);

            var dbObjects = database.Find<Vehicle>(x => x.Weight == 700);

			Assert.AreEqual(true, dbObjects.First() is Car);
			Assert.AreEqual(car.Driver, ((Car)dbObjects.First()).Driver);
        }

		[TestMethod]
		public void FindAndOrder()
		{
			var cars = new List<Car>();
			for(int i = 0; i <10; i++) {
				cars.Add(new Car() { LicensePlate = "ABC" + i });
			}
			var rnd = new Random(500);
			var randomCars = cars.OrderBy(x => rnd.Next()).ToList();

			foreach(var car in randomCars) {
				database.Create(car);
			}

			var unorderdCars = database.Find<Car>(x => x.LicensePlate.StartsWith("ABC"));
			var unorderdStr = string.Join("", unorderdCars.Select(x => x.LicensePlate.Substring(3)));
			Assert.AreNotEqual("0123456789",unorderdStr);

			var orderdCars = database.Find<Car>(x => x.LicensePlate.StartsWith("ABC")).OrderBy(x => x.LicensePlate);
			var orderStr = string.Join("", orderdCars.Select(x => x.LicensePlate.Substring(3)));
			Assert.AreEqual("0123456789", orderStr);

		}

		[TestMethod]
		public void FindObjectWithObjects()
		{
			var g = new Garage { Built = DateTime.Parse("2005-04-03"), SquareMeters = 300 };
			g.Cars.Add(new Car() { LicensePlate = "asd123" });
			g.Cars.Add(new Car() { LicensePlate = "qwe123" });

			var g2 = new Garage { Built = DateTime.Parse("1994-04-03"), SquareMeters = 500 };
			g2.Cars.Add(new Car() { LicensePlate = "lmk456" });
			g2.Cars.Add(new Car() { LicensePlate = "oiu987" });

			database.Create(g);
			database.Create(g2);

			var result = database.Find<Garage>(x => x.Cars.Any(y => y.LicensePlate == "oiu987"));

			Assert.AreEqual(1, result.Count());
			Assert.AreEqual(g2.SquareMeters, result.First().SquareMeters);
		}

		[TestMethod]
		[ExpectedException(typeof(NonReferenceTypeException))]
		public void FindValueType()
		{
			var temp = database.Find<string>(x=>x == "lorem");
			var list = temp.ToList();
		}

		[TestMethod]
		[ExpectedException(typeof(NonReferenceTypeException))]
		public void FindInterface()
		{
			var temp = database.Find<IComparable>(x => x != null);
			var list = temp.ToList();
		}
	}
}
