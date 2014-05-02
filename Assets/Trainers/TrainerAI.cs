using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainerAI : MonoBehaviour {
	Trainer trainer = null;
	Trainer enemyTrainer = null;

	Pokemon currentPokemon = null;
	Vector3 trainerPos = Vector3.zero;
	enum States {Idle, InBattle, Defeated, Inactive};
	States currentState = States.Idle;
	List<int> aiAlivePokemon = new List<int> ();

	void Start(){
		trainer = GetComponent<Trainer>();
	}

	void Update(){
		if (Player.trainer==this)	return;
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Dialog.doneDialog=true;
		}
		
		switch(currentState){
			
			case States.Idle:{
				Vector3 direct = Player.trainer.transform.position - transform.position;
				if (direct.sqrMagnitude<10*10 && Vector3.Dot(direct, transform.forward)>0){
					
					Dialog.inDialog = true;
					Dialog.NPCobj = gameObject;
					Dialog.NPCname = "Young Trainer";
					Dialog.text = "You're a pokemon trainer right? That means we have to battle!";
					if (Dialog.doneDialog){
						Dialog.inDialog = false;
						
						currentState = States.InBattle;
						trainerPos = transform.position - direct.normalized*10;
						enemyTrainer = Player.trainer;
					}
				}
				break;
			}
				
			case States.InBattle:	InBattle();	break;
			
			case States.Defeated: {
				Dialog.inDialog = true;
				Dialog.NPCobj = gameObject;
				Dialog.NPCname = "Young Trainer";
				Dialog.text = "You've beaten me!!";
				if (Dialog.doneDialog){
					Dialog.inDialog = false;
					currentState = States.Inactive;
				}
				break;
			}
			case States.Inactive: {
				//add timer here to reset AI state to idle after a period of time
				break;
			}
			
		}
	}
	
	void InBattle(){
		//move trainer to position
		Vector3 direct = trainerPos-transform.position;
		direct.y = 0;
		aiAlivePokemon.Clear ();
		if (direct.sqrMagnitude>2){
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", true);
		}else{
			for (int x = 0; x < trainer.party.GetSlots().Count; x++) {
				if (trainer.party.GetSlot(x).pokemon.hp > 0) {
					aiAlivePokemon.Add(x);
				}
				else if (aiAlivePokemon[x] != null) {
					aiAlivePokemon.Remove(x);
				}
			}
			if (direct.sqrMagnitude>1)	transform.position += direct;
			currentPokemon = trainer.party.GetActivePokemon();

			if (currentPokemon.obj!=null){
				direct = currentPokemon.obj.transform.position-transform.position;
			}else{
				direct = enemyTrainer.transform.position-transform.position;
			}
			direct.y = 0;
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", false);
			if (aiAlivePokemon.Count > 0) {
				if (currentPokemon.obj==null)	trainer.ThrowPokemon(trainer.party.GetSlot(aiAlivePokemon[0]).pokemon); //Only 1 pokemon is throwable
			}
			else {
				Debug.Log("ai defeated");
				currentState = States.Defeated;
			}
		}
		
		/*if (currentPokemonObj!=null){
			PokemonTrainer pokeComp = currentPokemonObj.GetComponent<PokemonTrainer>;
			if (pokeComp!=null){
				if (Player.pokemonObj!=null){
					pokeComp.AttackEnemy(Player.pokemonObj);
				}
			}
		}*/
	}
}