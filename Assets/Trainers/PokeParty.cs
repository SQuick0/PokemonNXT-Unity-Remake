/*
 * PokeParty
 * Each trainer has their own party of Pokemon..
 * 
 * Common Function List:
 * Count()                          :int               Count how many Pokemon are in the party                        :Number of Pokemon.
 * HasPokemon()    					:bool              Has atleast one Pokemon to select                              :Returns the quantity left over.				
 * GetSlots()                       :List<Slot>        Get the list of each Slot of a Pokemon                         :List of Slots.
 * GetSlot(index)                   :Slot              Get a specific slot by index position.                         :Slot.
 * HasSlot(index)                   :bool              Check if there's a slot at index position.                     :Whether Pokemon is in slot.
 * GetSelected()                    :Slot              Get the slot associated with the active selection.             :Retrieve the slot for Poke ball.
 * IsSelected(pokemon)              :Pokemon           Check if a Pokemon is currently active.                        :Whether Pokemon is selected.
 * HasCapacity()                    :bool              Check if there's room in the party for another Pokemon.        :Whether there's room for another Pokemon.
 * AddPokemon(pokemon)              :bool              Add a Pokemon to the party.                                    :Success of adding a Pokemon.
 * Remove(index)                    :void              Remove a Pokemon from the party.                               :N/A.
 * Select(index)                    :Slot              Select a Pokemon slot in party.                                :Success of finding and selecting a Pokemon.
 * SelectNext()                     :Slot              Select the next occupied slot. Loops.                          :Occupied slot.
 * SelectPrev()                     :Slot              Select the previous occupied slot. Loops.                      :Occupied slot.
 * Swap(index1, index2)             :void              Swap two Pokemon slot positions.                               :N/A
 * GetPokemon(pokemon id)           :Pokemon           Retrieve a Pokemon with a certain id.                          :Pokemon with specified id.
 * ReleaseSelected()                :Pokeball          Will release active Pokeball and retrieve active Pokemon.      :Pokemon released
 * CaptureActive()                  :Pokemon           Will capture a release Pokemon, back into their ball.          :Pokemon captured
 * GetActivePokemon()               :Pokemon           Get the released Pokemon instance.                             :Pokemon released
 * IsActive(pokemon)                :bool              Verifies if a Pokemon has been released.                       :Release status of given Pokemon
 * HasActive()                      :bool              Checks if a Pokemon is released.                               :Whether a Pokemon has been released
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PokeParty
{
	const int PARTY_MAX = 10;

	Trainer trainer; //Enables it to be usable by any trainer in multiplayer (independant)
	List<Slot> slots;
	int selected;

	public Pokemon active; //Pokemon out of Pokeball

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

	public bool HasSlot(int index) {
		if (GetSlot(index) == null)
			return false;

		return true;
	}

	public Slot GetSelected() {
		if (selected == -1)
			return null;

		var slot = GetSlot(selected);
		return (slot != null) ? slot : null;
	}
	
	public Pokemon GetSelectedPokemon() {
		var slot = GetSelected();
		return (slot != null) ? slot.pokemon : null;
	}
	
	public bool IsSelected(Pokemon pokemon) {
		return GetSelectedPokemon() == pokemon;
	}
	
	public bool HasCapacity() {
		return Count() + 1 < PARTY_MAX;
	}
	
	public bool AddPokemon(Pokemon pokemon) {
		if (!HasCapacity())
			return false;

		var slot = new Slot(this, pokemon);
		GetSlots().Add(slot);

		if (selected == -1)
			Select(slot.index); //Select by default if no pokemon is selected

		return true;
	}

	public void Remove(int index) {
		if (!HasSlot(index)) {
			if (System.Diagnostics.Debugger.IsAttached)
				throw new Exception(String.Format("Error: Slot '{0}' has is empty.", index));
			else
				return; //Ignore the action
		}

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
		return Select(++selected % Count());  //Loop slot index when beyond bounds
	}
	
	public Slot SelectPrev() {
		return Select((--selected < 0) ? Count() - 1 : selected);
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

	public Pokeball ReleaseSelected() {
		CaptureActive(); //Capture if a Pokemon is already out

		var release = GetActivePokemon();
		if (release == null) {
			var selected = GetSelectedPokemon();
			if (selected != null) {
				if (!selected.thrown) {
					selected.thrown = true;

					var transform = trainer.GetTrainerBaseObj().transform;
					var ball = trainer.Instantiate(Resources.Load("Pokeball")).GetComponent<Pokeball>();

					ball.transform.position = transform.position;
					ball.rigidbody.AddForce((transform.forward * 2 + transform.up) * 400);

					ball.pokemon = selected;
					ball.trainer = trainer;
					//gamegui.SetChatWindow(ball.GetComponent<Pokeball>().pokemon.GetName() + "! I choose you!");

					active = selected;
					return ball;
				}
			}
		}

		return null;
	}

	public Pokemon CaptureActive() {
		var capture = GetActivePokemon();
		if (capture != null) {
			capture.obj.Return();
			active = null;

			return capture;
		}

		return null;
	}

	public Pokemon GetActivePokemon() {
		if (active == null || (active != null && active.obj == null))
			return null;

		return active;
	}

	public bool IsActive(Pokemon pokemon) {
		return (active != null) ? active == pokemon : false;
	}

	public bool HasActive() {
		return GetActivePokemon() != null;
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

