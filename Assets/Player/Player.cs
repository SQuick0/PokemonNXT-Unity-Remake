using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static bool click = false;
	static bool jumpCool = true;

	public static TrainerPlayer trainer = null;
	public static Pokemon pokemon {get{return trainer.party.GetActivePokemon();} set{}}

	public static GameGUI gamegui = new GameGUI();

	void Start(){
		//trainer = GameObject.Find("Player").GetComponent<Trainer>(); //Previously both the Unity Trainer object and the trainer class were combined
		//trainer = new TrainerPlayer();
		gameObject.AddComponent ("CameraControl");
	}

	void Update(){
		var party = trainer.party;

		//do nothing if in dialog
		if (Dialog.inDialog){
			Screen.lockCursor = false;
			Screen.showCursor = true;
			trainer.SetVelocity(Vector3.zero);
			return;
		}

		//menu
		if ((GameGUI.menuActive && !party.HasActive()) || CameraControl.releaseCursor) {
			Screen.lockCursor = false;
			Screen.showCursor = true;
		} else {
			Screen.lockCursor = true;
			Screen.showCursor = false;
		}

		//player control
		click = CanClick();
		if (party.HasActive()) {
			HandlePokemon();
		} else {
			HandleTrainer();
			//move trainer
			Vector3 vel = Quaternion.Euler(0,CameraControl.ay,0) * (Vector3.forward*Input.GetAxis("Vertical") + Vector3.right*Input.GetAxis("Horizontal"));
			trainer.SetVelocity(vel);
		}
	}

	public void HandlePokemon() {
		//move pokemon
		trainer.SetVelocity(Vector3.zero);
		
		Vector3 velocity = Vector3.zero;
		velocity += pokemon.obj.transform.forward * Input.GetAxis("Vertical");
		velocity += pokemon.obj.transform.right * Input.GetAxis("Horizontal");
		velocity *= pokemon.obj.speed;
		
		pokemon.obj.SetVelocity(velocity);
		pokemon.obj.transform.Rotate(pokemon.obj.transform.up, Input.GetAxis("Mouse X"));
		
		if(Input.GetButton("Jump") && jumpCool && Physics.Raycast(pokemon.obj.transform.position+Vector3.up*0.1f, Vector3.down, 0.2f)){
			pokemon.obj.rigidbody.AddForce(Vector3.up*3000);
			jumpCool = false;
		}
		if(!Input.GetButton("Jump"))	jumpCool = true;
		
		pokemon.pp -= Time.deltaTime/500;
		if (pokemon.pp <= 0){
			pokemon.obj.Return();
		}

		//throw pokemon
		if (!click && Input.GetKeyDown(KeyCode.Return)){
			trainer.RecallPokemon();
			click = true;
		}
	}

	public void HandleTrainer() {
		var party = trainer.party;

		//select pokemon
		if (!click && !trainer.party.HasActive()){
			for(int i = 1; i <= trainer.party.Count(); i++) {
				if (Rebind.GetInputDown("SELECT_POKE_PARTY_" + i))
					trainer.party.Select(i - 1);
			}
			
			if (Rebind.GetInputDown("SELECT_POKE_PREV"))
				trainer.party.SelectPrev();
			else if (Rebind.GetInputDown("SELECT_POKE_NEXT"))
				trainer.party.SelectNext();
		}

		//throw pokemon
		if (!click && Input.GetKeyDown(KeyCode.Return)){
			trainer.ThrowPokeball();
			click = true;
		}
		
		//activate menu
		if (Input.GetKeyDown(KeyCode.Escape) && !click){
			if (!party.HasActive())
				GameGUI.menuActive = !GameGUI.menuActive;

			click = true;
		}
		
		//capture pokemon
		if(Input.GetKeyDown("c")) {
			GameGUI gamegui = GetComponent<GameGUI>();
			CapturePokemon();
			click = true;
		}
		
		//chat window
		if(Input.GetKeyDown ("i")){
			if(GameGUI.chatActive)
				GameGUI.chatActive=false;
			else
				GameGUI.chatActive=true;
			
			click = true;
		}
		
		if (Input.GetKeyDown ("h")) {
			PokeCenter.HealPokemon ();
		}
		/*
	 * don't try using this right now, because it doesn't exist!
		if (Input.GetKeyDown ("k")) {
			Populate okasf = new Populate();
			okasf.Test();
		}
	*/
		//anticlick
		bool anti = false;
		for(int i = 1; i <= 10 && !anti; i++) {
			if (Rebind.GetInput("SELECT_POKE_PARTY_" + i))
				anti = true;
		}
	}

	public static bool CanClick() {
		bool anti = false;
		for(int i = 1; i <= 10 && !anti; i++) {
			if (Rebind.GetInput("SELECT_POKE_PARTY_" + i))
				anti = true;
		}

		return (!anti && !Rebind.GetInput("SELECT_POKE_PREV") && !Rebind.GetInput("SELECT_POKE_NEXT")
		    	&& !Rebind.GetInput("THROW_POKEBALL") && !Input.GetKey(KeyCode.Escape)
		        && !Input.GetMouseButton(0) && !Input.GetMouseButton(1));
	}

	public static void CapturePokemon(){
		/*
		GameGUI gamegui = new GameGUI();
		Debug.LogError("Capture Pokemon");
		Vector3 pokemonPositon = pokemonObj.transform.position;
		GameObject ball = (GameObject)Instantiate(Resources.Load("Pokeball"));
		//ball.transform.position = GameObject.Find("_PokeballHolder").transform.position;
		GameObject.Find ("_PokeballHolder").transform.LookAt(pokemonPositon);
		ball.transform.position = GameObject.Find ("_PokeballHolder").transform.position;
		//ball.rigidbody.AddForce
		//	( Camera.main.transform.forward*500+ Camera.main.transform.up*300 );
		ball.rigidbody.AddForce(pokemonPositon*500 + Camera.main.transform.up*300);
		Pokeball.CapturePokemon();
		Destroy (ball, 1);*/
	}
}