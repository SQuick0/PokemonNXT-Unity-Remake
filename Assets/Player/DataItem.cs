/*
 * We don't really want to duplicate items for each trainer's inventory.
 * Instead each each item in an inventory should reference the shared data.
 * The Inventory will have then a instantiable class with unique information and a reference to this object. (quantity, etc)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataItem : Data.BaseItem {
	public string name;
	public string description;

	public Texture2D icon;

	public DataItem(int id, string name, string icon, string description) : base(id) {
		this.name = name;
		this.description = description;
		this.icon = (Texture2D)Resources.Load("Icons/items/" + icon);
	}
}