/*
 * Due to the excessive interface hardcoding, we may need to integrate a real interface (DFGUI? costs, or wait till Unity 5 UI?). DFGUI has a built in form editor too.
 */

using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	public static bool menuActive = false;
	public static bool chatActive=false;
	public static bool dataWindow=false;
	public static string addToChat;
	public static ArrayList chatHistory = new ArrayList();
	int pokedexEntery = 1;
	enum MenuWindows{None, Multiplayer, Pokedex, Pokemon, Inventory, Talents, Options, Quit};
	MenuWindows currentWindow = MenuWindows.None;

	void Start(){
		GUImgr.Start();
	}

	void OnGUI(){
		GUI.skin.label.fontSize = 15;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.label.normal.textColor = Color.black;

		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		float ypos = 0;

		if (Dialog.inDialog){
			Dialog.GUIWindow();
			return;
		}

		Dialog.doneDialog = false;

		if(chatActive){
			OpenChatWindow();
		}

		if (Player.pokemonActive && Player.pokemon.obj!=null){
			Player.pokemon.obj.GetComponent<PokemonDomesticated>().BattleGUI();
			return;
		}
		
		if (menuActive){
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			ypos = 0;
			GUI.DrawTexture(new Rect(Screen.width-100,0,150,Screen.height), GUImgr.gradLeft);
			for(int i=0; i<8; i++){
				if ((int)currentWindow==i && i>0)
					GUI.DrawTexture(new Rect(Screen.width-120,ypos+5,150,15), GUImgr.gradLeft);
				if (mx>Screen.width-200 && my>ypos && my<ypos+25){
					GUI.DrawTexture(new Rect(Screen.width-120,ypos+5,150,15), GUImgr.gradLeft);
					if (Input.GetMouseButton(0) && !Player.click){
						Player.click = true;
						if (i==0)
							menuActive = false;
						else
							currentWindow=(MenuWindows)i;
					}
				}
				if (i==0)
					GUI.Label(new Rect(Screen.width-210,ypos,200,25), "Close");
				else
					GUI.Label(new Rect(Screen.width-210,ypos,200,25), ((MenuWindows)i).ToString());
				ypos+=25;
			}

			string timeTxt = TimeMgr.hour.ToString()+":";
			if (TimeMgr.minuite<10){
				timeTxt+="0"+((int)TimeMgr.minuite).ToString();
			}else{
				timeTxt+=((int)TimeMgr.minuite).ToString();
			}
			GUI.Label(new Rect(Screen.width-210,Screen.height-25,200,25), timeTxt);

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			
			switch(currentWindow){
			case MenuWindows.Multiplayer:	MultiplayerWindow();	break;
			case MenuWindows.Pokedex:		PokedexWindow();		break;
			case MenuWindows.Pokemon:		PokemonWindow();		break;
			case MenuWindows.Inventory:		InventoryWindow();		break;
			case MenuWindows.Talents:		TalentsWindow();		break;
			case MenuWindows.Options:		OptionsWindow();		break;
			case MenuWindows.Quit:			QuitWindow();			break;
			}
			return;
		}
		
		ypos = 0;
		var party = Player.trainer.party;
		/*foreach(var slot in Player.trainer.party.GetSlots()){
			var pokemon = slot.pokemon;

			if (party.IsActive(pokemon)){
				GUI.DrawTexture(new Rect(0,ypos+16,100,32), GUImgr.gradRight);
				//this.OpenStatWindow (pokemon);
				this.OpenStatWindow (pokemon);
			}

			GUI.DrawTexture(new Rect(0,ypos,64,64), pokemon.icon);
			GUI.Label(new Rect(64,ypos,200,25), pokemon.name+" lvl"+pokemon.level.ToString());
			//changed to do tests.  DO NOT COMMIT
			GUImgr.DrawBar(new Rect(64,ypos+25,100,5), pokemon.hp, GUImgr.hp, false);
			GUImgr.DrawBar(new Rect(64,ypos+35,100,5), pokemon.xp, GUImgr.xp);
			ypos += 70;
		}
		*/
		//New Age UI
		foreach (var slot in Player.trainer.party.GetSlots()) {
			var pokemon = slot.pokemon;
			var label = new GUIStyle();
			label.fontSize = 12;
			label.fontStyle = FontStyle.Bold;

			if (party.IsActive (pokemon)) {
				//GUI.DrawTexture (new Rect (0, ypos + 16, 100, 64), GUImgr.gradRight);
				this.OpenStatWindow (pokemon);
			}
			GUI.DrawTexture (new Rect(0, ypos, 72, 70), GUImgr.statOk);
			GUI.DrawTexture(new Rect(0, ypos, 48, 48), pokemon.icon);
			GUI.Label(new Rect(36, ypos, 40, 25), "Lv: "+pokemon.level, label);
			GUImgr.DrawBar (new Rect (0, ypos + 53, 48, 4), pokemon.xp, GUImgr.xp, true);
			GUImgr.DrawBar (new Rect (53, ypos + 20, 4, 36), pokemon.pp, GUImgr.pp, false);
			GUImgr.DrawBar (new Rect (58, ypos + 20, 4, 36), pokemon.hp, GUImgr.hp, false);
			ypos += 70;
		}
	
	}

	
	//Creates a box for a Pokemon Overview.  Currently shows a base stat overview for the current
	//selected pokemon.  Having trouble converting out move and item names for the overview.
	//Currently has hard coded names for items/moves to test UI spacing.
	public void OpenStatWindow(Pokemon pkmn) {
		if (dataWindow) {
			GUI.DrawTexture (new Rect (Screen.width - 275, Screen.height - 250, 250, 250), GUImgr.gradRight);
			GUI.Label (new Rect (Screen.width - 270, Screen.height - 245, 200, 25), pkmn.GetName ());
			GUI.Label (new Rect (Screen.width - 270, Screen.height - 215, 75, 25), "HP: " + pkmn.CurrentHP () + "/" + pkmn.TotalHP ());
			GUI.Label (new Rect (Screen.width - 270, Screen.height - 185, 75, 25), "Atk: " + pkmn.TotalAttack ());
			GUI.Label (new Rect (Screen.width - 270, Screen.height - 155, 75, 25), "Def: " + pkmn.TotalDefence ());
			GUI.Label (new Rect (Screen.width - 270, Screen.height - 125, 75, 25), "Spd: " + pkmn.TotalSpeed ());
			//As far as I can tell, held items aren't implemented, so I'm just hardcoding None.
			GUI.Label (new Rect (Screen.width - 195, Screen.height - 215, 190, 25), "Item: None" /*+ pokemon.GetItemName()*/);
			int height = 185;
			int loop = 1;
			foreach (Move mve in pkmn.moves) {
				if (loop > 4)
					break;
				GUI.Label (new Rect (Screen.width - 195, Screen.height - height, 190, 25), "Move " + loop + ": " + mve.ToFriendlyString ());
				loop++;
				height -= 30;
			}
		}
	}
	
	void MultiplayerWindow(){
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;

		float ypos = 0;
		GUI.DrawTexture(new Rect(0,ypos,300,200), GUImgr.gradRight);

		ypos+=20;
		if (Network.peerType==NetworkPeerType.Disconnected){
			GUI.Label(new Rect(20, ypos, 200,25), "Not connected");
		}
	}

	public void OpenChatWindowa() {
		int bottomLeftX = 0;
		int errorHeight = 300;
		int bottomLeftY = Screen.height - errorHeight;
		int screenWidth = Screen.width;
		GUI.DrawTexture(new Rect(bottomLeftX,bottomLeftY,screenWidth,errorHeight), GUImgr.gradRight);
		//GUI.Label(new Rect(bottomLeftX,(bottomLeftY-(errorHeight/2)+GUI.skin.label.fontSize),screenWidth,errorHeight), addToChat);
		for (int x=0;x<chatHistory.Count;x++) {
			int linePosition = GUI.skin.label.fontSize;
			if(x>0){linePosition=GUI.skin.label.fontSize*x;}
			string tmpChat = chatHistory[x].ToString();
			GUI.Label(new Rect(bottomLeftX,(bottomLeftY-(errorHeight/2)+linePosition),screenWidth,errorHeight), tmpChat);
		}
	}
	
	private Vector2 scrollPosition;
	public void OpenChatWindow() {
		int bottomLeftX = 0;
		int errorHeight = 300;
		int bottomLeftY = Screen.height - errorHeight;
		int screenWidth = Screen.width;
		GUI.DrawTexture(new Rect(bottomLeftX,bottomLeftY,screenWidth,errorHeight), GUImgr.gradRight);
		GUILayout.BeginArea(new Rect(bottomLeftX+10,bottomLeftY+10,screenWidth-20,errorHeight-20));
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (Screen.width-100), GUILayout.Height (Screen.height-100));
		GUI.skin.box.wordWrap = true;
		GUILayout.Box(addToChat);
		GUILayout.EndScrollView ();
		GUILayout.EndArea();
	}
	public void SetChatWindow(string toChat) {
		addToChat = addToChat + "\n" + toChat;
		if (chatHistory.Count > 10) {
			chatHistory.Remove (0);
		}
		chatHistory.Add (toChat);
		//scrollPosition = new Vector2(0, Mathf.Infinity);
		scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);
	}
	
	void PokedexWindow(){
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		int displayN = Screen.height/64;
		float ypos = 0;
		for(int i=pokedexEntery-displayN/2; i<=pokedexEntery+displayN/2; i++){
			int entry = i;
			if (entry<1)	entry += Pokedex.states.Length-1;
			if (entry>Pokedex.states.Length-1)	entry -= Pokedex.states.Length-1;
			
			if (entry==pokedexEntery){
				GUI.DrawTexture(new Rect(0,ypos+16,100,32), GUImgr.gradRight);
			}
			if (mx<100 && my>ypos && my<ypos+64){
				GUI.DrawTexture(new Rect(0,ypos+16,100,32), GUImgr.gradRight);
				if (!Player.click && Input.GetMouseButton(0)){
					Player.click = true;
					pokedexEntery = entry;
				}
			}
			string numberText = entry.ToString();
			if (entry<100)	numberText = "0"+numberText;
			if (entry<10)	numberText = "0"+numberText;
			
			if (Pokedex.states[entry]==Pokedex.State.Unknown)
				GUI.Label(new Rect(64,ypos,200,25), "#"+numberText+" ? ? ? ? ? ? ? ? ?");
			else{
				GUI.Label(new Rect(64,ypos,200,25), "#"+numberText+" "+Pokemon.GetName(entry));
			}
			ypos += 64;
		}
		
		if (Pokedex.states[pokedexEntery]==Pokedex.State.Captured){
			GUI.Label(new Rect(250,0,Screen.width-400,Screen.height), Pokedex.PokeDexText(pokedexEntery));
		}
	}
	
	void PokemonWindow(){
		var party = Player.trainer.party;
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		float xpos = Screen.width/2 - party.Count()*64/2;
		float swapXPos = 430;
		Pokemon pokemon;
		int curSlot = 0;
		
		foreach(var slot in party.GetSlots()){
			pokemon = slot.pokemon;
			
			if (party.IsActive(pokemon)) {
				GUI.DrawTexture(new Rect(xpos+16,0,32,50), GUImgr.gradDown);
				curSlot = slot.index;
			}
			/*else {
				GUI.DrawTexture (new Rect(swapXPos, Screen.height/4, 64, 64), GUImgr.statOk);
				//GUI.DrawTexture(new Rect(swapXPos,Screen.height/4, 64, 64), pokemon.icon);
				loop++;
				if (GUI.Button(new Rect(swapXPos,Screen.height/4,64,64), pokemon.icon)) {
					party.Swap (loop, curSlot);
				}
				swapXPos+=70;
			}*/
			
			if (my<64 && mx>xpos && mx<xpos+64){
				GUI.DrawTexture(new Rect(xpos+16,0,32,50), GUImgr.gradDown);
				if (Input.GetMouseButton(0) && !Player.click){
					Player.click = true;
					party.Select(slot.index);
					if (Player.pokemon.obj!=null){
						Player.pokemon.obj.GetComponent<PokemonObj>().Return();
						Player.trainer.ThrowPokemon(pokemon);
					}
				}
			}

			GUI.DrawTexture(new Rect(xpos,0,64,64), pokemon.icon);
			xpos+=64;

		}

		//I really HATE that I have to do a double loop here to do my swap buttons, but I don't have
		//a good way of making sure that my active pokemon is set before I make the buttons otherwise.
		//Purpose: To allow for a quick swap of the party, by simply clicking a member portrait while
		//looking at the party member you want to switch.
		GUI.DrawTexture (new Rect (330, Screen.height / 8, 100 + 70*(party.Count () - 1), 70), GUImgr.bckDrop);
		GUI.Label (new Rect (330, Screen.height / 8, 150, 25), "Quick Swap: ");
		foreach (var slot in party.GetSlots()) {
			pokemon = slot.pokemon;
			if (!party.IsActive(pokemon)) {
				GUI.DrawTexture (new Rect (swapXPos, 1* Screen.height / 8, 64, 64), GUImgr.statOk);
				//GUI.DrawTexture(new Rect(swapXPos,Screen.height/4, 64, 64), pokemon.icon);
				if (GUI.Button (new Rect (swapXPos, 1* Screen.height / 8, 64, 64), pokemon.icon)) {
					party.Swap (slot.index, curSlot);
				}
				swapXPos += 70;
			}
		}
		
		if (Player.pokemon!=null){
			Pokemon poke = Player.pokemon;
			float ypos = 70;
			GUI.DrawTexture(new Rect(0,ypos,300,200), GUImgr.gradRight);
			ypos+=20;
			GUI.Label(new Rect(20, ypos, 200,25), poke.name);
			GUI.Label(new Rect(150, ypos, 200,25), "HP");
			GUImgr.DrawBar(new Rect(175,ypos+10,100,5), poke.hp, GUImgr.hp);
			ypos+=20;
			string numberText = poke.number.ToString();
			if (poke.number<100)	numberText = "0"+numberText;
			if (poke.number<10)	numberText = "0"+numberText;
			GUI.Label(new Rect(20, ypos, 200,25), "#"+numberText+" "+Pokemon.GetName(poke.number));
			GUI.Label(new Rect(150, ypos, 200,25), "XP");
			GUImgr.DrawBar(new Rect(175,ypos+10,100,5), poke.xp, GUImgr.xp);
			ypos+=50;
			
			GUI.Label(new Rect(20, ypos, 200,25), "Health "+poke.health.ToString());
			GUI.Label(new Rect(150, ypos, 200,25), "Speed "+poke.speed.ToString());
			ypos+=20;
			GUI.Label(new Rect(20, ypos, 200,25), "Attack "+poke.attack.ToString());
			GUI.Label(new Rect(150, ypos, 200,25), "Defence "+poke.defence.ToString());

			ypos+=20;
			if (poke.heldItem!=null){
				GUI.Label(new Rect(20, ypos, 200,25), poke.heldItem.data.name);
			}
		}
	}
	
	void InventoryWindow(){
		GUI.DrawTexture(new Rect(0,0,100,Screen.height), GUImgr.gradRight);
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		float ypos = 0;
		foreach(var item in Player.trainer.inventory.GetItems()){
			if (item.id == Player.trainer.item.id)	GUI.DrawTexture(new Rect(0,ypos+8,150,16), GUImgr.gradRight);
			if (mx<100 && my>ypos && my<ypos+30){
				GUI.DrawTexture(new Rect(0,ypos+8,150,16), GUImgr.gradRight);
				if (Input.GetMouseButton(0) && !Player.click){
					Player.click = true;
					Player.trainer.inventory.Select(item);
				}
			}
			GUI.DrawTexture(new Rect(0,ypos,32,32), item.data.icon);
			if (item.id>1)
				GUI.Label(new Rect(32,ypos+5,100,25), item.data.name+" x"+item.id.ToString());
			else
				GUI.Label(new Rect(32,ypos+5,100,25), item.data.name);
			ypos+=30;
		}

		if (Player.trainer.item != null){
			ypos = 0;
			float width = Screen.width-400;
			GUI.DrawTexture(new Rect(180,-50,width+40,200), GUImgr.gradDown);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			if (my<25){
				if (mx>200 && mx<200+width/3){
					GUI.DrawTexture(new Rect(200,0,width/3,25), GUImgr.gradDown);
					if (Input.GetMouseButton(0)	&& !Player.click){
						Player.click = true;
						Player.trainer.item.Use();
					}
				}
				if (mx>200+width/3 && mx<200+2*width/3){
					GUI.DrawTexture(new Rect(200+width/3,0,width/3,25), GUImgr.gradDown);
				}

				//Is this drop? Too much hardcoding
				if (mx>200+2*width/3 && mx<200+width){
					GUI.DrawTexture(new Rect(200+2*width/3,0,width/3,25), GUImgr.gradDown);
					if (Input.GetMouseButton(0)	&& !Player.click){
						Player.click = true;
						Player.trainer.inventory.Remove(Player.trainer.item, 1);
					}
				}
			}
			
			GUI.Label(new Rect(200,ypos,width/3,25), "Use");
			GUI.Label(new Rect(200+width/3,ypos,width/3,25), "Hold");
			GUI.Label(new Rect(200+2*width/3,ypos,width/3,25), "Drop");
			
			ypos += 25;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(200,ypos,width,50),Player.trainer.item.data.description);
		}
	}
	
	void TalentsWindow(){
	}
	
	void OptionsWindow(){
	}
	
	void QuitWindow(){
		float mx = Input.mousePosition.x;
		float my = Screen.height-Input.mousePosition.y;
		float width = Screen.width-400;
		GUI.DrawTexture(new Rect(180,-50,width+40,100), GUImgr.gradDown);
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		
		if (my<25){
			if (mx>200 && mx<200+width/2){
				GUI.DrawTexture(new Rect(200,0,width/2,25), GUImgr.gradDown);
				if (Input.GetMouseButton(0)	&& !Player.click){
					Player.click = true;
					Application.LoadLevel("Menu");
				}
			}
			if (mx>200+width/2 && mx<200+width){
				GUI.DrawTexture(new Rect(200+width/2,0,width/2,25), GUImgr.gradDown);
				if (Input.GetMouseButton(0)	&& !Player.click){
					Player.click = true;
					currentWindow = MenuWindows.None;
				}
			}
		}
		
		GUI.Label(new Rect(200,0,width/2,25), "Quit");
		GUI.Label(new Rect(200+width/2,0,width/2,25), "Cancel");
		GUI.Label(new Rect(200,25,width,25), "Are you sure you want to quit?");
		
		PlayerPrefs.Save();
	}
}