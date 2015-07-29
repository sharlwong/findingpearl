using UnityEngine;
using System.Collections;

// attach this script to the anchor, which is the CHILD of every rope fragment;
//
// the reason why we can't do this in the parent instead, is because
// the parent already has a bigger sphere collider for the user input
// to be easier to perform (as there is a bigger area to click/touch);
// therefore, the parent's bigger collider should not be used to trigger the 
// anchoring event, which occurs when the rope fragment hits the
// seashell's anchor;
//
// Unity's collision or trigger events don't work if there are two colliders
// on the same object, hence we have to split it up into two game objects
//
// hence we use the child, which has a smaller collider, for the anchoring event

public class RopeFragmentAnchorController : MonoBehaviour {
		
	private RopeModel ropeModel;
	
	void Start () {
		// ropeModel = transform.parent.parent.GetComponent<RopeModel>();
	}

// obsolete script: we decided not to fix the position of the rope fragments
// when it hits the seashell's anchor, as it produces weird visual effects
// because the rope is not "elastic"
//
//	void OnTriggerEnter(Collider other) {
//		if (other.gameObject.name == "Seashell Anchor") {
//			GameObject fragment = transform.parent.gameObject;
//			ropeModel.AnchorFragment(fragment);
//			ropeModel.AnchorNeighborFragments(fragment);
//		}
//	}
}
