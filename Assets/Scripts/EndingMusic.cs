using UnityEngine;
using System.Collections;

public class EndingMusic : MonoBehaviour {

	static bool AudioBegin = false; 
	
	void Awake()
	{
		if (!AudioBegin) {
			GetComponent<AudioSource>().Play ();
			DontDestroyOnLoad (gameObject);
			AudioBegin = true;
		} 
	}
	
	void Update () {
		if(Application.loadedLevelName == "StartScene")
		{
			GetComponent<AudioSource>().Stop();
			AudioBegin = false;
		}
		
	}
}