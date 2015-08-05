using UnityEngine;
using System.Collections;

public class StoryChangeScene : MonoBehaviour {

	void Start() {
		// PlayerPrefs.DeleteAll();
	}

	public void ChangeToScene(string sceneToChangeTo) {

		if (Application.loadedLevelName == "StoryScene5") {
			if (PlayerPrefs.GetInt("StoryActive") == 1) {
				PlayerPrefs.SetInt("StoryActive", 0);
				PlayerPrefs.Save();
				TransitAndLoad("StartScene");
				return; // important to return from method!
			} 
		}
	
		TransitAndLoad(sceneToChangeTo);
	}

	public void OnStartButtonEnter() {
		if (PlayerPrefs.HasKey("FirstTimePlayer")) {
			ChangeToScene("LevelSelectionScene");
		} else {
			PlayerPrefs.SetInt("FirstTimePlayer", 1);
			PlayerPrefs.Save();
			ChangeToScene("StoryScene1");
		}
	}

	public void OnStoryButtonEnter() {
		PlayerPrefs.SetInt("StoryActive", 1);
		PlayerPrefs.Save();
		ChangeToScene("StoryScene1");
	}

	private void TransitAndLoad(string sceneToChangeTo) {
		GameObject.Find("_Manager").GetComponent<Fading>().BeginFade(1);
		Application.LoadLevel(sceneToChangeTo);
	}
}
