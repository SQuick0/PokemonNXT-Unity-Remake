/*
 * PartyMenuPopup
 * This will handle everything that happens when a user right clicks on a pokemon in their party window.
 * 
 * TO DO!
 * 	build menu list into popup
 * 
 * 
 * BUGS!
 * 	my computer is crappy and runs slow, but i think these popups might eat a silly amount of memory
 * 
*/

using UnityEngine;
using System.Collections;

public class PartyMenuPopup : MonoBehaviour {
	public bool showMenu{get;set;}
	public Pokemon pokemon{get;set;}
	GUIStyle label;
	public float mouseX;
	public float mouseY;

	void Start() {
		label = new GUIStyle();
		label.fontSize = 12;
		label.fontStyle = FontStyle.Bold;
		//pokemon = GetComponent<PartyMenu> ().pokemon;
	}
	
	//This should handle the menu drawing and labels
	void OnGUI() {
		GUI.depth = 0;	//this puts menu over the rest of the GUI
		if (showMenu && pokemon != null) {
			GUI.DrawTexture(new Rect(mouseX,mouseY,100,100), GUImgr.gradRight);
			GUI.Label(new Rect((mouseX+5), (mouseY+5), 40, 25), pokemon.name, label);
			GUI.Label(new Rect((mouseX+5), (mouseY+15), 40, 25), "menu will go here", label);
		}
	}
}