using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		StartCoroutine(ShowShellScore());
	}
	
	// Update is called once per frame
	private IEnumerator ShowShellScore() {
		yield return new WaitForSeconds(3);
		this.gameObject.SetActive(false);
	}

}
