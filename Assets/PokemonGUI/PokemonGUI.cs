using UnityEngine;
using System.Collections;


public class PokemonGUI: MonoBehaviour {
	
	public bool HpBarToggle{get;set;}
	BattleGUI battleGUI;
	PokemonObj pokemonObj;
	//GameGUI gamegui;
	void Awake(){


	}

	void Start() {
		battleGUI = gameObject.AddComponent<BattleGUI> ();
		//gamegui = gameObject.GetComponent<GameGUI>();
	}

	void OnGUI() {
		if (HpBarToggle) {
			battleGUI.pokemonObj = Player.pokemon.obj;
			battleGUI.ToggleHud ();
		} else {
			Destroy(battleGUI);
		}
	}
	
	
	
	public void FloatingHPbar() {
		
		//GUI.Label (new Rect (x - 40, y - 200, 200, 20), "HP: " + currentHealth.ToString () + "/" + health.ToString ());
		
	}
	
}