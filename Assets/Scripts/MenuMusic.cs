using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

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
		if(Application.loadedLevelName == "Level 1")
		{
			GetComponent<AudioSource>().Stop();
			AudioBegin = false;
		}
		
	}
}
