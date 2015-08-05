using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	// Pull GameObject will disappear upon the first touch of the user
	void Update() {
		if (Input.anyKeyDown) {
			Destroy (this.gameObject);
		}	
	}
}
