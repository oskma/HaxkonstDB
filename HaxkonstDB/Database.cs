using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using HaxkonstDB.Helpers;
using HaxkonstDB.Exceptions;
using System.Reflection;

namespace HaxkonstDB
{
    public class Database
    {
        private DirectoryInfo dir;
        private Dictionary<object, string> objDict;

		/// <summary>
		/// Create an instance of the database
		/// </summary>
		/// <param name="filepath">The filepath to where that data is stored</param>
        public Database(string filepath) {
            dir = new DirectoryInfo(filepath);            
            if (!dir.Exists){
                dir.Create();
            }
            objDict = new Dictionary<object, string>();
        }

		/// <summary>
		/// Add a new object to the database
		/// </summary>
		/// <param name="obj">The object that should be added</param>
        public void Create(object obj) {

            if (objDict.ContainsKey(obj)) {
                throw new CreateExistingObjectException("Cannot create object that is already in database");
            }

			if (SerializeHelper.IsValueType(obj)) {
				throw new NonReferenceTypeException("The database dose only support reference types, and not objects of type " + obj.GetType().Name);
			}

            var str = SerializeHelper.SerializeObject(obj);
            var filename = Guid.NewGuid().ToString();
			File.WriteAllText(Path.Combine(SerializeHelper.GetTypeDirectory(dir,obj.GetType()).FullName, filename), str);

            objDict.Add(obj, filename);
        }

		/// <summary>
		/// Update an exisiting object
		/// </summary>
		/// <param name="obj">An object that has been retrived from the database</param>
        public void Update(object obj) {
            if (!objDict.ContainsKey(obj)) {
                throw new UpdateUnexistingObjectException("Cannot update object that is not in database");
            }
            var key = objDict[obj];
            var str = SerializeHelper.SerializeObject(obj);
			File.WriteAllText(Path.Combine(SerializeHelper.GetTypeDirectory(dir,obj.GetType()).FullName, key), str);           
        }

		/// <summary>
		/// Delete and object
		/// </summary>
		/// <param name="obj">An object that has been retrived from the database</param>
        public void Delete(object obj) {
            if (!objDict.ContainsKey(obj)) {
                throw new DeleteUnexistingObjectException("Cannot delete object that is not in database");
            }
            var key = objDict[obj];
			File.Delete(Path.Combine(SerializeHelper.GetTypeDirectory(dir,obj.GetType()).FullName, key));
            objDict.Remove(obj);
        }

		/// <summary>
		/// Finds objects in the database
		/// </summary>
		/// <typeparam name="T">The type of object that should be searched</typeparam>
		/// <param name="p">The query for retriving objects</param>
		/// <returns>A list of of 0 or more objects of the specified type</returns>
        public IEnumerable<T> Find<T>(Func<T, bool> p) {

			foreach (var file in SerializeHelper.GetTypeDirectory(dir,typeof(T)).GetFiles("*",SearchOption.AllDirectories)) {

				Type fileType = Type.GetType(file.Directory.Name,true);

				var obj = SerializeHelper.DeserializeObject(File.ReadAllText(file.FullName), fileType);

				if (!(obj is T)) {
                    continue;
                }
                if (p((T)obj)) {
                    objDict.Add(obj, file.Name);
                    yield return (T)obj;
                }
            }
        }
    }
}
