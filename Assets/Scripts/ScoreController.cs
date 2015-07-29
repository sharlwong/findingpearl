using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {

	public Text scoreText;

	private int score;
	
	void Start() {
		score = 0;
		SetScoreText();
	}

	public void IncrementScore(int amount) {
		score += amount;
		SetScoreText();
	}

	public void DecrementScore(int amount) {
		if (score > 0)
			score -= amount;
		SetScoreText();
	}

	public void RopeBreaks() {
		score = 0;
		SetScoreText();
	}

	public void SetScoreText() {
		scoreText.text = "RP: " + score.ToString();
	}

	public int getScore() {
		return score;
	}
}
