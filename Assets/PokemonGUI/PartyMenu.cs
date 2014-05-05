/*
 * PartyMenu
 * This will draw the pokemon party window, including various stats.
 * 
 * 
 * BUGS!
 * 	I think this or partymenupopup eats memory
 * 
*/

using UnityEngine;
using System.Collections;

public class PartyMenu : MonoBehaviour {
	public Pokemon pokemon;
	PartyMenuPopup partyMenuPopup;
	Rect rect;
	bool click;

	void Start(){
		GUImgr.Start();
		if (this.GetComponent<PartyMenuPopup> () != null) {
			Debug.Log("partymenu != null");
			partyMenuPopup = gameObject.GetComponent<PartyMenuPopup>();
		} else {
			Debug.Log("partymenu else");
			partyMenuPopup = gameObject.AddComponent<PartyMenuPopup> ();
		}
	}

	//This should handle keys and clicking
	void Update() {
		if (partyMenuPopup.showMenu && Input.GetKeyUp (KeyCode.Escape) || Input.GetMouseButtonUp (0)) {
			partyMenuPopup.showMenu = false;
			click = false;
		}
		if (Input.GetMouseButtonDown(1) && !click){ // when button clicked...
			click = true;
			// if right clicking on a party menu item
			//if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<PartyMenu>() != null) {
			if (rect != null && rect.Contains(Event.current.mousePosition)) {
				partyMenuPopup.mouseX = Event.current.mousePosition.x;
				partyMenuPopup.mouseY = Event.current.mousePosition.y;
				partyMenuPopup.pokemon = pokemon;
				partyMenuPopup.showMenu = true;
			}
		}
	}
	
	public void AddSlot(Pokemon thisPokemon, float yPos) {
		pokemon = thisPokemon;
		var label = new GUIStyle();
		label.fontSize = 12;
		label.fontStyle = FontStyle.Bold;
		GUI.DrawTexture (new Rect(0, yPos, 72, 70), GUImgr.statOk);
		if (pokemon.icon != null) {
			GUI.DrawTexture (new Rect (0, yPos, 48, 48), pokemon.icon);
		}
		GUI.Label(new Rect(36, yPos, 40, 25), "Lv: "+pokemon.level, label);
		GUImgr.DrawBar (new Rect (0, yPos + 53, 48, 4), pokemon.xp, GUImgr.xp, true);
		GUImgr.DrawBar (new Rect (53, yPos + 20, 4, 36), pokemon.pp, GUImgr.pp, false);
		GUImgr.DrawBar (new Rect (58, yPos + 20, 4, 36), pokemon.hp, GUImgr.hp, false);
		rect = new Rect (0, yPos, 72, 70);
	}
}