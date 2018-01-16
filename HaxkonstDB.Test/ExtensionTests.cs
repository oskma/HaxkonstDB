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
	public class ExtensionTests
	{

		private string dir = @"C:\Temp\db-test\extensiontest\";
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

		[DataTestMethod]
		[DataRow("abba,hugo,benny", "abba,benny,hugo")]
		[DataRow("walter,viggo,johan,123", "123,johan,viggo,walter")]
		public void OrderByStringTest(string unorderd, string orderd)
		{
			foreach(var s in unorderd.Split(',')) {
				var car = new Car() {
					Driver = s,
					Weight = 500
				};
				database.Create(car);
			}
			var all = database.Find<Car>(x => x.Weight == 500).OrderBy(x => x.Driver);		
			Assert.AreEqual(orderd, string.Join(",", all.Select(x=>x.Driver)));
		}

		[DataTestMethod]
		[DataRow(2, null, "c,d,e,f,g,h")]
		[DataRow(0, 5, "a,b,c,d,e")]
		[DataRow(null, null, "a,b,c,d,e,f,g,h")]
		[DataRow(3, 3, "d,e,f")]
		//[DataRow(10, null, null)]//todo
		public void SkipAndTakeTest(int? skip, int? take, string result)
		{
			foreach (var s in new []{ "a","b","c","d","e","f","g","h" }) {
				var car = new Boat() {
					Name = s,
					Buoyancy = 34.5f
				};
				database.Create(car);
			}

			var all = database.Find<Boat>(x => x.Buoyancy == 34.5f).OrderBy(x=>x.Name);
			if(skip != null) {
				all = all.Skip(skip.Value);
			}
			if(take != null) {
				all = all.Take(take.Value);
			}
			Assert.AreEqual(result, string.Join(",", all.Select(x=>x.Name)));

		}
		

    }
}
