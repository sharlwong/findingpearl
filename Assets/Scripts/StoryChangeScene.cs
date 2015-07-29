using UnityEngine;
using System.Collections;

public class StoryChangeScene : MonoBehaviour {

	public void ChangeToScene(string sceneToChangeTo) {
		float fadeTime = GameObject.Find("_Manager").GetComponent<Fading>().BeginFade(1);
		Application.LoadLevel(sceneToChangeTo);
	}
}
