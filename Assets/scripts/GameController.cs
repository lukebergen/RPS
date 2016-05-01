using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public enum State {
		Menu,
		Fighting
	}

	public State state;
	public Player player1;
	public Player player2;
	public int frames;
	public int framesPerTick;

	private Vector2 startTouch = Vector2.zero;
	private Vector2 endTouch = Vector2.zero;

	private GameObject tickIndicator;
	private TextMesh gameMessage;

	public void Start() {
		tickIndicator = GameObject.Find ("Tick Indicator");
		gameMessage = GameObject.Find ("Game Message").GetComponent<TextMesh>();
	}

	public void StartGame() {
		player1 = new Player ();
		player2 = new Player ();
		frames = -100;
		state = State.Fighting;
		tickIndicator.transform.Translate(new Vector3(0.0f, 0.0f, 100.0f));
		gameMessage.text = "Ready";
		updateHealth ();
	}

	private void endGame() {
		state = State.Menu;
		tickIndicator.transform.Translate (new Vector3 (0.0f, 0.0f, -100.0f));
		GameObject menu = GameObject.Find ("Main Menu");
		menu.transform.Translate (new Vector3 (0.0f, 0.0f, 100.0f));
	}

	public void Update() {

		if (state == State.Fighting) {
			handleFightInput ();
		}

	}

	public void FixedUpdate() {
		if (state == State.Fighting) {
			if (frames == -1) {
				gameMessage.text = "Go!";
			}
			if (frames == framesPerTick) {
				if (gameMessage.text != "") {
					gameMessage.text = "";
				}
				decisionTime ();
				frames = 0;
			}
			if (frames == 0) {
				tickIndicator.transform.Rotate (new Vector3 (0f, 0f, 45));
			}
			frames++;
		}
	}

	private void decisionTime() {
		Player.Tick (player1, player2);
		updateHealth ();
		if (player1.hp == 0 || player2.hp == 0) {
			if (player1.hp == 0) {
				gameMessage.text = "Player 1 wins";
			} else { 
				gameMessage.text = "Player 2 wins";
			}
			endGame ();
		}
	}

	private void updateHealth() {
		GameObject bar = GameObject.Find ("Player1 Health Remaining");
		float remainingF = ((float)player1.hp / (float)Player.maxHp);
		bar.transform.localScale = new Vector3(remainingF, 1.0f, 1.0f);
		bar.transform.localPosition = new Vector3 ((remainingF - 1) * 0.5f, 0.0f, -0.1f);

		bar = GameObject.Find ("Player2 Health Remaining");
		remainingF = ((float)player2.hp / (float)Player.maxHp);
		bar.transform.localScale = new Vector3(remainingF, 1.0f, 1.0f);
		bar.transform.localPosition = new Vector3 ((1 - remainingF) * 0.5f, 0.0f, -0.1f);
	}

	// Player1 (the human player) is the player on the left for now.
	// So "Right" is "Toward" and "Left" is "Away"
	private void handleFightInput() {
		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0) {
			Touch t = Input.touches[0];
			if (t.phase == TouchPhase.Began) {
				startTouch = t.position;
			} elif (t.phase == TouchPhase.Ended) {
				endTouch = t.position;
			}
		}

		#elif true

		// remove this and use "else" for testing dragging stuff
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		if (horizontal != 0 || vertical != 0) {
			startTouch = Vector2.one;
			endTouch = Vector2.one + new Vector2(horizontal, vertical);
		}

		#else

		if (Input.GetMouseButtonDown(0)) {
			Vector3 mouse = Input.mousePosition;
			startTouch = new Vector2(mouse.x, mouse.y);
		}

		if (Input.GetMouseButtonUp(0) && startTouch != Vector2.zero) {
			Vector3 mouse = Input.mousePosition;
			endTouch = new Vector2(mouse.x, mouse.y);
		}

		#endif

		if (startTouch != Vector2.zero && endTouch != Vector2.zero) {
			Vector2 drag = endTouch - startTouch;
			startTouch = Vector2.zero;
			endTouch = Vector2.zero;

			Direction dir;
			if (drag == Vector2.zero) {
				dir = Direction.Idle;
			} else {
				if (Mathf.Abs (drag.x) > Mathf.Abs (drag.y)) {
					if (drag.x < 0) {
						dir = Direction.Away;
					} else {
						dir = Direction.Toward;
					}
				} else {
					if (drag.y < 0) {
						dir = Direction.Down;
					} else {
						dir = Direction.Up;
					}
				}
			}
			player1.Input(dir);
		}
	}
}

public enum Direction {
	Idle,
	Up,
	Down,
	Away,
	Toward
}