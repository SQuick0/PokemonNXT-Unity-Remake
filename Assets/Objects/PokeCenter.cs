using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PokeCenter : MonoBehaviour {
	public static void HealPokemon() {
		var party = Player.trainer.party;
		foreach (var slot in party.GetSlots()) {
			var pokemon = slot.pokemon;

			//pokemon.hp = pokemon.health;
			pokemon.hp = 1;
		}
	}
}