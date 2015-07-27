using UnityEngine;
using System.Collections;

// attach this script to each rope fragment
public class RopeFragmentController : MonoBehaviour {

	private RopeModel ropeModel;
	private Vector3 screenPoint;
	private Vector3 offset;

	void Start() {

		// the parent of each rope fragment would be the overall Rope object, containing 
		// the RopeModel script component, so we retrieve it here
		ropeModel = transform.parent.GetComponent<RopeModel>();
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
			.GetComponentInChildren<RopeFragmentAnchorController>()
			.EnableTriggerBehavior();
	}

	void OnMouseDrag() {

		// only update the transform of this rope fragment if it is moveable
		if (ropeModel.IsFragmentMoveable(gameObject)) {

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

			ropeModel.MoveRope(gameObject, updatedPosition);
		}
	}

	void OnMouseUp() {
		gameObject
			.GetComponentInChildren<RopeFragmentAnchorController>()
			.DisableTriggerBehavior();
	}

	// note: touch controls still seem to work normally even without custom code
	// but unity warns that it may cause performance issues
}
