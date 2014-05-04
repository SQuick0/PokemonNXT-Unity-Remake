
using UnityEngine;
using System.Collections;

public class BattleGUI : MonoBehaviour {
	public int number = 0;
	public int level = 0;
	public int health = 0;
	public int damage= 0;
	public string moveCast;
	public int currentHealth = 0;
	public PokemonObj pokemonObj;

	/*
	public BattleGUI(PokemonObj pokemonObj){
		
		this.pokemonObj = pokemonObj;
		pokemonObj.pokemon.pp = 1;
	}
	*/

	public static BattleGUI CreateBattleGUI (GameObject where, PokemonObj pokeobjtmp) {
		BattleGUI myC = where.AddComponent<BattleGUI>();
		myC.pokemonObj = pokeobjtmp;
		return myC;
	}

	//void Update(){
	void OnGUI() {
		if (pokemonObj != null) {
			number = pokemonObj.pokemon.number;
			level = pokemonObj.pokemon.level;
			currentHealth = (int)pokemonObj.pokemon.currentHealth;
			health = (int)pokemonObj.pokemon.health;
			damage = (int)pokemonObj.pokemon.damage;
		}
	}	
	
	public void ToggleHud(){
		if (pokemonObj != null) {
			Vector2 xy;
			xy = Camera.main.WorldToScreenPoint (pokemonObj.transform.position);
			float x = xy.x;
			float y = Screen.height - xy.y;
			GUImgr.DrawBar (new Rect (x - 40, y - 69, 85, 8), pokemonObj.pokemon.hp, GUImgr.hp);   // Draw HP bar
			GUImgr.DrawBar (new Rect (x - 40, y - 61, 85, 8), pokemonObj.pokemon.pp, GUImgr.pp);	  // Draw PP bar
			//GUI.Label(new Rect(x-40,y-80 ,200,15), "HP: " + currentHealth.ToString()+"/" +  health.ToString());
			if (pokemonObj.enemy && pokemonObj.enemy.pokemon != null) {
					moveCast = pokemonObj.enemy.pokemon.moveCast;
					damage = (int)pokemonObj.enemy.pokemon.damage;
					Vector2 xy1;
					xy1 = Camera.main.WorldToScreenPoint (pokemonObj.enemy.transform.position);
					float x1 = xy1.x;
					float y1 = Screen.height - xy1.y; //
					GUImgr.DrawBar (new Rect (x1 - 40, y1 - 60, 85, 8), pokemonObj.enemy.pokemon.hp, GUImgr.hp); //Draw Enemy HP Bar
					GUI.Label (new Rect (x1 + 70, y1 - 100, 250, 40), damage.ToString () + " damage taken from " + moveCast + " !");

			}
		}
	}

	public void EnemyTargetWindow(Pokemon pokemon) {
		float tmpPokeHp = pokemon.hp;
		if (tmpPokeHp <= 0)
						tmpPokeHp = 0;
		float ypos = 5;
		float xpos = (Screen.width / 2) - 100;
		GUI.DrawTexture(new Rect(xpos,ypos,200,60), GUImgr.gradRight);
		GUI.Label(new Rect(xpos,ypos,200,20), pokemon.name + " lvl" + pokemon.level.ToString());
		ypos+=20;
		GUI.Label(new Rect(xpos,ypos,200,20), "HP");
		GUImgr.DrawBar(new Rect(xpos+35,ypos+5,200,10), tmpPokeHp, GUImgr.hp);
	}
	
}

