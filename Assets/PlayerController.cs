using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Text countText;
	public Text winText;
	private int count=0;

	
	public void SetScoreText ()
	{
		count += 1;
		countText.text = "Score: " + count.ToString ();
		if (count >= 12)
		{
			winText.text = "You Win!";
		}
	}
}