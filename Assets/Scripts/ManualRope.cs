using UnityEngine;
using System.Collections;

// attach this script to the parent object containing all the rope fragments
// a rope is made up of many fragments
public class ManualRope : MonoBehaviour {

	public GameObject fragmentPrefab;
		
	public Vector3 firstFragmentPosition;	
	public Vector3 lastFragmentPosition; // x,y should be same as the first fragment
	public Vector3 fragmentInterval; // only along z axis for vertical rope 

	// starting x coordinate to be put into the model
	// model refers to how the fragments in the rope should move
	// in this implementation, it is a sigmoid model
	public float modelStartX;

	// rate of decay of the model
	public float modelDecayRate;

	// rendered thickness of rope
	public float ropeWidth;

	// maximum length the rope can stretch before breaking
	public float maxRopeLength;

	public int brokenRopeGap;
	
	// GameObjects used to render a broken rope
	public GameObject brokenRopeSegment1;
	public GameObject brokenRopeSegment2;
					
	// num of fragments that should become anchored to a shell
	public int anchoredFragsPerShell;

	// array containing the game objects of each rope fragment
	GameObject[] ropeFragments; 

	// array to record the ropeFragmentPosition so as to calculate the
	// difference between the old and new position for the fragment
	// that was moved by the user
	Vector3[] ropeFragmentsPosition; 

	int numOfFragments;

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
	
