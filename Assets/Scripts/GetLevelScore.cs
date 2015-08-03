using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetLevelScore : MonoBehaviour {

	private string levelText;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		levelText = GetComponentsInChildren<Text>()[0].text;
		scoreText = GetComponentsInChildren<Text>()[1];

		scoreText.text = "RP: " + PlayerPrefs.GetInt(levelText).ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
