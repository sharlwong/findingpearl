using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// attach this script to each Seashell Anchor
public class SeashellAnchorController : MonoBehaviour {

	public Text shellScore;

	public int valueOfShell;
	private bool scoreIncreased;
	private ScoreController scoreController;
	private Color32 originalColor;
	private Vector3 shellPosition;
	private SphereCollider col;
	private float scale;
	private Texture shellTexture;
	
	void Start() {

		// cannot attach a reference in the scene to a prefab and automatically
		// apply that to all prefabs; therefore we to have use the find method instead
		scoreController = (ScoreController) FindObjectOfType(typeof(ScoreController));
		scoreIncreased = false;
		originalColor = GetComponentsInParent<Renderer>()[1].material.color;
		shellPosition = transform.parent.transform.position;
		col = transform.GetComponent<SphereCollider>();
		scale = transform.localScale.x;
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

		shellTexture = GetComponentsInParent<Renderer>()[1].material.GetTexture("_MainTex");

		if (shellTexture.ToString () == "Beige_Shell") {
			valueOfShell = 10;
		} else if (shellTexture.ToString () == "Yellow_Shell") {
			valueOfShell = 20;
		} else if (shellTexture.ToString () == "Red_Shell") {
			valueOfShell = 30;
		}

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

				// show score earned with shell
				StartCoroutine(ShowShellScore());
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

	// uncomment this to see the size of the Physics.OverlapSphere in the scene
//	void OnDrawGizmos() {
//		Gizmos.color = Color.red;
//		Gizmos.DrawWireSphere(shellPosition, col.radius*transform.localScale.x);
//	}

	int GetNumOfOverlappingRopeFragments() {

		// since our sphere collider itself has been scaled in the scene,
		// the value of col.radius must also be scaled
		// e.g. if we create a sphere, set its radius to 0.5, and then
		// scale it to 0.1, 0.1, 0.1, the radius value is still 0.5, but
		// the sphere is much smaller, so we need to scale the radius value here
		Collider[] colliders = Physics.OverlapSphere(
									shellPosition, 
									col.radius*scale);

		int numOfRopeFragmentsOverlapping = 0;
		foreach (Collider collider in colliders) {
			if (collider.name == "Anchor Collider") {
				numOfRopeFragmentsOverlapping++;
			}
		}
		return numOfRopeFragmentsOverlapping;
	}

	// Fade in and out shell score
	// By toggling the 'active status' of the shellScore game object
	private IEnumerator ShowShellScore() {
		shellScore.gameObject.SetActive(true);
		shellScore.text = valueOfShell + " RP";
		yield return new WaitForSeconds(1);
		shellScore.gameObject.SetActive(false);
	}

}
