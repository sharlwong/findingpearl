using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionController : MonoBehaviour {

	public Text levelText;
	public Text scoreText;
	public Button nextLevelButton;

	private string lastLevelName;
	private int lastLevel;
	private int lastLevelScore;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey ("LastLevelName")) 
		{
			lastLevelName = PlayerPrefs.GetString ("LastLevelName");
			lastLevel = PlayerPrefs.GetInt("LastLevel");
			lastLevelScore = PlayerPrefs.GetInt(lastLevelName);
		}

		levelText.text = lastLevelName;
		scoreText.text = lastLevelScore.ToString() + " RP";

		if (lastLevelScore == 0) {
			nextLevelButton.interactable = false;
			nextLevelButton.GetComponent<Image>().canvasRenderer.SetAlpha(0.5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RestartLevel() {
		Application.LoadLevel(lastLevelName);
	}

	public void LevelSelection() {
		Application.LoadLevel("LevelSelectionScene");
	}

	public void NextLevel() {
		if (lastLevelScore != 0) {
			Application.LoadLevel(lastLevel + 1);
		}
	}
}
