using UnityEngine;
using System.Collections;

public class TrainerAI : Trainer { //Characteristics specific to the NPC
	public TrainerObjAI obj;

	public TrainerAI(TrainerObjAI obj): base("Tom") {
		this.obj = obj;
	}

	public override MonoBehaviour GetTrainerBaseObj() {
		return (MonoBehaviour)(Object)obj;
	}
	
	public override GameObject Instantiate(UnityEngine.Object resource) {
		return (GameObject)TrainerObjAI.Instantiate(resource);
	}

	public override void Update() {
		base.Update();
	}
}