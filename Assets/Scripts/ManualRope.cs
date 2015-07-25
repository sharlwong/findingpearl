using UnityEngine;
using System.Collections;

// attach this script to the parent object containing all the rope fragments
// a rope is made up of many fragments
public class ManualRope : MonoBehaviour {

	// number of rope fragments
	public int numOfFragments;

	// vertical interval between fragments
	public float fragmentInterval; 

	// smoothness value scales the distance that neighboring fragments  
	// will move with respect to the moved fragment for non-anchored fragments
	public float ropeSmoothness;

	// rendered thickness of rope
	public float ropeWidth;

	// maximum length the rope can stretch before breaking
	public float maxRopeLength;
	
	// GameObjects used to render a broken rope
	public GameObject brokenRopeSegment1;
	public GameObject brokenRopeSegment2;
					
	// num of fragments that should become anchored to a shell
	public int numOfAnchoredFragments;

	// the x distance that fragments near the shell should move before
	// becoming anchored, with respect to the seashell's anchor
	public float anchoredFragmentsMoveAmount;

	// array containing the game objects of each rope fragment
	GameObject[] ropeFragments; 

	// array to record the ropeFragmentPosition so as to calculate the
	// difference between the old and new position for the fragment
	// that was moved by the user
	Vector3[] ropeFragmentsPosition; 

	// number referring to the most recent fragment moved by the player
	int lastFragmentNumMovedByPlayer;

	// total number of rope fragments attached to the parent rope object
	// int numOfFragments;

	// array that tracks whether fragments should be moveable
	// e.g. 1st and last fragment should be immovable
	// and fragments that have reached the seashell should be immovable
	bool[] moveableFragments;

	bool ropeBroken = false;

	// various line renderers to draw lines between rope fragments
	// and give the appearance of a rope to the user
	LineRenderer ropeRenderer; 
	LineRenderer brokenRopeRenderer1;
	LineRenderer brokenRopeRenderer2;

	Color rendererStartColor;
	Color rendererEndColor;

	// TODO: possibly instantiate fragments programatically prior to production
	// but while debugging declare it in the scene for now
	void Start() {



		numOfFragments = transform.childCount;

		// initialize and populate the respective arrays
		ropeFragments = new GameObject[numOfFragments];
		ropeFragmentsPosition = new Vector3[numOfFragments];
		moveableFragments = new bool[numOfFragments];

		for (int i = 0; i < numOfFragments; i++) {
			ropeFragments[i] = transform.GetChild(i).gameObject;
			ropeFragmentsPosition[i] = transform.GetChild(i).position;
			// Debug.Log(ropeFragmentsPosition[i]);

			// only allow fragments in between the first and last fragment to be moveable
			if ( (i != 0) && (i != (numOfFragments-1)) ) {
				moveableFragments[i] = true;
			} else {
				moveableFragments[i] = false;
			}
		}

		// ** assign values to the various line renderers and set some properties ** //

		// these colors also interact with the material and its tint attached to the renderer;
		// i.e. don't expect the rendered rope to take on the exact same color specified here
		rendererStartColor = new Color32(147,149,152,255);
		rendererEndColor = new Color32(147,149,152,255);

		ropeRenderer = GetComponent<LineRenderer>(); 
		ropeRenderer.SetVertexCount(numOfFragments);
		ropeRenderer.SetWidth(ropeWidth, ropeWidth);
		ropeRenderer.SetColors(rendererStartColor, rendererEndColor);

		brokenRopeRenderer1 = brokenRopeSegment1.GetComponent<LineRenderer>();
		brokenRopeRenderer1.SetWidth(ropeWidth, ropeWidth);
		brokenRopeRenderer1.SetColors(rendererStartColor, rendererEndColor);

		brokenRopeRenderer2 = brokenRopeSegment2.GetComponent<LineRenderer>();
		brokenRopeRenderer2.SetWidth(ropeWidth, ropeWidth);
		brokenRopeRenderer2.SetColors(rendererStartColor, rendererEndColor);
	}

	// used by the controller to determine if a fragment is moveable
	public bool isFragmentMoveable(GameObject fragmentMoved) {
		return moveableFragments[getFragmentNumber(fragmentMoved)];
	}

	// get the fragment number from the fragment that was moved
	public int getFragmentNumber(GameObject fragmentMoved) {
		int fragmentNum;
		for (fragmentNum = 0; fragmentNum < numOfFragments; fragmentNum++) {
			if (ropeFragments[fragmentNum] == fragmentMoved) {
				break;
			}
		}
		return fragmentNum;
	}

