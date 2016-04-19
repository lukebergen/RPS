using UnityEngine;
using System.Collections;

public class FightCPUButton : MonoBehaviour {
	private bool clicked = false;

	void OnMouseDown() {
		Debug.Log ("Click!");
		GameObject menu = GameObject.Find ("Main Menu");
		menu.transform.Translate(new Vector3 (0.0f, 0.0f, -100.0f)); // just move the menu way back behind the camera
		GameObject controller = GameObject.Find("Game Controller");
		controller.GetComponent<GameController> ().StartGame ();
	}
}
