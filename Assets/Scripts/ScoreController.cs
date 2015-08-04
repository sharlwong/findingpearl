using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {

	public Text timerText;
	public Text scoreText;
	public float timeRemaining;

	public Text ropeBreakText;
	public Text winText;
	public Text timeupText;

	public AudioClip ropeSnapSound;
	public AudioClip collectShellSound;

	private AudioSource source;
	private RopeModel ropeModel;

	private int score;
	private Object[] shellsArray;
	private int maxScore;
	

	// *** Public Methods *** //

	public void Start() {
		source = GetComponent<AudioSource>();
		ropeModel = FindObjectOfType<RopeModel>();

		score = 0;
		maxScore = CalculateMaxScore();
		SetScoreText();

		// Timer
		InvokeRepeating ("DecreaseTimeRemaining", 1.0f, 1.0f);
	}
	
	public void Update() {
		if (timeRemaining == 0) {
			timerText.text = "Time is Up!";
			timeupText.gameObject.SetActive(true);
			StartCoroutine(PrepareTransition());
		}
		SetTimerText();
	}

	public int getScore() {
		return score;
	}

	public void IncrementScore(int amount) {
		source.PlayOneShot(collectShellSound);
		score += amount;
		SetScoreText();
		
		if (score == maxScore) {
			winText.gameObject.SetActive(true);
			StartCoroutine(PrepareTransition());
		}
	}

	public void DecrementScore(int amount) {
		if (score > 0)
			score -= amount;
		SetScoreText();
	}
	
	public void RopeBreaks() {
		source.PlayOneShot(ropeSnapSound);
		score = 0;
		SetScoreText();
		ropeBreakText.gameObject.SetActive(true);
		StartCoroutine(PrepareTransition());
	}


	// *** Private methods *** //
	
	private int CalculateMaxScore() {
		int score = 0;
		shellsArray = FindObjectsOfType(typeof(SeashellAnchorController));
		foreach(Object shell in shellsArray) {
			SeashellAnchorController actualShell = (SeashellAnchorController) shell;
			score += actualShell.valueOfShell;
		}
		return score;
	}
	
	private	void SetScoreText() {
		scoreText.text = "RP: " + score.ToString();
	}
	
	private void SetTimerText() {
		timerText.text = "Time: " + timeRemaining;
	}
	
	private void DecreaseTimeRemaining() {
		timeRemaining --;
	}

	private IEnumerator PrepareTransition() {
		CancelInvoke("DecreaseTimeRemaining");
		ropeModel.FreezeRope();
		yield return new WaitForSeconds(2.0f);
		GameEnd();
	}

	private void GameEnd() {

		//save the score globally
		PlayerPrefs.SetInt(Application.loadedLevelName, score);
		PlayerPrefs.Save();

		//save current scene
		PlayerPrefs.SetInt("LastLevel", Application.loadedLevel);
		PlayerPrefs.Save();

		PlayerPrefs.SetString("LastLevelName", Application.loadedLevelName);
		PlayerPrefs.Save();

		Application.LoadLevel("TransitionScene");
	}
}
