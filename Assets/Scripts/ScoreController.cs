using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {

	public Text countText;
	private int count;

	// Use this for initialization
	void Start () {
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter (Collision col){
//		Debug.Log( "collide (name) : " + col.collider.gameObject.name );
//		Debug.Log ("shell collide");
		if (col.gameObject.name == "Rope Fragment(Clone)") {
			SetScoreText();
		}
	}

	void SetScoreText() {
		count += 1;
		countText.text = "Score: " + count.ToString ();
	}
}
