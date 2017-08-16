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
    public class UpdateTest
    {

        private string dir = @"C:\Temp\db-test\updatetest\";
        private Database databse;

        [TestInitialize]
        public void Init() {
            databse = new Database(dir);
        }

        [TestCleanup]
        public void Cleanup() {
            databse = null;
            Directory.Delete(dir, true);
        }



        [TestMethod]
        public void UpdateObjectTest() {

            var car = new Car() {
                Driver = "Nicke Nyfiken",
                YearlyMaintancenCost = 200.50M,
                LicensePlate = "QWE123"
            };

            databse.Create(car);

            var dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(1, dbObjects.Count());

            car.LicensePlate = "ASD456";
            databse.Update(car);

            dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
            Assert.AreEqual(1, dbObjects.Count());
            Assert.AreEqual("ASD456", dbObjects.First().LicensePlate);
        }

        [TestMethod]
        [ExpectedException(typeof(UpdateUnexistingObjectException))]
        public void UpdateNonExistingObjectTest() {
            var car = new Car() {
                Driver = "Kusin Vittamin",
                LicensePlate = "AGB789G"
            };
            databse.Update(car);
        }

    }
}
