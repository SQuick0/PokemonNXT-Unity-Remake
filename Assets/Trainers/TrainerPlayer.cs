using UnityEngine;
using System.Collections;

public class TrainerPlayer : Trainer { //Characteristics specific to the player
	public TrainerObjPlayer obj;

	public TrainerPlayer(TrainerObjPlayer obj): base("Red") {
		this.obj = obj;
	}

	public override MonoBehaviour GetTrainerBaseObj() {
		return (MonoBehaviour)(System.Object)obj;
	}
	
	public override GameObject Instantiate(UnityEngine.Object resource) {
		return (GameObject)TrainerObjPlayer.Instantiate(resource);
	}

	public override void Update() {
		base.Update();
	}
}
