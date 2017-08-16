using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HaxkonstDB.Test.Entities;
using System.Linq;
using HaxkonstDB.Exceptions;

namespace HaxkonstDB.Test
{
    [TestClass]
    public class SaveTest
    {
        private string dir = @"C:\Temp\db-test\savetest\";
        private Database databse;

        [TestInitialize]
        public void Init()
        {
            databse = new Database(dir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            databse = null;
            Directory.Delete(dir,true);
        }



        [TestMethod]
        public void SaveObjectTest()
        {

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

    }
}
