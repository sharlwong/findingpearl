using UnityEngine;
using System.Collections;

public class RandomShell : MonoBehaviour {

	private int selectedTexture;
	public Texture2D[] textures;  // Assign textures in the Inspector

	void Start () {
		// Choose a random texture to render
		selectedTexture = Random.Range (0, textures.Length);
		GetComponent<Renderer>().material.SetTexture("_MainTex", textures[selectedTexture]);
	}

}


