/*
 * This class handles clicking and pressing Tab to target wild pokemon.
 * 
 * TO DO!
 * 	restrict attacks to only targeted pokemon.
 * 	tie into battleGUI.ToggleHud()
 * 
 * BUGS!
 * 	Target a pokemon, then attack a different pokemon, results in two target windows overlayed.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTarget : MonoBehaviour {
	private List<Transform> allPokemon = new List<Transform>();
	public Transform targetedPokemon{ get; set; }
	private Transform playerTransform;
	public bool activeTarget{ get; set; }
	public GameObject highlightSparkles;
	PokemonWild pokemonWild;
	Pokemon pokemon;
	BattleGUI battleGUI;
	int tabCount = 0;

	void Start() {
		allPokemon = new List<Transform>();
		targetedPokemon = null;
		battleGUI = gameObject.AddComponent<BattleGUI> ();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)){ // when button clicked...
			RaycastHit hit; // cast a ray from mouse pointer:
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// if enemy hit...
			if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("pokemon")){
				//UnHighlightTarget();
				TargetPokemon(hit.transform);
			}
		}

		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (activeTarget) {
				UnHighlightTarget();
			}
			TargetPokemon(FindNearestPokemon());
		}
		if (Input.GetKey(KeyCode.Escape) && activeTarget){
			activeTarget = false;
		}
	}

	void OnGUI() {
		if (activeTarget && targetedPokemon != null) {
			pokemonWild = targetedPokemon.GetComponent<PokemonWild>();
			pokemon = pokemonWild.pokemonObj.pokemon;
			battleGUI.pokemonObj = pokemonWild.pokemonObj;
			//battleGUI.ToggleHud();
			battleGUI.EnemyTargetWindow(pokemon);
		}
	}

	private void AddTargetPokemon() {
		foreach(GameObject tmpPokemon in GameObject.FindGameObjectsWithTag("pokemon")){
			AddTarget(tmpPokemon.transform);
		}
	}
	
	private void AddTarget(Transform addThisPokemon) {
		allPokemon.Add(addThisPokemon);
	}

	public Transform GetHighlightSparkles() {
		return this.highlightSparkles.transform;
	}
	private void SortTargetsByDistance() {
		allPokemon.Sort (delegate(Transform t1, Transform t2) {
				return Vector3.Distance (t1.position, playerTransform.position).CompareTo (Vector3.Distance (t2.position, playerTransform.position));
		});
	}

	private void LimitTargetDistance(float limit) {
		foreach (Transform limitTmp in allPokemon) {
			if (Vector3.Distance(limitTmp.position,playerTransform.position) > limit) {
				allPokemon.Remove(limitTmp);
			}
		}
	}

	private Transform FindNearestPokemon() {
		int currentTabCount = tabCount;
		tabCount++;
		playerTransform = Player.trainer.transform;
		int numFound = allPokemon.Count;
		if (numFound == 0) {
			AddTargetPokemon();
		}
		//LimitTargetDistance(float 25.0)
		numFound = allPokemon.Count;
		SortTargetsByDistance ();

		return allPokemon [currentTabCount];
	}

	public void TargetPokemon(Transform targetThis) {
		if (targetedPokemon != null) {
			UnHighlightTarget ();
		}
		targetedPokemon = targetThis;
		activeTarget = true;
		HighlightTarget();
	}

	public void HighlightTarget() {
		highlightSparkles = (GameObject)Instantiate (Resources.Load ("ReturnEffect"));
	}

	public void UnHighlightTarget() {
		targetedPokemon = null;
		activeTarget = false;
		/*
		if (highlightSparkles != null) {
			Destroy (highlightSparkles);
		}
		*/
	}
}