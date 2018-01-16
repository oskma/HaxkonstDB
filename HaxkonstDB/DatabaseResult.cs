using HaxkonstDB.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HaxkonstDB
{
	public class DatabaseResult<T> : IEnumerable<T>, IEnumerable
	{
		//List<T> items = null;// new List<T>();
		private ObjectTracker objectTracker;
		private Func<T, bool> filter = null;
		private Func<T, object> order = null;
		private DirectoryInfo directory = null;
		private FileInfo[] files = null;
		private int? skip = null;
		private int? take = null;
		
		
		private DatabaseResult(){
			throw new NotSupportedException();
		}

		internal DatabaseResult(DirectoryInfo dir, Func<T, bool> p)
		{
			//items = new List<T>();
			objectTracker = new ObjectTracker();
			filter = p;
			directory = dir;
			files = SerializeHelper.GetTypeDirectory(directory, typeof(T)).GetFiles("*", SearchOption.AllDirectories);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (order != null) {
				var orderdDict = new Dictionary<T, string>();
				foreach (var file in files) {
					Type fileType = Type.GetType(file.Directory.Name, true);
					var obj = SerializeHelper.DeserializeObject(File.ReadAllText(file.FullName), fileType);
					if (!(obj is T)) {
						continue;
					}
					if (filter == null || filter((T)obj)) {
						orderdDict.Add((T)obj, file.FullName);
					}
				}
				files = orderdDict.Keys.OrderBy(order).Select(x => new FileInfo( orderdDict[x]) ).ToArray();
			}
			

			foreach (var file in files) {

				Type fileType = Type.GetType(file.Directory.Name, true);

				var obj = SerializeHelper.DeserializeObject(File.ReadAllText(file.FullName), fileType);

				if (!(obj is T)) {
					continue;
				}
				if (filter == null || filter((T)obj)) {
					if(skip != null && skip > 0) {
						skip--;
						continue;
					}
					if(take != null) {
						if(take == 0) {
							break;
						} else {
							take--;
						}
					}

					objectTracker.Insert(obj, file.Name);
					yield return (T)obj;
				}
			}

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public DatabaseResult<T> OrderBy(Func<T, object> p)
		{
			this.order = p;
			return this;
		}

		public DatabaseResult<T> Skip(int s)
		{
			skip = s;
			return this;
		}

		public DatabaseResult<T> Take(int t)
		{
			take = t;
			return this;
		}

	}


	//public static class ResultExtensions
	//{
	//	public static DatabaseResult<T> OrderByExtension<T>(this DatabaseResult<T> child, Func<T, object> p) {
	//		return child;
	//	}
	//}

}
