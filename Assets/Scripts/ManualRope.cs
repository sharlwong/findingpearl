using UnityEngine;
using System.Collections;

// attach this script to the parent object containing all the rope fragments
public class ManualRope : MonoBehaviour {

	// array containing the rope fragment game objects
	GameObject[] ropeFragments; 

	// array to record the ropeFragmentPosition so as to calculate the
	// difference between the old and new position for the fragment
	// that was moved by the user
	Vector3[] ropeFragmentsPosition; 

	// total number of rope fragments attached to the parent rope object
	int fragmentCount;

	// various data structures below for the line renderer
	float[] xPositions;
	float[] yPositions;
	float[] zPositions;
	CatmullRomSpline splineX;
	CatmullRomSpline splineY;
	CatmullRomSpline splineZ;
	int splineFactor = 2;

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

		LineRenderer renderer = GetComponent<LineRenderer>();
		renderer.SetWidth(0.2f, 0.21f);
		renderer.SetVertexCount((fragmentCount - 1) * splineFactor + 1);

		xPositions = new float[fragmentCount];
		yPositions = new float[fragmentCount];
		zPositions = new float[fragmentCount];
				
		splineX = new CatmullRomSpline(xPositions);
		splineY = new CatmullRomSpline(yPositions);
		splineZ = new CatmullRomSpline(zPositions);
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
		// currently uses a simple scale factor (more sophisticated math could be used
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

	// called every frame, only after all Update functions have been called
	void LateUpdate() {
		// copy rigidbody positions to the line renderer
		LineRenderer renderer = GetComponent<LineRenderer>();

		int i;
		for (i = 0; i < fragmentCount; i++) {
			Vector3 position = ropeFragmentsPosition[i];
			xPositions[i] = position.x;
			yPositions[i] = position.y;
			zPositions[i] = position.z;
		}
		
		for (i = 0; i < (fragmentCount - 1) * splineFactor + 1; i++) {
			renderer.SetPosition(i, new Vector3(
				splineX.GetValue(i / (float) splineFactor), 
				splineY.GetValue(i / (float) splineFactor), 
				splineZ.GetValue(i / (float) splineFactor)));
		}
	}

}
