using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	void Start() {
	
	}

	void Update() {
	
	}

	public void ResetLevel() {
		Application.LoadLevel(Application.loadedLevelName);
	}
}
