using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisterNewUser : MonoBehaviour {

	public InputField usernameField;
	public InputField passwordField;
	public Text informText;
	

	public void SaveNewUser () 
	{
		string username = usernameField.text;
		string password = passwordField.text;

		if (PlayerPrefs.HasKey(username)) {

			informText.text = "This username has been used";

		}
		else {

			// username will be the key, and password is the value
			PlayerPrefs.SetString(username, password);
			PlayerPrefs.Save();

			informText.text = "Registration Successful! You can now login!";
		}

	}
}
