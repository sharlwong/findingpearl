using UnityEngine;
using System.Collections;

// attach this script to each Seashell Anchor
public class SeashellAnchorController : MonoBehaviour {

	public int valueOfShell;

	private bool scoreIncreased;
	private ScoreController scoreController;
	private Color32 originalColor;
	
	void Start() {

		// cannot attach a reference in the scene to a prefab and automatically
		// apply that to all prefabs; therefore we to have use the find method instead
		scoreController = (ScoreController) FindObjectOfType(typeof(ScoreController));
		scoreIncreased = false;
		originalColor = GetComponentsInParent<Renderer>()[1].material.color;
	}

	// had to change approach to the score update;
	// originally used OnTriggerEnter and OnTriggerExit
	// to increase and decrease the score,
	// however, after increasing the score once,
	// a single fragment leaving the shell and calling OnTriggerExit()
	// will cause the score to decrease even though there
	// are other fragments still in contact with the shell anchor.
	//
	// now we use Physics.OverlapSphere to count how many fragments 
	// are in contact the shell instead; if there is  >= 1 fragment
	// inside, the score will be increased (if it hasn't been increased),
	// otherwise, it will be decreased (if it hasn't been decreased).
	void Update() {
		if (RopeOverlapsWithCollider()) {

			// only increment score once
			if (!scoreIncreased) {
				scoreController.IncrementScore(valueOfShell);
				scoreIncreased = true;

				// alter the color (tint) of the seashell
				// to show the player that the score has increased
				// as a result of this seashell overlapping with the rope

				// logs "Turqoise-shell-shadow (Instance)"
				// Debug.Log(GetComponentsInParent<Renderer>()[1].material.name); 
				GetComponentsInParent<Renderer>()[1]
					.material.SetColor("_Color", new Color32(255,255,100,255));
			}
		} 

		else {
			if (scoreIncreased) {
				scoreController.DecrementScore(valueOfShell);
				scoreIncreased = false;
				GetComponentsInParent<Renderer>()[1]
					.material.SetColor("_Color", originalColor);
			}
		}
	}
	
	bool RopeOverlapsWithCollider() {
		if (GetNumOfOverlappingRopeFragments() == 0) {
			return false;
		} else {
			return true;
		}
	}

	int GetNumOfOverlappingRopeFragments() {
		Vector3 parentPosition = transform.parent.transform.position;
		SphereCollider col = transform.GetComponent<SphereCollider>();
		Collider[] colliders = Physics.OverlapSphere(parentPosition, col.radius);
		
		int numOfRopeFragmentsOverlapping = 0;
		foreach (Collider collider in colliders) {
			if (collider.name == "Anchor Collider") {
				numOfRopeFragmentsOverlapping++;
			}
		}

		return numOfRopeFragmentsOverlapping;
	}
}
