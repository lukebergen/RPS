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
	public int framesPerTick = 60;

	private Vector2 startTouch = Vector2.zero;
	private Vector2 endTouch = Vector2.zero;

	public void StartGame() {
		Debug.Log ("Ok, starting up a game");
		player1 = new Player ();
		player2 = new Player ();
		frames = 0;
		state = State.Fighting;
	}

	public void Update() {

		if (state == State.Fighting) {
			handleFightInput ();
		}

	}

	public void FixedUpdate() {
		if (state == State.Fighting) {
			if (frames % framesPerTick == 0) {
				decisionTime ();
			}
			frames++;
		}
	}

	private void decisionTime() {
		Player.Tick (player1, player2);
	}

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
						dir = Direction.Left;
					} else {
						dir = Direction.Right;
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
	Left,
	Right
}