	void Start() {

		// commented out after implementing programatic instantiation of rope fragments;
		// existing fragments in the Scene have been DISABLED (greyed out in Project Hierarchy)
		// numOfFragments = transform.childCount;

		// calculate number of fragments required
		numOfFragments = Mathf.CeilToInt( (lastFragmentPosition.z-firstFragmentPosition.z) 
		                                 /fragmentInterval.z );
		
		Debug.Log("Number of fragments: " + numOfFragments);

		// initialize the respective arrays
		ropeFragments = new GameObject[numOfFragments];
		ropeFragmentsPosition = new Vector3[numOfFragments];
		moveableFragments = new bool[numOfFragments];

		// instatiate fragments and populate the arrays
		Vector3 position = firstFragmentPosition;

		for (int i = 0; i < numOfFragments; i++) {

			ropeFragments[i] = (GameObject) Instantiate(
												fragmentPrefab, 
												position, 
												Quaternion.identity);

			ropeFragments[i].transform.parent = transform;
			ropeFragmentsPosition[i] = position;

			// commented out after implementing programatic instantiation of rope fragments;
			// existing fragments in the Scene have been DISABLED (greyed out in Project Hierarchy)
			// ropeFragments[i] = transform.GetChild(i).gameObject;
			// ropeFragmentsPosition[i] = transform.GetChild(i).position;

			// only allow fragments in between the first and last fragment to be moveable
			if ( (i != 0) && (i != (numOfFragments-1)) ) {
				moveableFragments[i] = true;
			} else {
				moveableFragments[i] = false;
			}

			position += fragmentInterval;
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
	
	public bool IsFragmentMoveable(GameObject fragmentMoved) {
		return moveableFragments[GetFragmentNumber(fragmentMoved)];
	}

	// get the fragment number from the fragment that was moved
	public int GetFragmentNumber(GameObject fragmentMoved) {
		int fragmentNum;
		for (fragmentNum = 0; fragmentNum < numOfFragments; fragmentNum++) {
			if (ropeFragments[fragmentNum] == fragmentMoved) {
				break;
			}
		}
		return fragmentNum;
	}

	// called by the ManualRopeController to move the moveable rope fragments,
	// given that the player has dragged a particular fragment
	public void MoveRope(GameObject fragmentMoved, Vector3 position) {

		int fragmentNum = GetFragmentNumber(fragmentMoved);
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
		float distanceMovedByPlayer = newX - oldX;

		// then update the array storing the positions with the new position
		ropeFragmentsPosition[fragmentNum] = fragmentMoved.transform.position;	

		// then invoke recursive call to move neighboring fragments
		// both above and below the fragment that was moved (hence two calls)
		MoveFragment(fragmentNum-1, -1, distanceMovedByPlayer, modelStartX);
		MoveFragment(fragmentNum+1, 1, distanceMovedByPlayer, modelStartX);
	}

	// recursively move neighboring rope fragments
	//
	// distanceMovedByPlayer is unchanged between subsequent recursive calls to MoveFragment
	//
	// x is the input to the sigmoid function
	void MoveFragment(int fragmentNumber, int direction, float distanceMovedByPlayer, float x) {

		// base case: return if reached the fragments at the start and at the end
		if ( (fragmentNumber <= 0) || ((fragmentNumber+1) >= numOfFragments) ) {
			return;
		}
		
		if (moveableFragments[fragmentNumber] == true) {

			// calculate amountToMoveInX using a sigmoid model;
			//
			// for fragments near to the first fragment,
			// we want those fragments to move a lot so that the curve is smooth
			// i.e. the amountToMoveInX is high;
			//
			// but as we try to move further fragments in subsequent recursive calls, 
			// the amountToMoveInX should be lesser;
			//
			// and for the fragments very far away, we don't want much movement at all
			// so the amountToMoveInX should tend to zero
			//
			// therefore the sigmoid is a suitable model;
			//
			// the input to the sigmoid function should gradually decrease,
			// as we want the sigmoid value to start high, but end low;
			// sketch out the curve to visualize this better;
			//
			// don't forget to tune the parameters; 
			// alternative models can be considered too
			float amountToMoveInX = Sigmoid(x) * distanceMovedByPlayer;

			// rope fragment is restricted to move only in the x axis direction
			// (i.e. left and right, with respect to a vertically oriented rope in game)
			Vector3 newPosition =  ropeFragmentsPosition[fragmentNumber]
									+ new Vector3(amountToMoveInX, 0.0f, 0.0f);

			// update the array that stores the positions
			ropeFragmentsPosition[fragmentNumber] = newPosition;

			// also update the actual position of the fragment
			ropeFragments[fragmentNumber].transform.position = newPosition;

		}

		x = x - modelDecayRate;

		// update the fragment below with respect to the rope in game
		if (direction < 0) {
			MoveFragment(fragmentNumber-1, direction, distanceMovedByPlayer, x);
		}

		// update the fragment above with respect to the rope in game
		else {
			MoveFragment(fragmentNumber+1, direction, distanceMovedByPlayer, x);
		}
	}

	// compute the sigmoid value
	float Sigmoid(float x) {
		return 1.0f / ( 1.0f + Mathf.Exp(-x) );
	}
	
	// called by ManualRopeControllerAnchor to anchor 
	// fragments near the seashell (i.e. make immovable); 
	// the number of fragments to anchor is referenced in anchoredFragsPerShell
	public void AnchorFragments(GameObject fragmentMoved, GameObject anchor) {
		int fragmentNum = GetFragmentNumber(fragmentMoved);
		lastFragmentNumMovedByPlayer = fragmentNum;
		
		int upperLimit = fragmentNum + anchoredFragsPerShell/2 + 1;
		int lowerLimit = upperLimit - anchoredFragsPerShell;
		
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
		for (int d = 0; d < (anchoredFragsPerShell/2 + 1); d++) {
			
			int upper = fragmentNum + d;
			int lower = fragmentNum - d;
			
			// set that fragment to be immovable
			moveableFragments[upper] = false;
			moveableFragments[lower] = false;
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
			Debug.Log("Rope has been broken at fragment number " + lastFragmentNumMovedByPlayer);
			ropeBroken = true;

			// first, undraw the main rope 
			ropeRenderer.SetVertexCount(0);

			// ** then, draw the two broken rope segments from the point of breakage ** //

			// equally allocated the specified gap between the first and second rope segment
			int halfGap = brokenRopeGap/2;

			// draw the first rope segment
			int numOfVertices = lastFragmentNumMovedByPlayer - halfGap;
			brokenRopeRenderer1.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer1.SetPosition(i, ropeFragmentsPosition[i]);
			}

			// draw the second rope segment
			numOfVertices = (numOfFragments-1) - lastFragmentNumMovedByPlayer - halfGap;
			brokenRopeRenderer2.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer2
					.SetPosition(i, ropeFragmentsPosition[i+1+lastFragmentNumMovedByPlayer+halfGap]);
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
