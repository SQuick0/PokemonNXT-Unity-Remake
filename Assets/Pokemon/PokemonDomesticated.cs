using UnityEngine;
using System.Collections;

public class PokemonDomesticated : MonoBehaviour {
	public enum Orders {Heel, Idle, Charge}
	public Orders currentOrder = Orders.Heel;
	public Trainer trainer = null; //Will only cast to TrainerPlayer for now, will do have to a get object function specific to the MonoBehaviour TrainerPlayer and TrainerAI objects. (As they don't support multi-inheritance)
	
	bool letsGo = false;
	public PokemonObj pokemonObj;
	GameGUI gamegui = new GameGUI();

	void Start(){
		pokemonObj = GetComponent<PokemonObj>();
		pokemonObj.pokemon.pp = 1;
	}

	void Update(){
		var party = Player.trainer.party;
		var pokemon = party.GetActivePokemon();
		if (party.HasActive())	return; //Why the?

		switch(currentOrder){
		case Orders.Heel:{
			var trainerObj = trainer.GetTrainerBaseObj();
			Vector3 direct = trainerObj.transform.position - transform.position;
			direct.y = 0;
			if (letsGo){
				transform.rotation = Quaternion.LookRotation(direct);
				pokemonObj.SetVelocity(direct.normalized * pokemonObj.speed);
			}
			if (direct.sqrMagnitude<10)	letsGo = false;
			if (direct.sqrMagnitude>20)	letsGo = true;
			break;}
		}
	}
}