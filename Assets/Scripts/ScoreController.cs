using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {

	public Text timerText;
	public Text scoreText;
	public float timeRemaining;

	public Text ropeBreakText;
	public GameObject ropeBreakBG;
	
	private int score;
	private Object[] shellsArray;
	private int maxScore;

	private float timer = 3f;
	
	void Start() {

		//Timer
		InvokeRepeating ("decreaseTimeRemaining", 1.0F, 1.0F);

		// Score
		score = 0;
		SetScoreText();

		shellsArray = FindObjectsOfType(typeof(SeashellAnchorController));

		foreach(Object shell in shellsArray) {
			SeashellAnchorController actualShell = (SeashellAnchorController) shell;
			maxScore += actualShell.valueOfShell;
		}
	}

	void Update() {
		if (timeRemaining == 0)
		{
			timeElapsed();
		}
		
		timerText.text = "Time: " + timeRemaining;
	}

	// Countdown
	void decreaseTimeRemaining() {
		timeRemaining --;
	}
	
	// Gameover because time has elapsed
	void timeElapsed(){
		CancelInvoke ("decreaseTimeRemaining");
		timerText.text = "Time is Up!";

		GameEnd();
	}

	public void IncrementScore(int amount) {
		score += amount;
		SetScoreText();

		if (score == maxScore) {
			GameEnd();
		}
	}

	public void DecrementScore(int amount) {
		if (score > 0)
			score -= amount;
		SetScoreText();
	}

	public void RopeBreaks() {
		score = 0;
		SetScoreText();

		ropeBreakBG.SetActive(true);
		ropeBreakBG.GetComponent<Image>().canvasRenderer.SetAlpha(0.6f);
		ropeBreakText.gameObject.SetActive(true);
		ropeBreakText.text = "Oops! Your rope snapped! ):";

		GameEnd();

	}
	

	public void SetScoreText() {
		scoreText.text = "RP: " + score.ToString();
	}

	public int getScore() {
		return score;
	}

	private void GameEnd() {
		//save the score globally
		PlayerPrefs.SetInt (Application.loadedLevelName, score);
		PlayerPrefs.Save ();

		//save current scene
		PlayerPrefs.SetInt("LastLevel", Application.loadedLevel);
		PlayerPrefs.Save();

		PlayerPrefs.SetString("LastLevelName", Application.loadedLevelName);
		PlayerPrefs.Save();

		Application.LoadLevel("TransitionScene");
	}
}
