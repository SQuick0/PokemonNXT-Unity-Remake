using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trainer : MonoBehaviour {
	public PokeParty party;
	public List<DataItem> inventory = new List<DataItem>();

	Vector3 velocity = Vector3.zero;

	void Start(){
		party = new PokeParty(this);

		//kanto starters, why not
		party.AddPokemon(new Pokemon(1, true));
		party.AddPokemon(new Pokemon(4, true));
		party.AddPokemon(new Pokemon(7, true));
		Pokedex.states [1] = Pokedex.State.Captured;
		Pokedex.states [4] = Pokedex.State.Captured;
		Pokedex.states [7] = Pokedex.State.Captured;

		inventory.Add (new DataItem(ItemTypes.Pokeball, 5));
		inventory.Add (new DataItem(ItemTypes.Potion, 2));
		//inventory.Add(2, 1); //New inventory code references shared item data. (id, quantity)
		//inventory.Add(5, 1);
	}

	void Update(){
		//inventoryMGR
		for(int i=0; i<inventory.Count; i++){
			if (inventory[i].id<=0)	inventory.Remove(inventory[i]);
		}
	}

	public void ThrowPokemon(Pokemon poke){
		if (poke.thrown)	return;
		poke.thrown = true;
		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		ball.transform.position = transform.position;
		ball.rigidbody.AddForce( (transform.forward*2+ transform.up)*400 );
		ball.GetComponent<Pokeball>().pokemon = poke;
		ball.GetComponent<Pokeball>().trainer = this;
		//gamegui.SetChatWindow(ball.GetComponent<Pokeball>().pokemon.GetName() + "! I choose you!");
	}

	public void SetVelocity(Vector3 vel){
		velocity = vel;
		Animator ani = GetComponent<Animator>();

		if (vel.magnitude>0.1f){
			ani.SetBool("run",true);
			transform.rotation = Quaternion.LookRotation(vel);
		}else{
			ani.SetBool("run",false);
		}
	}
}