	// called by ManualRopeControllerAnchor to move only a limited number of fragments
	// near the anchor such that a nice arc can form around the seashell, 
	// and set them as immovable thereafter
	public void MoveLimitedRope(GameObject fragmentMoved, GameObject anchor, float anchorPosition) {
		int fragmentNum = getFragmentNumber(fragmentMoved);
		lastFragmentNumMovedByPlayer = fragmentNum;

		int upperLimit = fragmentNum + numOfAnchoredFragments/2 + 1;
		int lowerLimit = upperLimit - numOfAnchoredFragments;

		if (upperLimit > numOfFragments) {
			Debug.Log("Exceeded legal values for upper limit. Try specifying a smaller limit.");
			return;
		}

		if (lowerLimit < 0) {
			Debug.Log("Exceeded legal values for lower limit. Try specifying a smaller limit.");
			return;
		}


		// example of what the loop is doing:
		//		iteration 1, fix fragment 10 position
		// 		iteration 2, fix fragment 9 and 11 position (they share same x coordinate)
		// 		iteration 3, fix fragment 8 and 12 position (they share same x coordinate)
		// 		etc.
		//
		// 		example: position fragments to surround left side of the seashell,
		//		given that fragment 10 is the fragment that collided with the seashell's anchor
		//		
		//					...
		//				fragment 12
		//			fragment 11
		//		fragment 10		(seashell)
		//			fragment 9
		//				fragment 8
		//					...
		float newX = anchorPosition;

		for (int d = 0; d < (numOfAnchoredFragments/2 + 1); d++) {

			int upper = fragmentNum + d;
			int lower = fragmentNum - d;

			// update position
			Vector3 newPositionAbove = new Vector3(newX, 
			                                       ropeFragmentsPosition[upper].y, 
			                                       ropeFragmentsPosition[upper].z);
			Vector3 newPositionBelow = new Vector3(newX, 
			                                       ropeFragmentsPosition[lower].y, 
			                                       ropeFragmentsPosition[lower].z);

			ropeFragments[upper].transform.position = newPositionAbove;
			ropeFragments[lower].transform.position = newPositionBelow;

			ropeFragmentsPosition[upper] = newPositionAbove;
			ropeFragmentsPosition[lower] = newPositionBelow;

			// set that fragment to be immovable
			moveableFragments[upper] = false;
			moveableFragments[lower] = false;

			// ** calculate the x coordinate for the next fragment **//

			// if anchor is on the left side of the screen, then the neighboring fragments
			// near to the seashell should be moved towards the right of the anchor
			// i.e. newX should increase
			if (anchor.name == "Left Anchor") {
				newX += anchoredFragmentsMoveAmount;
			} 

			// else, do the reverse, i.e. newX should decrease
			else {
				newX -= anchoredFragmentsMoveAmount;
			}
		}
	}

	// called by the ManualRopeController to move the moveable rope fragments,
	// given that the player has dragged a particular fragment
	public void MoveRope(GameObject fragmentMoved, Vector3 position) {

		int fragmentNum = getFragmentNumber(fragmentMoved);
		lastFragmentNumMovedByPlayer = fragmentNum;

		// return if the fragment is not moveable
		if (moveableFragments[fragmentNum] == false) {
			return;
		} 

		// update the position of the fragment
		fragmentMoved.transform.position = position;

		// compute difference in old and new position
		float newX = fragmentMoved.transform.position.x;
		float oldX = ropeFragmentsPosition[fragmentNum].x;
		float delta = newX - oldX;

		// then update the array storing the positions with the new position
		ropeFragmentsPosition[fragmentNum] = fragmentMoved.transform.position;	

		// then invoke recursive call to move neighboring fragments
		// both above and below the fragment that was moved (hence two calls)
		MoveFragment(fragmentNum-1, -1, delta);
		MoveFragment(fragmentNum+1, 1, delta);
	}

	// recursively move neighboring rope fragments
	// delta is the distance between the old X position and the new X position
	void MoveFragment(int fragmentNumber, int direction, float delta) {

		// base case: return if reached the fragments at the start and at the end
		if ( (fragmentNumber <= 0) || ((fragmentNumber+1) >= numOfFragments) ) {
			return;
		}

		// calculate how much to move the neighbor fragment by using a scale factor
		// currently uses a simple scale factor (more sophisticated math could be used)
		delta = ropeSmoothness * delta;

		if (moveableFragments[fragmentNumber] == true) {

			// rope fragment is restricted to move only in the x axis direction
			// (i.e. left and right, with respect to a vertically oriented rope in game)
			Vector3 newPosition =  ropeFragmentsPosition[fragmentNumber]
									+ new Vector3(delta, 0.0f, 0.0f);

			// update the array that stores the positions
			ropeFragmentsPosition[fragmentNumber] = newPosition;

			// also update the actual position of the fragment
			ropeFragments[fragmentNumber].transform.position = newPosition;

		}


		// update the fragment below with respect to the rope in game
		if (direction < 0) {
			MoveFragment(fragmentNumber-1, direction, delta);
		}

		// update the fragment above with respect to the rope in game
		else {
			MoveFragment(fragmentNumber+1, direction, delta);
		}
	}
	
	float CalculateRopeLength() {
		float lengthOfRope = 0.0f;
		for (int i = 0; i < (numOfFragments-1); i++) {
			lengthOfRope += Vector3.Distance(ropeFragmentsPosition[i], ropeFragmentsPosition[i+1]);
		}
		// Debug.Log("Length of rope: " + lengthOfRope);
		return lengthOfRope;
	}

	// called every frame, only after all Update functions have been called
	void LateUpdate() {

		if (ropeBroken) {
			return;
		}

		if (CalculateRopeLength() > maxRopeLength) {
			Debug.Log("Rope has been broken!");
			ropeBroken = true;

			// first, undraw the rope 
			GetComponent<LineRenderer>().SetVertexCount(0);

			// then, draw the two broken rope segments from the point of breakage

			// draw the first rope segment
			int numOfVertices = lastFragmentNumMovedByPlayer;
			Debug.Log(numOfVertices);
			brokenRopeRenderer1.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer1.SetPosition(i, ropeFragmentsPosition[i]);
			}

			// draw the second rope segment
			numOfVertices = numOfFragments - lastFragmentNumMovedByPlayer;
			brokenRopeRenderer2.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer2.SetPosition(i, ropeFragmentsPosition[i+lastFragmentNumMovedByPlayer]);
			}

			return;
		}

		// draw the rope normally if the rope has not been broken
		for (int i = 0; i <numOfFragments; i++) {
			ropeRenderer.SetPosition(i, ropeFragmentsPosition[i]);
		}

		return;
	}

}
