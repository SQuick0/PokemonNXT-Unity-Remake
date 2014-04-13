/*
 * We don't really want to duplicate items for each trainer's inventory.
 * Instead each each item in an inventory should reference the shared data.
 * The Inventory will have then a instantiable class with unique information and a reference to this object. (quantity, etc)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Identifiers;

public class DataItem : Data.BaseItem {
	public string name;
	public string description;
	public ITEM_POCKET pocket;
	public ITEM_FLAG_BINARY flags;

	public Texture2D icon;

	public DataItem(int id, string name, string icon, ITEM_POCKET pocket, ITEM_FLAG_BINARY flags, string description) : base(id) {
		this.name = name;
		this.icon = (Texture2D)Resources.Load("Icons/items/" + icon);
		this.pocket = pocket;
		SetFlags(flags);
		this.description = description;
	}

	public void SetFlags(ITEM_FLAG_BINARY flags) {
		this.flags = flags;
	}

	public void AddFlag(ITEM_FLAG_BINARY flag) {
		SetFlags(flags | flag);
	}
}