/*
 * Inventory
 * Each trainer has their own independant inventory.
 * 
 * Common Function List:
 * Add(id, quantity)                :bool          Add an item to the inventory, it will combine with any existing    :Whether there was enough capacity for the item.
 * AddGetRemainder(id, quantity)    :int           Adds an item and if it doesn't fit, it'll fit as many as it can    :Returns the quantity left over.				
 * Use(id, quantity, target)        :bool          Runs the events for using an item. Targets are: Trainer/Pokemon    :Whether it had another items to use and that it succeeded.
 * Remove(id, quantity)             :bool          Removes a certain amount of items from the inventory.              :Whether it has enough items to remove.
 * GetItems(ITEM_POCKET)            :List<Item>    Gets a list of items in a specific pocket category.                :List of items in pocket.
 * GetItems()                       :List<Item>    Gets all items in inventory.                                       :List of all items.
 * GetItem(id)                      :Item          Gets the Inventory.Item inventory instance for that id.            :Item instance.
 * GetQuantity(id)                  :int           Gets the quantity of an item.                                      :Quantity of item.
 * HasItem(id)  					:int           Has an item of that id in inventory.                               :Has item success.
 * HasItemCapacity(id, quantity) 	:int           Has enough room for additional quantities of a certain item.       :Can fit a specified amount more of an item.
 * HasCapacity() 					:bool          Has enough room for a new unique item.                             :Can fit another kind of item in inventory.
 * Select(id) 						:void          Select active item, could be used to hold 2D items visibly.        :N/A
 * Transfer(ref Item) 				:void          Transfer an item into the inventory, from an Item instance.        :Will set Item reference to null if no quantity is left
 * Transfer(Inventory)              :void          Transfer all abled items from another inventory into this one.     :N/A
 */

using System.Collections;
using System.Collections.Generic;
using System;
using Identifiers;

using Items = System.Collections.Generic.List<Inventory.Item>;
using CachedPocket = System.Collections.Generic.Dictionary<Identifiers.ITEM_POCKET, Inventory.CachePocket>;

public class Inventory
{
	const int ITEMS_MAX = 99; //Items in inventory
	const int QUANTITY_MAX = 99; //Item stack

	public Trainer trainer;

	public Items items; //Doing operations on items or pockets outside this class is considered hard-coding, public until use is exclusively internal
	public Item selected;

	CachedPocket cachedPockets;

	public Inventory(Trainer trainer) {
		this.trainer = trainer;
		items = new Items();
		selected = null;
	}

	public bool Add(int id, int quantity) {
		return Add(GetItem(id), id, quantity);
	}

	public bool Add(Item item, int id, int quantity) {
		if (item == null) {
			if (!HasCapacity())
				return false; //Inventory full
			
			items.Add(new Item(this, id, quantity));
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

	public bool Use(int id, int quantity = 1, Target target = null) {
		return Use(GetItem(id), quantity, target);
    }

	public bool Use(Item item, int quantity = 1, Target target = null) {
		if (quantity > item.quantity)
			return false;

		target = (target == null) ? (Target)(System.Object)trainer : target; //Remember to take away the cast once trainer is inheriting Target properly
		for(int i = 0; i < quantity; i++) { //Call the number of times used
			item.HandleUse(target);
		}
		
		Remove(item, quantity);
		
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
		
		if (item.quantity == 0) { //If none are left in inventory, remove item from inventory
			if (trainer.item.id == item.id) //If the currently selected item has been entirely used up, switch to another
				selected = items[0];

			items.Remove(item);
		}

		return result;
	}

	public Items GetItems() { //All items in inventory
		return items;
	}

	public Items GetItems(ITEM_POCKET pocket) { //Returns specified pocket items as a list. Keeps the result cached in-case it's called upon every frame when drawing the inventory (no-gui)
		var existing = cachedPockets[pocket];
		if (existing != null && !existing.invalidate) //Check if there is a previous list of this data and ensure it's not been changed
			return existing.items;

		var items = new Items();

		foreach(var item in GetItems()) {
			if (item.data.pocket == pocket)
				this.items.Add(item);
		}

		existing = cachedPockets[pocket] = new CachePocket(items);

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

	public bool HasItem(int id) {
		return GetQuantity(id) > 0;
	}

	public bool HasItemCapacity(int id, int quantity) {
		return HasItemCapacity(GetItem(id), quantity);
	}

	public bool HasItemCapacity(Item item, int quantity) {
		return ((item == null) ? quantity : item.quantity + quantity) <= QUANTITY_MAX;
	}

	public bool HasCapacity(int quantity = 1) { //Check if there's enough space in the inventory for another unique item
		return items.Count + quantity <= ITEMS_MAX;
	}

	public void Select(int id) {
		Select(GetItem(id));
	}

	public void Select(Item item) {
		selected = item;
	}

	public void Transfer(ref Item ritem) {
		if (trainer != ritem.inventory.trainer) {
			int remainder = AddGetRemainder(ritem.id, ritem.quantity);
			ritem.quantity = remainder;

			if (ritem.quantity == 0) {
				ritem = null; //Garbage collector will free
			}
		}
	}

	public void Transfer(Inventory other) {
		other.GetItems().RemoveAll(item => {Transfer(ref item); return item == null;});
	}

	//Contains unique traits specific to the trainer's inventory
	public class Item : System.IComparable<Item> {
		public Inventory inventory; //Some form of reference is necessary for multiplayer and complex inventories

		public DataItem data;
		public int id {get{return data.id;} set{}}
		public int quantity;

		public Item(Inventory inventory, int id, int quantity = 1) {
			this.inventory = inventory;
			data = Data.items[id];
			this.quantity = quantity;
		}

		public int CompareTo(Item other) { //Sort by name
			return data.name.CompareTo(other.data.name);
		}

		public void Use(Target target = null) {
			inventory.Use(this, 1, target);
		}

		//Inventory will call this function and do actions accordingly for item handling
		public void HandleUse(Target target) { //do this and remember to save inventory
			//Item use effects
		}
	}

	public class CachePocket {
		public Items items;
		public bool invalidate;

		public CachePocket(Items items) {
			this.items = items;
			invalidate = false;
		}
	}
}

