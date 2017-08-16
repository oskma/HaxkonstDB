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
    public class FindTest
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

    }
}
