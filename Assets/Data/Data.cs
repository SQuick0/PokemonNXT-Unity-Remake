/*
 * This class binds clockwork's data bindings into the project.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Data
{
	static public bool hasLoaded {get;set;}

	static public DataSet<DataItem> items; //Referenced by inventory item instances as shared data

	static Data() {
		Init(); //Make it load immediately, for now
		LoadMock();
	}

	static public void LoadMock() {
		//Insert mock database test data items here. Things like in-game items can still be added in trainer create.
		//I still need to add in pockets and item flag values
		items.Add(new DataItem(1, "Pokeball", "poke-ball.png", "A device for catching wild Pokemon. It's thrown like a ball at a Pokemon, comfortably encapsulating its target."));
		items.Add(new DataItem(4, "Potion", "potion.png", "A spray-type medicine for treating wounds. It can be used to restore a small amount of HP to an injured Pokemon."));
	}

	//Connect to the database, query, record data and then DataQuery is be destroyed
	static public void Init() {
		hasLoaded = true;

		items = new DataSet<DataItem>(100); //Make it pre-allocate capacity based on the max id of the data in the database
	}
	
	//Will free upon leaving through the menu.
	static public void Free() {
		hasLoaded = false;
	}

	//Indexible structures are the most efficient by implementation, as not all items are stored consecutively and large Dictionaries are to inefficient for this use, this will give us the best of both worlds.
	//After loading they will remain a fixed length, such requirements for quick inserting and removal such as a list is not necessary
	public class DataSet<DataType> {
		private DataType[] data;

		public DataSet(int allocate) {
			data = new DataType[allocate];
		}

		public DataType this[int id]
		{
			get { return Get(id); }
			set {}
		}
		
		public DataType Get(int id) {
			if (System.Diagnostics.Debugger.IsAttached) {
				throw new Exception(String.Format("Error: Data not loaded for '{1}'.", typeof(DataType).GetType().Name));
				if (data[id] == null)
					throw new Exception(String.Format("Error: Data item does not exist an id of '{1}'.", id));
			}

			return data[id];
		}

		//Remember that item is another name for an element in an array/list.
		public DataType Add(int id, DataType item) {
			if (data.Length >= id)
				Array.Resize(ref data, id + 1);

			data[id] = item;

			return item;
		}

		public DataType Add(DataType item) {
			BaseItem cast = item as BaseItem; //Cast into its base form to obtain id

			if (cast == null) //Handle data which is not inheriting BaseItem
				throw new Exception(String.Format("Error: Data item type '{1}' is not inheriting BaseItem.", typeof(BaseItem).GetType().Name));

			return Add(cast.id, item);
		}
	}

	//Inherited by all data items. Polymorphism allows us to interact with any base item class generically.
	public class BaseItem {
		public int id;
		
		public BaseItem(int id) {
			this.id = id;
		}
	}
}