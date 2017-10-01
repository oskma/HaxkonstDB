using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HaxkonstDB.Test.Entities;
using System.Linq;
using HaxkonstDB.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HaxkonstDB.Test
{
    [TestClass]
    public class CreateTests
    {
        private string dir = @"C:\Temp\db-test\savetest\";
        private Database database;

        [TestInitialize]
        public void Init(){
            database = new Database(dir);
        }

        [TestCleanup]
        public void Cleanup(){
            database = null;
            Directory.Delete(dir,true);
        }

        [TestMethod]
        public void SaveObjectTest(){

            var car = new Car()
            {
                Driver = "John Snow",
                IsInUse = true,
                Weight = 1000,
                YearlyMaintancenCost = 2500.34M,
                LicensePlate = "ADB123"
            };

            var dbObjects = database.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(0, dbObjects.Count());

            database.Create(car);

            dbObjects = database.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(1, dbObjects.Count());
        }



        [TestMethod]
        [ExpectedException(typeof(CreateExistingObjectException))]
        public void SaveAndAndUpdateObjectTest() {
            var boat = new Boat() {
                Name = "Boaty McBoatface",
                IsInUse = true
            };

            database.Create(boat);
            boat.IsInUse = false;
            database.Create(boat);

            var dbObjects = database.Find<Boat>(x => x.Name == boat.Name);
            Assert.AreEqual(2, dbObjects.Count());
        }

        [DataTestMethod]
        [DataRow("Анатолий Викторович", "char01")]
        [DataRow("Jörgen Bjönvaldsson", "char02")]
        [DataRow("Bob & John", "char03")]
        [DataRow("Dwayne \"The Rock\" Johnson", "char04")]
        public void SaveCharacterEncodingTest(string driver, string licensPlate)
        {
            var car = new Car()
            {
                Driver = driver,
                LicensePlate = licensPlate
            };

            database.Create(car);
            var dbObjects = database.Find<Car>(x => x.LicensePlate == car.LicensePlate);
            Assert.AreEqual(car.Driver, dbObjects.First().Driver);

        }

		[TestMethod]
		public void SaveSameObjectInMultipleThreads()
		{
			var t1 = new Task(()=> {
				database.Create(new Car() { Driver = "The stig" });
			} );
			var t2 = new Task(() => {
				database.Create(new Car() { Driver = "The stig" });
			});
			t1.Start();
			t2.Start();

			t1.Wait();
			t2.Wait();

			var obj = database.Find<Car>(x=> x.Driver == "The stig" );
			Assert.AreEqual(2, obj.Count());

		}

		[DataTestMethod]
		[DataRow("Lorem ipsum")]
		[DataRow(123324)]
		[DataRow((Int64)213423)]
		[DataRow((String)"Hodor")]
		[DataRow('g')]
		[DataRow(243.12)]
		[DataRow((byte)16)]
		[ExpectedException(typeof(NonReferenceTypeException))]
		public void SaveValueType(object obj){
			database.Create(obj);
		}

		[TestMethod]
		[ExpectedException(typeof(NonReferenceTypeException))]
		public void SaveDateTime()
		{
			var dt1 = new DateTime(2017, 08, 16);
			database.Create(dt1);
		}

		[TestMethod]
		public void SaveValueTypeList()
		{
			var dt1 = new List<string>() { "Cat" , "Dog" };
			database.Create((IEnumerable<string>)dt1);

			var list = database.Find<List<string>>(x => x.Count > 1);
			Assert.AreEqual("CatDog", string.Join("", dt1));
		}

		[TestMethod]
		public void SaveList()
		{
			var c1 = new Car() { Driver = "Leaila K" };
			var c2 = new Car() { Driver = "Tina Turner" };


			var dt1 = new List<Car>() { c1,c2 };
			database.Create(dt1);

			var list = database.Find<List<Car>>(x => x.Count == 2).FirstOrDefault();
			Assert.AreEqual("Tina Turner", list[1].Driver );
		}

		[TestMethod]
		public void SaveObjectWithObjects()
		{
			var g = new Garage { Built = DateTime.Parse("2005-04-03"), SquareMeters = 300 };
			g.Cars.Add(new Car() { Driver = "John Snow", LicensePlate = "asd123" });
			g.Cars.Add(new Car() { Driver = "The kingslayer", LicensePlate = "qwe123" });
			g.Cars.Add(new Car() { Driver = "Kalisi", LicensePlate = "dr4gon" });

			database.Create(g);

			var result = database.Find<Garage>(x => x.Built > DateTime.Parse("2000-02-02"));
			Assert.AreEqual(1, result.Count());
			Assert.AreEqual(3, result.First().Cars.Count());

		}

	}
}
