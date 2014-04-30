using UnityEngine;
using System.Collections;


public class PokemonGUI: MonoBehaviour {
	
	public static bool HpBarToggle = false;
	BattleGUI battleGUI;
	PokemonObj pokemonObj;
	//GameGUI gamegui = new GameGUI();
	void Awake(){
		pokemonObj = GetComponent<PokemonObj> ();
		battleGUI = new BattleGUI(pokemonObj);

	}
	void Update()
		
	{	
		
		
	}	
	void OnGUI()
	{	
		
		if(HpBarToggle){	
			
			battleGUI.ToggleHud();
		}
	}
	
	
	
	public void FloatingHPbar(){
		
		//GUI.Label (new Rect (x - 40, y - 200, 200, 20), "HP: " + currentHealth.ToString () + "/" + health.ToString ());
		
	}
	
}