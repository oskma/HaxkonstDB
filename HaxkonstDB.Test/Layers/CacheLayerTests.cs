using HaxkonstDB.Layers;
using HaxkonstDB.Test.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaxkonstDB.Test.Layers
{
	[TestClass]
	public class CacheLayerTests
    {

		[TestInitialize]
		public void Init()
		{
					
		}

		[TestCleanup]
		public void Cleanup()
		{

		}

		[TestMethod]
		public void ClearCacheWhenObjectUpdatedTest()
		{
			var mockLayer = new MockLayer(null);
			var database = new Database(new CacheLayer(mockLayer));

			var car = new Car() { Driver = "Bob" , LicensePlate = "1234"};

			database.Create(car);

			mockLayer.FindResponseList = new List<Car> { car };

			var result = database.Find<Car>(x => x.LicensePlate == "1234").FirstOrDefault();

			result.Driver = "Billy";

			database.Update(result);

			var r2 = database.Find<Car>(x => x.LicensePlate == "1234").FirstOrDefault();

			Assert.AreEqual(r2.Driver, ((Car)mockLayer.UpdateCallArg).Driver);

		}


	}
}
