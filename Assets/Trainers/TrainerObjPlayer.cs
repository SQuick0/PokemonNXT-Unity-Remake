/*
 * Unity player Trainer object. 
 */

using UnityEngine;
using System.Collections;

public class TrainerObjPlayer : MonoBehaviour
{
	TrainerPlayer trainer;

	// Use this for initialization
	void Start ()
	{
		trainer = new TrainerPlayer(this);
		Player.trainer = trainer;
	}

	// Update is called once per frame
	void Update ()
	{
		trainer.Update();
	}
}

