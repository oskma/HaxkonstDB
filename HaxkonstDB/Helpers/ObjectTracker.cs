using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Helpers
{
    internal class ObjectTracker
    {
		//todo: user something better then a dictionary
		//todo: change singelton pattern?
		private static Dictionary<WeakReference, string> objDict = null;

		internal ObjectTracker()
		{
			if (objDict == null) {
				objDict = new Dictionary<WeakReference, string>();
			}
		}

		internal void Insert(object obj, string filename)
		{
			objDict.Add(new WeakReference(obj), filename);
		}

		internal void Remove(object obj)
		{
			foreach (KeyValuePair<WeakReference, string> entry in objDict) {
				if (entry.Key.Target == obj) {
					objDict.Remove(entry.Key);
					return;
				}
			}
		}

		internal string GetFilename(object obj)
		{
			foreach (KeyValuePair<WeakReference, string> entry in objDict) {
				if (entry.Key.Target == obj) {
					return entry.Value;
				}
			}
			throw new KeyNotFoundException("Could not find filename for object");
		}

		internal bool Contains(object obj)
		{
			foreach (KeyValuePair<WeakReference, string> entry in objDict) {
				if(entry.Key.Target == obj) {
					return true;
				}
			}
			return false;
		}

    }
}
