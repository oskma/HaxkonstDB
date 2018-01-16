//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace HaxkonstDB.Layers
//{

//	internal class CacheLayer : LayerBase
//	{
//		internal CacheLayer(LayerBase fallback) : base(fallback)
//		{

//		}

//		internal override void Create(object obj)
//		{
//			Fallback.Create(obj);
//		}

//		internal override void Delete(object obj)
//		{
//			Fallback.Delete(obj);
//		}

//		internal override IEnumerable<T> Find<T>(Func<T, bool> p)
//		{
//			return Fallback.Find(p);
//		}

//		internal override void Update(object obj)
//		{
//			Fallback.Update(obj);
//		}
//	}

//}
