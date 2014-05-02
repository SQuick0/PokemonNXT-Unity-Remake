using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour {
	float runSpeed = 2;
	public bool run = false;
	
	void OnAnimatorMove() {
		//Animator animator = GetComponent<Animator> ();
		Animator animator = Player.trainer.GetComponent<Animator> ();
		if (animator) {
			Vector3 newPosition = animator.deltaPosition;
			if (Rebind.GetInput ("SPRINT") || run) {
				newPosition *= runSpeed;
			}
			//rigidbody.position += newPosition;
			rigidbody.position+=newPosition;
		}
	}
}