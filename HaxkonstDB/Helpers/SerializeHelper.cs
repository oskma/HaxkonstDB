using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HaxkonstDB.Helpers
{
    internal static class SerializeHelper
    {
        internal static string SerializeObject(object obj) { 
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        } 

        internal static object DeserializeObject(string str, Type type) {
			return Newtonsoft.Json.JsonConvert.DeserializeObject(str, type);
        }

		internal static string FolderName(Type t){
			var assemblieName = t.GetTypeInfo().Assembly.GetName().Name;
			var name = t.FullName + ", " + assemblieName;
			return name;
		}

		internal static DirectoryInfo GetTypeDirectory(DirectoryInfo baseDir, Type t)
		{
			var path = new List<string>() { FolderName(t) };

			Type bt = t.GetTypeInfo().BaseType;

			while (bt != null) {
				path.Insert(0, FolderName(bt));
				bt = bt.GetTypeInfo().BaseType;
			}

			var subdir = new DirectoryInfo(Path.Combine(baseDir.FullName, string.Join("/", path)));
			if (!subdir.Exists) {
				subdir.Create();
			}
			return subdir;
		}

		internal static bool IsNotReferenceType(Type type)
		{

			if (type.GetTypeInfo().IsInterface) {
				return true;
			}

			if (type.GetTypeInfo().IsValueType) {
				return true;
			}

			//treat string as valuetype
			if (type.Name == "String" ) {//todo: google if there is a better way to this when i get internet
				return true;
			}

			return false;
		}
	}
}
