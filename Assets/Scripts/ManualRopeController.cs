using UnityEngine;
using System.Collections;

// attach this script to each rope fragment, 
// except the FIRST and LAST fragment (as those are fixed)
public class ManualRopeController : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;

	void OnMouseDown() {
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		offset = transform.position - Camera.main.ScreenToWorldPoint(
										new Vector3(Input.mousePosition.x, 
		            								Input.mousePosition.y, 
		            								screenPoint.z));
	}

	void OnMouseDrag() {

		Vector3 updatedPosition = offset + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
		                                                                     			Input.mousePosition.y, 
		                                                                     			screenPoint.z));

		// only update the x coordinate as we want simplified user controls along x axis
		// store current transform position in a temp variable to avoid error message
		// "cannot modify a value type return value of 'UnityEngine.Transform.position'"
		Vector3 temp = transform.position;
		temp.x = updatedPosition.x;
		transform.position = temp;

		// the parent of each rope fragment would be the overall Rope object, and the ManualRope script is attached
		// to that overall Rope object, which contains a method called MoveRope(gameObject)
		transform.parent.GetComponent<ManualRope>().MoveRope(gameObject);
	}


	// TODO: attempt to model elasticity by moving back the rope to "rest" position
	// as far as possible, while still allowing for "collision" with other objects
	//
	// TODO: so far each manual rope fragment is kinematic, i.e. default collision
	// physics is not turned on; when Fixed Update is called should update 
	// and call MoveRope again to allow physics?
	void OnMouseUp() {

	}

	// touch controls still seem to work normally even without custom code
}
