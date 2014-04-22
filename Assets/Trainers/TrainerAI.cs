using UnityEngine;
using System.Collections;

public class TrainerAI : MonoBehaviour {
	Trainer trainer = null;
	Trainer enemyTrainer = null;

	Pokemon currentPokemon = null;
	Vector3 trainerPos = Vector3.zero;
	enum States {Idle, InBattle, DefeatText, LooseText, Defeated};
	States currentState = States.Idle;

	void Start(){
		trainer = GetComponent<Trainer>();
	}

	void Update(){
		if (Player.trainer==this)	return;
		
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
			break;}
			
		case States.InBattle:	InBattle();	break;

		case States.DefeatText:{
			Dialog.inDialog = true;
			Dialog.NPCobj = gameObject;
			Dialog.NPCname = "Young Trainer";
			Dialog.text = "Damn. I guess you're a much better trainer that I am.";
			if (Dialog.doneDialog){
				Dialog.inDialog = false;
				currentState = States.Defeated;
			}
			break;}
			
		}
	}
	
	void InBattle(){
		//move trainer to position
		Vector3 direct = trainerPos-transform.position;
		direct.y = 0;
		if (direct.sqrMagnitude>2){
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", true);
		}else{

			//if in position
			if (direct.sqrMagnitude>1)	transform.position += direct;
			//currentPokemon = trainer.party.GetActivePokemon();

			if (currentPokemon.obj!=null){
				direct = currentPokemon.obj.transform.position-transform.position;
			}else{
				direct = enemyTrainer.transform.position-transform.position;
			}
			direct.y = 0;
			transform.rotation = Quaternion.LookRotation(direct);
			GetComponent<Animator>().SetBool("run", false);
			GetComponent<Animator>().SetBool("cheer",true);

			if (currentPokemon==null){//Only 1 pokemon is throwable
				currentPokemon = null;
				for(int i=0; i<trainer.party.Count(); i++){
					if (trainer.party.GetSlot(i).pokemon.hp>0){
						currentPokemon = trainer.party.GetSlot(i).pokemon;
						trainer.ThrowPokemon(currentPokemon);
						break;
					}
				}
				if (currentPokemon==null){
					currentState = States.DefeatText;
					GetComponent<Animator>().SetBool("cheer",false);
					GetComponent<Animator>().SetBool("applause",true);
				}
			}else{

				//give orders to the pokemon!
				if (enemyTrainer.party.GetActivePokemon().obj!=null){
					currentPokemon.obj.enemy = enemyTrainer.party.GetActivePokemon().obj;
					currentPokemon.obj.GetComponent<PokemonDomesticated>().currentOrder = PokemonDomesticated.Orders.Charge;
				}
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