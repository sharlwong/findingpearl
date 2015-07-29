using UnityEngine;
using System.Collections;

public class ChangeScenes : MonoBehaviour {

	// Use this for initialization
	public void ChangeToScene(string sceneToChangeTo){
		Application.LoadLevel (sceneToChangeTo);
	}
}
