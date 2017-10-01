using HaxkonstDB.Exceptions;
using HaxkonstDB.Test.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaxkonstDB.Test
{

    [TestClass]
    public class UpdateTests
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

		[TestMethod]
		[ExpectedException(typeof(UpdateUnexistingObjectException))]
		public void UpdateDeletedObject()
		{
			var boat = new Boat() {
				Name = "Freja",
				IsInUse = false
			};
			databse.Create(boat);

			var t1 = new Task(() => {
				var b = databse.Find<Boat>(x => !x.IsInUse).First();
				Thread.Sleep(50);
				databse.Delete(boat);
			});
			var t2 = new Task(() => {
				var b = databse.Find<Boat>(x => !x.IsInUse).First();
				t1.Wait();
				boat.IsInUse = true;
				databse.Update(boat);
			});
			t1.Start();
			t2.Start();

			t2.GetAwaiter().GetResult();
		}

		[TestMethod]
		public void UpdateSameObjectsInDifernetTreads()
		{
			var boat = new Boat() {
				Name = "Loveboat",
				IsInUse = true,
				YearlyMaintancenCost = 100.50M
			};
			databse.Create(boat);

			var t1 = new Task(() => {
				var b = databse.Find<Boat>(x => x.IsInUse).First();
				b.YearlyMaintancenCost = 500M;
				Thread.Sleep(50);
				databse.Update(b);
			});
			var t2 = new Task(() => {
				var b = databse.Find<Boat>(x => x.IsInUse).First();
				t1.Wait();
				Assert.AreEqual(100.50M, b.YearlyMaintancenCost);
				b.YearlyMaintancenCost = 750M;
				databse.Update(b);
			});
			t1.Start();
			t2.Start();

			t2.Wait();

			var b2 = databse.Find<Boat>(x => x.IsInUse).First();
			Assert.AreEqual(750M, b2.YearlyMaintancenCost);

		}

		[TestMethod]
		public void SaveToNewDatabaseInstance()
		{
			var car = new Car() {
				Driver = "Nicke Nyfiken",
				YearlyMaintancenCost = 200.50M,
				LicensePlate = "QWE123"
			};

			databse.Create(car);

			var dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
			Assert.AreEqual(1, dbObjects.Count());

			var databse2 = new Database(dir);//new instance

			car.LicensePlate = "ASD456";
			databse2.Update(car);

			dbObjects = databse2.Find<Car>(x => x.Driver == car.Driver);
			Assert.AreEqual(1, dbObjects.Count());
			Assert.AreEqual("ASD456", dbObjects.First().LicensePlate);
		}

	}
}
