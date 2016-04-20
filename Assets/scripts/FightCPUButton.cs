using UnityEngine;
using System.Collections;

public class FightCPUButton : MonoBehaviour {
	private GameController controller;

	void Start() {
		controller = GameObject.Find ("Game Controller").GetComponent<GameController> ();
	}

	void OnMouseDown() {
		if (controller.state == GameController.State.Menu) {
			Debug.Log ("Click!");
			GameObject menu = GameObject.Find ("Main Menu");
			menu.transform.Translate (new Vector3 (0.0f, 0.0f, -100.0f)); // just move the menu way back behind the camera
			controller.StartGame();
		}
	}
}
