using UnityEngine;
using System.Collections;

// attach this script to the parent rope object containing all the rope fragments
public class RopeModel : MonoBehaviour {

	//** PUBLIC VARIABLES **//

	public GameObject fragmentPrefab;
		
	public Vector3 firstFragmentPosition;	
	public Vector3 lastFragmentPosition; // x,y should be same as the first fragment
	public Vector3 fragmentInterval; // only along z axis for vertical rope 

	// the x coordinates of the left and right boundary
	public float leftBoundaryX;
	public float rightBoundaryX;

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
	public int neighborFragsToAnchor;

	//** PRIVATE VARIABLES **//

	// array containing the game objects of each rope fragment
	private GameObject[] ropeFragments; 

	// array to record the ropeFragmentPosition so as to calculate the
	// difference between the old and new position for the fragment
	// that was moved by the user
	private Vector3[] ropeFragmentsPosition; 

	private int numOfFragments;

	// number referring to the most recent fragment moved by the player
	private int lastFragmentNumMovedByPlayer;

	// total number of rope fragments attached to the parent rope object
	// int numOfFragments;

	// array that tracks whether fragments should be moveable
	// e.g. 1st and last fragment should be immovable
	// and fragments that have reached the seashell should be immovable
	private bool[] moveableFragments;

	private bool ropeBroken = false;

	private float initialRopeLength;

	// various line renderers to draw lines between rope fragments
	// and give the appearance of a rope to the user
	private LineRenderer ropeRenderer; 
	private LineRenderer brokenRopeRenderer1;
	private LineRenderer brokenRopeRenderer2;

	private Color initialRendererColor;
	private Color currentRendererColor;
	
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

		initialRopeLength = CalculateRopeLength();

		// ** assign values to the various line renderers and set some properties ** //

		// these colors also interact with the material and its tint attached to the renderer;
		// i.e. don't expect the rendered rope to take on the exact same color specified here
		initialRendererColor = new Color32(147,149,152,255);

		ropeRenderer = GetComponent<LineRenderer>(); 
		ropeRenderer.SetVertexCount(numOfFragments);
		ropeRenderer.SetWidth(ropeWidth, ropeWidth);
		ropeRenderer.SetColors(initialRendererColor, initialRendererColor);

		brokenRopeRenderer1 = brokenRopeSegment1.GetComponent<LineRenderer>();
		brokenRopeRenderer1.SetWidth(ropeWidth, ropeWidth);

