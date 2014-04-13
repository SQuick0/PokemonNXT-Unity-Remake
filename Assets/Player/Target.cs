using UnityEngine;
using System.Collections;

public abstract class Target { //Shared target for using items or identification of an interactive target
	public enum TARGETS {TRAINER, POKEMON};

	public abstract TARGETS GetTargetType();
	public bool IsTargetType(TARGETS target) {
		return GetTargetType() == target;
	}
}