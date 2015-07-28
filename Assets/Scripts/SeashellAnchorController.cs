using UnityEngine;
using System.Collections;

public class SeashellAnchorController : MonoBehaviour {

	private bool scoreIncreased;
	private ScoreController scoreController;
	
	void Start() {

		// cannot attach a reference in the scene to a prefab and automatically
		// apply that to all prefabs; therefore we to have use the find method instead
		scoreController = (ScoreController) FindObjectOfType(typeof(ScoreController));
		scoreIncreased = false;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "Anchor Collider") {

			// only increment the score once
			if (!scoreIncreased) {
				scoreController.IncrementScore(100);
				scoreIncreased = true;

				// alter the color (tint) of the seashell
				// Debug.Log(GetComponentsInParent<Renderer>()[1].material.name); // logs "Turqoise-shell-shadow (Instance)"
				GetComponentsInParent<Renderer>()[1].material.SetColor("_Color", new Color32(255,255,100,255));
			}
		}
	}
}
