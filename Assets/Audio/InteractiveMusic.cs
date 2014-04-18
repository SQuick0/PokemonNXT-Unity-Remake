using UnityEngine;
using System.Collections.Generic;
using System;

public class InteractiveMusic : MonoBehaviour {

	public string bankName;
	public string eventName;
	// Use this for initialization
	void Start () {
		uint bankID; // Not used
		AkSoundEngine.LoadBank( bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );
		AkSoundEngine.PostEvent (eventName, gameObject);
	}

	private void Update()
	{
		string theme = "Explore";

			if (Input.GetKey(KeyCode.Return) || Input.GetKey ("enter"))
			{
				theme = "Battle";
				AkSoundEngine.SetSwitch("Default_Music", theme, gameObject);
			}
	}
}