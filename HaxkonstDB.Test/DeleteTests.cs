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
	public class DeleteTests
	{

		private string dir = @"C:\Temp\db-test\deltetest\";
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
		public void DeleteObjectTest() {
			var car = new Car() {
				Driver = "Homer Simpson",
				LicensePlate = "ADB789"
			};

			databse.Create(car);

			var dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
			Assert.AreEqual(1, dbObjects.Count());

			databse.Delete(car);

			dbObjects = databse.Find<Car>(x => x.Driver == car.Driver);
			Assert.AreEqual(0, dbObjects.Count());
		}

		[TestMethod]
		[ExpectedException(typeof(DeleteUnexistingObjectException))]
		public void DeletNonExistingObjectTest() {
			var car = new Car() {
				Driver = "Lisa Simpson",
				LicensePlate = "AGB789"
			};
			databse.Delete(car);
		}

    }
}
