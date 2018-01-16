using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("HaxkonstDB.Test")]

namespace HaxkonstDB.Layers
{
    internal abstract class LayerBase
    {
		private LayerBase() { }

		internal LayerBase Fallback;
		internal LayerBase(LayerBase fallback)
		{
			Fallback = fallback;
		}

		internal abstract void Create(object obj);
		internal abstract void Update(object obj);
		internal abstract void Delete(object obj);
		internal abstract DatabaseResult<T> Find<T>(Func<T, bool> p);
	}

	

}
