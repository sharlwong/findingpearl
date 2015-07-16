using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public Text countText;
	public Text timerText;
	public Text playButtonText;
	public float timeRemaining;
	private int count;
	private bool gameRunning=true;
	
	void Start() {
		InvokeRepeating ("decreaseTimeRemaining", 1.0F, 1.0F);
		countText.color = Color.blue;
		countText.text = "Old Score: " + RetrieveScore ().ToString ();
		count = 0;
	}
	
	void Update() {
		if (timeRemaining == 0)
		{
			timeElapsed();
		}
		
		timerText.text = timeRemaining + " seconds remaining!";
	}
	
	// Countdown
	void decreaseTimeRemaining() {
		timeRemaining --;
	}
	
	// Gameover
	void timeElapsed(){
		CancelInvoke ("decreaseTimeRemaining");
		playButtonText.text = "Game Over";
		gameRunning = false;
	}
	
	// Set score
	public void SetScoreText ()
	{
		if (gameRunning == true) {
			count += 1;
			countText.text = "Score: " + count.ToString ();
			countText.color = Color.black;
			SaveScore(count);
		}
	}
	
	private void SaveScore (int score) 
	{
		PlayerPrefs.SetInt ("Score", score);
		PlayerPrefs.Save ();
	}
	
	/*
	 * Returns the previous score. If previous score does not exist, it returns 0.
	 */
	private int RetrieveScore ()
	{
		if (PlayerPrefs.HasKey ("Score")) 
		{
			return PlayerPrefs.GetInt ("Score");
		}
		
		else
		{
			return 0;
		}
	}
}


