using UnityEngine;
using System.Collections;

public class FightCPUButton : MonoBehaviour {
	private GameController controller;
	private GameObject menu;

	void Start() {
		// TODO: move all of this into GameController. This should just be a
		// glorified click event tracker and just pass that along to GameController.
		controller = GameObject.Find ("Game Controller").GetComponent<GameController> ();
		// it was getting annoying having this in front of the action during dev. Start with it out of the way and move it into place at app start.
		menu = GameObject.Find ("Main Menu");
		menu.transform.Translate (new Vector3 (0.0f, 0.0f, 100.0f));
	}

	void OnMouseDown() {
		if (controller.state == GameController.State.Menu) {
			Debug.Log ("Click!");
			menu.transform.Translate (new Vector3 (0.0f, 0.0f, -100.0f)); // just move the menu way back behind the camera
			controller.StartGame();
		}
	}
}
