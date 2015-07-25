using UnityEngine;
using System.Collections;

// attach this script to each rope fragment
public class ManualRopeController : MonoBehaviour {

	ManualRope manualRope;
	private Vector3 screenPoint;
	private Vector3 offset;

	void Start() {

		// the parent of each rope fragment would be the overall Rope object, containing 
		// the ManualRope script component, so we retrieve it here
		manualRope = transform.parent.GetComponent<ManualRope>();
	}

	void OnMouseDown() {
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		offset = transform.position - Camera.main.ScreenToWorldPoint(
										new Vector3(Input.mousePosition.x, 
		            								Input.mousePosition.y, 
		            								screenPoint.z));

		// only enable triggering behavior in the child script
		// when the player is directly moving this fragment
		// i.e. the rest of the fragments not directly controlled by player
		// should not have the triggering behavior
		gameObject
			.GetComponentInChildren<ManualRopeControllerAnchor>()
			.enableTriggerBehavior = true;
	}

	void OnMouseDrag() {

		// only update the transform of this rope fragment if it is moveable
		if (manualRope.isFragmentMoveable(gameObject)) {

			Vector3 updatedPosition 
				= offset 
					+ Camera.main.ScreenToWorldPoint(
						new Vector3(Input.mousePosition.x, 
									Input.mousePosition.y, 
									screenPoint.z));

			// retain the y and z coordinates of the rope fragment,
			// as we want a simple user control of moving fragments only in x axis
			updatedPosition.y = transform.position.y;
			updatedPosition.z = transform.position.z;

			manualRope.MoveRope(gameObject, updatedPosition);
		}
	}
	
	// KIV: attempt to model elasticity by moving back the rope to "rest" position smoothly
	void OnMouseUp() {
		gameObject
			.GetComponentInChildren<ManualRopeControllerAnchor>()
			.enableTriggerBehavior = false;
	}

	// note: touch controls still seem to work normally even without custom code
	// but unity warns that it may cause performance issues
}
