using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HaxkonstDB.Test.Entities;
using System.Linq;
using HaxkonstDB.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace HaxkonstDB.Test
{
    [TestClass]
    public class CreateTest
    {
        private string dir = @"C:\Temp\db-test\savetest\";
        private Database databse;

        [TestInitialize]
        public void Init(){
            databse = new Database(dir);
        }

        [TestCleanup]
        public void Cleanup(){
            databse = null;
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

            var dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(0, dbObjects.Count());

            databse.Create(car);

            dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(1, dbObjects.Count());
        }



        [TestMethod]
        [ExpectedException(typeof(CreateExistingObjectException))]
        public void SaveAndAndUpdateObjectTest() {
            var boat = new Boat() {
                Name = "Boaty McBoatface",
                IsInUse = true
            };

            databse.Create(boat);
            boat.IsInUse = false;
            databse.Create(boat);

            var dbObjects = databse.Find<Boat>(x => x.Name == boat.Name);
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

            databse.Create(car);
            var dbObjects = databse.Find<Car>(x => x.LicensePlate == car.LicensePlate);
            Assert.AreEqual(car.Driver, dbObjects.First().Driver);

        }

		[TestMethod]
		public void SaveSameObjectInMultipleThreads()
		{
			var t1 = new Task(()=> {
				databse.Create(new Car() { Driver = "The stig" });
			} );
			var t2 = new Task(() => {
				databse.Create(new Car() { Driver = "The stig" });
			});
			t1.Start();
			t2.Start();

			t1.Wait();
			t2.Wait();

			var obj = databse.Find<Car>(x=> x.Driver == "The stig" );
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
			databse.Create(obj);
		}

		[TestMethod]
		[ExpectedException(typeof(NonReferenceTypeException))]
		public void SaveDateTime()
		{
			var dt1 = new DateTime(2017, 08, 16);
			databse.Create(dt1);
		}


	}
}
