/*
 * We don't really want to duplicate items for each trainer's inventory.
 * Instead each each item in an inventory should reference the shared data.
 * The Inventory will have then a instantiable class with unique information and a reference to this object. (quantity, etc)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataItem : Data.BaseItem {
	public ItemTypes type = ItemTypes.Pokeball;
	public Texture2D icon;

	public DataItem(ItemTypes type, int id) : base(id){
		this.type = type;
		icon = (Texture2D)Resources.Load("Icons/"+type.ToString());
	}

	public void Use(){
		switch(type){
		case ItemTypes.Pokeball:
			//Pokeball.ThrowPokeBall(Player.This.gameObject);
			id--;
			return;
		}
	}

	public static void CombineInventory(List<DataItem> inventory){
		for(int i=0; i<inventory.Count; i++){
			for(int j=i+1; j<inventory.Count; j++){
				if (inventory[i].type==inventory[j].type){
					inventory[i].id += inventory[j].id;
					inventory.Remove(inventory[j]);
				}
			}
		}
	}

	public static string ItemDescription(ItemTypes type){
		switch(type){
		case ItemTypes.Pokeball:	return "A device for catching wild Pokemon. It's thrown like a ball at a Pokemon, comfortably encapsulating its target.";
		case ItemTypes.Potion:		return "A spray-type medicine for treating wounds. It can be used to restore a small amount of HP to an injured Pokemon.";
		}
		return "";
	}
}

public enum ItemTypes{
	Pokeball,
	Greatball,
	Ultraball,
	Masterball,

	Potion,
	SuperPotion,
	HyperPotion,
	MaxPotion
};