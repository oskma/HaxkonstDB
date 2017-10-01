using HaxkonstDB.Layers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Test.Layers
{
	class MockLayer : LayerBase
	{

		public object CreateCallArg = null;
		public object DeleteCallArg = null;
		public object UpdateCallArg = null;
		public IEnumerable<object> FindResponseList = null;

		internal MockLayer(LayerBase fallback) : base(fallback)
		{

		}

		internal override void Create(object obj)
		{
			CreateCallArg = obj;
		}

		internal override void Delete(object obj)
		{
			DeleteCallArg = obj;
		}

		internal override IEnumerable<T> Find<T>(Func<T, bool> p)
		{
			return (IEnumerable<T>)FindResponseList;
		}

		internal override void Update(object obj)
		{
			UpdateCallArg = obj;
		}
	}
}
