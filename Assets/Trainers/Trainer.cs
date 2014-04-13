using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trainer : MonoBehaviour { //Need to separate the Unity object from this object, like Pokemon has. (Target)
	public PokeParty party;
	public Inventory inventory;
	public Inventory.Item item {get{return inventory.selected;} set{}}

	Vector3 velocity = Vector3.zero;

	void Start(){
		party = new PokeParty(this);
		inventory = new Inventory(this);

		//kanto starters, why not
		party.AddPokemon(new Pokemon(1, true));
		party.AddPokemon(new Pokemon(4, true));
		party.AddPokemon(new Pokemon(7, true));
		Pokedex.states [1] = Pokedex.State.Captured;
		Pokedex.states [4] = Pokedex.State.Captured;
		Pokedex.states [7] = Pokedex.State.Captured;
		
		inventory.Add(1, 5); //New inventory code references shared item data. (id, quantity)
		inventory.Add(4, 2);
	}

	void Update(){
	}

	public Target.TARGETS GetTargetType() {
		return Target.TARGETS.TRAINER;
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