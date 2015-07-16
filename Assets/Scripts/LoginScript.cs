using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour {

	public InputField usernameField;
	public InputField passwordField;
	public Text informText;
	

	public void Login() {

		string username = usernameField.text;
		string password = passwordField.text;

		if (PlayerPrefs.HasKey(username)) {

			string savedPassword = PlayerPrefs.GetString(username);

			if (savedPassword == password) {
				// go over to game scene
				informText.text = "Login Successful!";
				Application.LoadLevel("GameScene");
			}
			else {
				informText.text = "Password Invalid. Try again.";
			}
		}
		else {
			informText.text = "Username Invalid. Try again.";
		}
	}
}
