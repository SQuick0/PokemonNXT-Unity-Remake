using System.Collections;
using System.Collections.Generic;
using System;

public class PokeParty
{
	public const int PARTY_MAX = 10;

	Trainer trainer; //Enables it to be usable by any trainer in multiplayer (independant)
	List<Slot> slots;
	int selected;
	
	public PokeParty(Trainer trainer) {
		this.trainer = trainer;
		slots = new List<Slot>();
		Select(-1); //Assume the trainer has no pokemon
	}
	
	public int Count() {
		return slots.Count;
	}
	
	public bool HasPokemon() {
		return Count() > 0;
	}
	
	public List<Slot> GetSlots() {
		return slots;
	}
	
	public Slot GetSlot(int index) {
		return GetSlots()[index]; 
	}

	public Slot GetActive() {
		if (selected == -1)
			return null;

		var slot = GetSlot(selected);
		return (slot != null) ? slot : null;
	}
	
	public Pokemon GetActivePokemon() {
		var slot = GetActive();
		return (slot != null) ? slot.pokemon : null;
	}
	
	public bool IsActive(Pokemon pokemon) {
		return GetActivePokemon() == pokemon;
	}
	
	public bool CanAddPokemon() {
		return Count() + 1 < PARTY_MAX;
	}
	
	public bool AddPokemon(Pokemon pokemon) {
		if (!CanAddPokemon())
			return false;

		var slot = new Slot(this, pokemon);
		GetSlots().Add(slot);

		if (selected == -1)
			Select(slot.index); //Select by default if no pokemon is selected

		return true;
	}

	public void RemovePokemon(int index) {
		var slot = GetSlot(index);
		slots.RemoveAt(index);

		if (selected == index) //If the current Pokemon was removed, select the previous. (If there are none left, it will set it correctly to -1)
			Select(Count() - 1);
	}
	
	public Slot Select(int index) {
		if (index < 0 || index >= Count()) { //Check if it's an invalid slot
			selected = -1;
			return null;
		}

		var slot = GetSlots()[index];
		selected = index;

		return slot;
	}
	
	public Slot SelectNext() {
		var index = (selected - 1) % Count();  //Loop slot index when beyond bounds
		return Select(index);
	}
	
	public Slot SelectPrev() {
		var index = ((selected - 1) + Count() - 1) % Count();  //Loop slot index when below bounds
		return Select(index);
	}
	
	public void Swap(int index1, int index2) {
		var slots = GetSlots();
		
		if (System.Diagnostics.Debugger.IsAttached){
			if ((index1 < 0 || index1 >= Count()) || (index2 < 0 || index2 >= Count()))
				throw new Exception("Error: A PokeParty swap index is invalid.");
		}
		
		Slot slot = slots[index1];
		slots[index1] = slots[index2];
		slots[index2] = slot;
	}

	public Pokemon GetPokemon(int id) {
		foreach (var slot in slots) {
			if (slot.pokemon.number == id)
				return slot.pokemon;
		}

		return null;
	}
	
	public class Slot {
		public int index {get{return pokeParty.GetSlots().FindIndex(v => (v != null) ? v == this : false);}  set{}} //Directly refer to List<> for the index
		public Pokemon pokemon;
		
		private PokeParty pokeParty;
		
		public Slot(PokeParty pokeParty, Pokemon pokemon) {
			this.pokeParty = pokeParty;
			this.index = -1;
			this.pokemon = pokemon;
		}
	}
}

