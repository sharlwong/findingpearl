using UnityEngine;
using System.Collections;

// attach this script to the anchor, which is the CHILD of every rope fragment;
//
// the reason why we can't do this in the parent instead, is because
// the parent already has a bigger sphere collider for the user input
// to be easier to perform (as there is a bigger area to click/touch);
// therefore, the parent's bigger collider should not be used to trigger the 
// anchoring event, which occurs when the rope fragment hits the
// anchor near a sea shell;
//
// Unity's collision or trigger events don't work if there are two colliders
// on the same object, hence we have to split it up into two game objects
//
// hence we use the child, which has a smaller collider, for the anchoring event

public class RopeFragmentAnchorController : MonoBehaviour {
		
	private RopeModel ropeModel;
	private ScoreController scoreController;
	private bool enableTriggerBehavior;

	void Start () {
		ropeModel = transform.parent.parent.GetComponent<RopeModel>();

		// cannot attach a reference in the scene to a prefab and automatically
		// apply that to all prefabs; therefore we to have use find instead
		scoreController = (ScoreController) FindObjectOfType(typeof(ScoreController));

		// triggering behavior is off by default
		DisableTriggerBehavior();
	}

	void OnTriggerEnter(Collider other) {
		if (enableTriggerBehavior) {
			if (other.gameObject.name == "Seashell Anchor") {

				ropeModel.AnchorFragments(transform.parent.gameObject, 
				                          other.gameObject);

				scoreController.IncrementScore(100);
			}
		}
	}

	public void EnableTriggerBehavior() {
		enableTriggerBehavior = true;
	}

	public void DisableTriggerBehavior() {
		enableTriggerBehavior = false;
	}
}
