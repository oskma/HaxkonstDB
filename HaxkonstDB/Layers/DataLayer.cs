using HaxkonstDB.Exceptions;
using HaxkonstDB.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HaxkonstDB.Layers
{
	internal class DataLayer : LayerBase
	{
		private ObjectTracker objectTracker;
		private DirectoryInfo dir;

		internal DataLayer(DirectoryInfo directory, LayerBase fallback) : base(fallback)
		{
			objectTracker = new ObjectTracker();
			dir = directory;
		}

		internal override void Create(object obj)
		{
			if (objectTracker.Contains(obj)) {
				throw new CreateExistingObjectException("Cannot create object that is already in database");
			}

			if (SerializeHelper.IsNotReferenceType(obj.GetType())) {
				throw new NonReferenceTypeException("The database dose only support reference types, and not objects of type " + obj.GetType().Name);
			}

			var str = SerializeHelper.SerializeObject(obj);
			var filename = Guid.NewGuid().ToString();
			File.WriteAllText(Path.Combine(SerializeHelper.GetTypeDirectory(dir, obj.GetType()).FullName, filename), str);

			objectTracker.Insert(obj, filename);
		}

		internal override void Delete(object obj)
		{
			if (!objectTracker.Contains(obj)) {
				throw new DeleteUnexistingObjectException("Cannot delete object that is not in database");
			}
			var filename = objectTracker.GetFilename(obj);
			File.Delete(Path.Combine(SerializeHelper.GetTypeDirectory(dir, obj.GetType()).FullName, filename));
			objectTracker.Remove(obj);
		}

		internal override DatabaseResult<T> Find<T>(Func<T, bool> p)
		{
			if (SerializeHelper.IsNotReferenceType(typeof(T))) {
				throw new NonReferenceTypeException("The database dose only support reference types, and not objects of type " + typeof(T).Name);
			}
			return new DatabaseResult<T>(dir,p);
		}

		internal override void Update(object obj)
		{
			if (!objectTracker.Contains(obj)) {
				throw new UpdateUnexistingObjectException("Cannot update object that is not in database");
			}
			var filename = objectTracker.GetFilename(obj);
			var str = SerializeHelper.SerializeObject(obj);
			File.WriteAllText(Path.Combine(SerializeHelper.GetTypeDirectory(dir, obj.GetType()).FullName, filename), str);
		}
	}
}
