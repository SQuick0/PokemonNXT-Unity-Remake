using System.Collections;
using System.Collections.Generic;
using System;
using Identifiers;

using Items = System.Collections.Generic.List<Inventory.Item>;

public class Inventory
{
	const int INVENTORY_MAX = 100;

	Trainer trainer;
	public Items items; //Doing operations on items or pockets outside this class is considered hard-coding, public until use is exclusively internal
		        
	public Inventory(Trainer trainer) {
		this.trainer = trainer;

		items = new Items();
	}

	public void Add(int id, int quantity) {
		var item = new Item(id, quantity);
		items.Add(item);
	}

    public bool Use(int id, int quantity) {
		var item = items[0];

		if (quantity > item.quantity)
			return false;

		for(int i = 0; i < quantity; i++) { //Call the number of times specified
			item.Use();
		}

		Remove(id, quantity);

		return true;
    }


	public bool Remove(int id, int quantity) {
		var item = GetItem(id);
		if (item.quantity <= quantity) {
			return true;
		}

		return false;
	}

	public bool Remove(Item item, int quantity) {
		bool result = false;

		if (item.quantity <= quantity) {
			result = true;
			item.quantity -= quantity;
		}
		
		if (item.quantity == 0) //If none are left in inventory, remove item from inventory
			items.Remove(item);

		return result;
	}

	public Items GetItems() {
		return items;
	}

	public Item GetItem(int id) {
		foreach(var item in GetItems()) {
			if (id == item.id)
				return item;
		}

		return null;
	}

	public int GetQuantity(int id) {
		var item = GetItem(id);
		if (item != null)
			return item.quantity;

		return 0;
	}

	//Contains unique traits specific to the trainer's inventory
	public class Item {
		public DataItem data;

		public int id {get{return data.id;} set{}}
		public int quantity;

		public Item(int id, int quantity = 1) {
			data = Data.items[id];

			this.quantity = quantity;
		}

		//Inventory will call this function and do actions accordingly for item handling
		public void Use() {
		}
	}
}