		brokenRopeRenderer2 = brokenRopeSegment2.GetComponent<LineRenderer>();
		brokenRopeRenderer2.SetWidth(ropeWidth, ropeWidth);
	}

	// called by RopeFragmentAnchorController to anchor a single fragment
	public void AnchorFragment(GameObject fragmentMoved) {
		int fragmentNum = GetFragmentNumber(fragmentMoved);
		lastFragmentNumMovedByPlayer = fragmentNum;
		moveableFragments[fragmentNum] = false;
	}

	// called by RopeFragmentAnchorController to anchor 
	// neighboring fragments relative to fragmentMoved
	// the number of neighbors to anchor is referenced in neighborFragsToAnchor
	public void AnchorNeighborFragments(GameObject fragmentMoved) {
		int fragmentNum = GetFragmentNumber(fragmentMoved);
		int upperLimit = fragmentNum + neighborFragsToAnchor/2 + 1;
		int lowerLimit = upperLimit - neighborFragsToAnchor;

		if (upperLimit > numOfFragments) {
			Debug.Log("Exceeded legal values for upper limit. Try specifying a smaller limit.");
			return;
		}

		if (lowerLimit < 0) {
			Debug.Log("Exceeded legal values for lower limit. Try specifying a smaller limit.");
			return;
		}

		// example of what the loop is doing:
		//		say the fragmentNum is 10
		// 		iteration 1, fix fragment 9 and 11 position
		// 		iteration 2, fix fragment 8 and 12 position
		// 		etc.
		for (int d = 1; d < (neighborFragsToAnchor/2 + 1); d++) {
			moveableFragments[fragmentNum+d] = false;
			moveableFragments[fragmentNum-d] = false;
		}
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

	public bool IsFragmentMoveable(GameObject fragmentMoved) {
		return moveableFragments[GetFragmentNumber(fragmentMoved)];
	}

	// called by the RopeFragmentController to move moveable rope fragments,
	// given that the player has dragged a particular fragment
	public void MoveRope(GameObject fragmentMoved, Vector3 position) {

		int fragmentNum = GetFragmentNumber(fragmentMoved);
		lastFragmentNumMovedByPlayer = fragmentNum;

		// return if the fragment is not moveable
		if (moveableFragments[fragmentNum] == false) {
			return;
		} 

		// return if the new position of the fragment will exceed the boundaries
		if (position.x <= leftBoundaryX || position.x >= rightBoundaryX) {
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

			// return if the new position of the fragment will exceed the boundaries
			if (newPosition.x <= leftBoundaryX || newPosition.x >= rightBoundaryX) {
				return;
			}

			else {

				// update the array that stores the positions
				ropeFragmentsPosition[fragmentNumber] = newPosition;

				// also update the actual position of the fragment
				ropeFragments[fragmentNumber].transform.position = newPosition;
			}

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

	// as the rope gets increasingly stretched, alter the color and alpha
	void UpdateRopeColor() {

		float ropeStretchAmount = CalculateRopeLength() - initialRopeLength;
		float fractionalIncrease = ropeStretchAmount / initialRopeLength;

		// don't do anything if the stretch amount decreases
		if (fractionalIncrease <= 0) {
			return;
		}

		// rgb values range from 0 to 1 (not 0 to 255!)
		// initialRendererColor is unchanged throughout the game
		float initialRed = initialRendererColor.r;
		float initialGreen = initialRendererColor.g;
		float initialBlue = initialRendererColor.b;
		float initialAlpha = initialRendererColor.a;

		// TOOD: how should the color and alpha change as the rope is stretched?
		float newRed = initialRed;//fractionalIncrease*initialRed*0.5f;
		float newGreen = initialGreen;//+ fractionalIncrease*initialGreen*0.5f;
		float newBlue = initialBlue;// + fractionalIncrease*initialBlue*0.5f;
		float newAlpha = initialAlpha - fractionalIncrease*0.9f;

		currentRendererColor = new Color(newRed, newGreen, newBlue, newAlpha);
		ropeRenderer.SetColors(currentRendererColor, currentRendererColor);
	}

	float CalculateRopeLength() {
		float lengthOfRope = 0.0f;
		for (int i = 0; i < (numOfFragments-1); i++) {
			lengthOfRope += Vector3.Distance(ropeFragmentsPosition[i], ropeFragmentsPosition[i+1]);
		}
		// Debug.Log("Length of rope: " + lengthOfRope);
		return lengthOfRope;
	}

	void Update() {
		UpdateRopeColor();
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

			// and set all of its fragments to be immovable
			for (int i = 0; i < numOfFragments; i++) {
				moveableFragments[i] = false;
			}

			// ** then, draw the two broken rope segments from the point of breakage ** //

			// equally allocated the specified gap between the first and second rope segment
			int halfGap = brokenRopeGap/2;

			// draw the first rope segment
			int numOfVertices = lastFragmentNumMovedByPlayer - halfGap;
			brokenRopeRenderer1.SetColors(currentRendererColor, currentRendererColor);
			brokenRopeRenderer1.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer1.SetPosition(i, ropeFragmentsPosition[i]);
			}

			// draw the second rope segment
			numOfVertices = (numOfFragments-1) - lastFragmentNumMovedByPlayer - halfGap;
			brokenRopeRenderer2.SetColors(currentRendererColor, currentRendererColor);
			brokenRopeRenderer2.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer2
					.SetPosition(i, ropeFragmentsPosition[i+1+lastFragmentNumMovedByPlayer+halfGap]);
			}

			// update score
			((ScoreController) FindObjectOfType(typeof(ScoreController))).RopeBreaks();

			return;
		}

		// draw the rope normally if the rope has not been broken
		for (int i = 0; i <numOfFragments; i++) {
			ropeRenderer.SetPosition(i, ropeFragmentsPosition[i]);
		}

		return;
	}
}
