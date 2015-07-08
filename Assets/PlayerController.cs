using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Text countText;
	public Text winText;
	private int count;

	public void Start ()
	{
		countText.color = Color.blue;
		countText.text = "Old Score: " + RetrieveScore ().ToString ();
		count = 0;
	}

	public void SetScoreText ()
	{
		count += 1;
		SaveScore (count);

		countText.color = Color.black;
		countText.text = "Score: " + count.ToString ();
		if (count >= 12)
		{
			winText.text = "You Win!";
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