using System.Collections;
using System.Collections.Generic;
using System;
using Identifiers;

using Items = System.Collections.Generic.List<Inventory.Item>;

public class Inventory
{
	const int ITEMS_MAX = 99; //Items in inventory
	const int QUANTITY_MAX = 99; //Item stack

	Trainer trainer;
	public Items items; //Doing operations on items or pockets outside this class is considered hard-coding, public until use is exclusively internal
		        
	public Inventory(Trainer trainer) {
		this.trainer = trainer;

		items = new Items();
	}

	public bool Add(int id, int quantity) {
		return Add(GetItem(id), id, quantity);
	}

	public bool Add(Item item, int id, int quantity) {
		if (item == null) {
			if (!HasCapacity())
				return false; //Inventory full
			
			items.Add(new Item(id, quantity));
		} else {
			if (!HasItemCapacity(id, quantity))
				return false;
			
			item.quantity += quantity;
		}
		
		return true;
	}

	public int AddGetRemainder(int id, int quantity) { //Adds an item and returns how many could not fit
		var item = GetItem(id);
		if (HasItemCapacity(item, quantity)) {
			Add(id, quantity);
			return 0;
		}

		var available = Math.Min(QUANTITY_MAX, item.quantity + quantity); //How much will fit
		var remainder = QUANTITY_MAX - available; //How many won't fit

		return remainder;
	}

    public bool Use(int id, int quantity = 1) {
		var item = items[0];

		if (quantity > item.quantity)
			return false;

		for(int i = 0; i < quantity; i++) { //Call the number of times used
			item.Use();
		}

		Remove(id, quantity);

		return true;
    }


	public bool Remove(int id, int quantity) {
		return Remove(GetItem(id), quantity);
	}

	public bool Remove(Item item, int quantity) {
		bool result = false;

		if (HasItemCapacity(item, quantity)) {
			result = true;
			item.quantity -= quantity;
		}
		
		if (item.quantity == 0) //If none are left in inventory, remove item from inventory
			items.Remove(item);

		return result;
	}

	public Items GetItems() { //All items in inventory
		return items;
	}

	public Items GetItems(ITEM_POCKET pocket) { //All items in specified pocket of inventory
		var items = new Items();

		foreach(var item in GetItems()) {
			if (item.data.pocket == pocket)
				this.items.Add(item);
		}

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

	public bool HasItemCapacity(int id, int quantity) {
		return HasItemCapacity(GetItem(id), quantity);
	}

	public bool HasItemCapacity(Item item, int quantity) {
		return ((item == null) ? quantity : item.quantity + quantity) <= QUANTITY_MAX;
	}

	public bool HasCapacity(int number = 1) { //Check if there's enough space in the inventory for another unique item
		return items.Count + number <= ITEMS_MAX;
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

