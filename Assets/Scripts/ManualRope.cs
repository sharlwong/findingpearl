using UnityEngine;
using System.Collections;

// attach this script to the parent object containing all the rope fragments
public class ManualRope : MonoBehaviour {

	// maximum length the rope can stretch before breaking
	public float maximumRopeLength;

	// these game objects would become useful when the rope breaks,
	// in order to draw 2 separate ropes from the point of breakage
	public GameObject brokenRopeSegment1;
	public GameObject brokenRopeSegment2;

	// thickness to render the ropes
	public float ropeRendererWidth;
	
	// array containing the game objects of each rope fragment
	GameObject[] ropeFragments; 

	// array to record the ropeFragmentPosition so as to calculate the
	// difference between the old and new position for the fragment
	// that was moved by the user
	Vector3[] ropeFragmentsPosition; 

	// most recent rope fragment number that was moved
	int lastFragmentNumMovedByPlayer;

	// total number of rope fragments attached to the parent rope object
	int fragmentCount;

	bool ropeIsBroken = false;

	// various line renderers to draw lines between rope fragments
	// and give the appearance of a rope to the user
	LineRenderer ropeRenderer; 
	LineRenderer brokenRopeRenderer1;
	LineRenderer brokenRopeRenderer2;

	// TODO: possibly instantiate fragments programatically prior to production
	// but while debugging declare it in the scene for now
	void Start() {

		fragmentCount = transform.childCount;

		// initialize and populate the respective arrays
		ropeFragments = new GameObject[transform.childCount];
		ropeFragmentsPosition = new Vector3[transform.childCount];

		for (int i = 0; i < fragmentCount; i++) {
			ropeFragments[i] = transform.GetChild(i).gameObject;
			ropeFragmentsPosition[i] = transform.GetChild(i).position;
			Debug.Log(ropeFragmentsPosition[i]);
		}

		// assign values to the various line renderers and set some properties
		// this game object that the script is attached to contains a line renderer component
		ropeRenderer = GetComponent<LineRenderer>(); 
		ropeRenderer.SetVertexCount(fragmentCount);
		ropeRenderer.SetWidth(ropeRendererWidth, ropeRendererWidth);

		// the broken rope segment game objects similarly contain a line renderer component
		brokenRopeRenderer1 = brokenRopeSegment1.GetComponent<LineRenderer>();
		brokenRopeRenderer1.SetWidth(ropeRendererWidth, ropeRendererWidth);

		brokenRopeRenderer2 = brokenRopeSegment2.GetComponent<LineRenderer>();
		brokenRopeRenderer2.SetWidth(ropeRendererWidth, ropeRendererWidth);
	}
	
	// to be called by the Controller handling user input
	public void MoveRope(GameObject fragmentMoved) {

		// get the fragment number from the fragment that was moved
		int fragmentNum;
		for (fragmentNum = 0; fragmentNum < fragmentCount; fragmentNum++) {
			if (ropeFragments[fragmentNum] == fragmentMoved) {
				break;
			}
		}

		lastFragmentNumMovedByPlayer = fragmentNum;

		float newX = fragmentMoved.transform.position.x;
		float oldX = ropeFragmentsPosition[fragmentNum].x;
		float delta = newX - oldX;

		// update the array storing the positions
		ropeFragmentsPosition[fragmentNum] = fragmentMoved.transform.position;

		MoveFragment(fragmentNum-1, -1, delta);
		MoveFragment(fragmentNum+1, 1, delta);
	}

	// recursively move neighboring rope fragments
	void MoveFragment(int fragmentNumber, int direction, float delta) {

		// base case: return if reached the fragments at the start and at the end
		if ( (fragmentNumber <= 0) || ((fragmentNumber+1) >= fragmentCount) ) {
			return;
		}

		// calculate how much to move the neighbor fragment by using a scale factor
		// currently uses a simple scale factor (more sophisticated math could be used)
		float scale = 0.90f;
		delta = scale * delta;

		// rope fragment is restricted to move only in the x axis direction
		// (i.e. left and right, with respect to a vertically oriented rope in game)
		Vector3 newPosition =  ropeFragmentsPosition[fragmentNumber]
								+ new Vector3(delta, 0.0f, 0.0f);

		// update the array that stores the positions
		ropeFragmentsPosition[fragmentNumber] = newPosition;

		// also update the actual position of the fragment
		ropeFragments[fragmentNumber].transform.position = newPosition;


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
		for (int i = 0; i < (fragmentCount-1); i++) {
			lengthOfRope += Vector3.Distance(ropeFragmentsPosition[i], ropeFragmentsPosition[i+1]);
			Debug.Log (lengthOfRope);
		}
		Debug.Log("Length of rope: " + lengthOfRope);
		return lengthOfRope;
	}

	// called every frame, only after all Update functions have been called
	void LateUpdate() {

		if (ropeIsBroken) {
			return;
		}

		if (CalculateRopeLength() > maximumRopeLength) {
			Debug.Log("Rope has been broken!");
			ropeIsBroken = true;

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
			numOfVertices = fragmentCount - lastFragmentNumMovedByPlayer;
			brokenRopeRenderer2.SetVertexCount(numOfVertices);
			for (int i = 0; i < numOfVertices; i++) {
				brokenRopeRenderer2.SetPosition(i, ropeFragmentsPosition[i+lastFragmentNumMovedByPlayer]);
			}

			return;
		}

		// draw the rope normally if the rope has not been broken
		for (int i = 0; i <fragmentCount; i++) {
			ropeRenderer.SetPosition(i, ropeFragmentsPosition[i]);
		}

		return;
	}

}
