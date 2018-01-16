using System;
using System.Collections.Generic;
using System.IO;
using HaxkonstDB.Layers;

namespace HaxkonstDB
{
    public class Database
    {
        	
		private LayerBase dataHandler;

		/// <summary>
		/// Create an instance of the database
		/// </summary>
		/// <param name="filepath">The filepath to where that data is stored</param>
        public Database(string filepath) {
			DirectoryInfo dir = new DirectoryInfo(filepath);            
            if (!dir.Exists){
                dir.Create();
            }	
			//dataHandler = new CacheLayer(new IndexLayer(new DataLayer(dir, null)));
			dataHandler = new DataLayer(dir, null);
		}

		internal Database(LayerBase layer){
			dataHandler = layer;
		}

		/// <summary>
		/// Add a new object to the database
		/// </summary>
		/// <param name="obj">The object that should be added</param>
        public void Create(object obj) {
			dataHandler.Create(obj);         
        }

		/// <summary>
		/// Update an exisiting object
		/// </summary>
		/// <param name="obj">An object that has been retrived from the database</param>
        public void Update(object obj) {
			dataHandler.Update(obj);   
        }

		/// <summary>
		/// Delete and object
		/// </summary>
		/// <param name="obj">An object that has been retrived from the database</param>
        public void Delete(object obj) {
			dataHandler.Delete(obj);
        }

		/// <summary>
		/// Finds objects in the database
		/// </summary>
		/// <typeparam name="T">The type of object that should be searched</typeparam>
		/// <param name="p">The query for retriving objects</param>
		/// <returns>A list of of 0 or more objects of the specified type</returns>
        public DatabaseResult<T> Find<T>(Func<T, bool> p) {
			return dataHandler.Find(p);
        }
    }
}
