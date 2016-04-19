using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public enum State {
		Menu,
		Fighting
	}

	public enum Direction {
		Idle,
		Up,
		Down,
		Left,
		Right
	}

	public State state;
	public Player player1;
	public Player player2;
	public int ticks;
	public int ticksPerChoice = 60;

	private Vector2 startTouch = Vector2.zero;
	private Vector2 endTouch = Vector2.zero;

	public void StartGame() {
		Debug.Log ("Ok, starting up a game");
		player1 = new Player ();
		player2 = new Player ();
		ticks = 0;
		state = State.Fighting;
	}

	public void Update() {

		if (state == State.Fighting) {
			handleFightInput ();
		}

	}

	public void FixedUpdate() {
		if (state == State.Fighting) {
			if (ticks % ticksPerChoice == 0) {
				decisionTime ();
			}
			ticks++;
		}
	}

	private void decisionTime() {
		Debug.Log (player1.state + " vs " + player2.state);
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

public class Player {
	public enum State {
		Idle,
		Attack1,
		Attack2,
		Grab1,
		Grab2,
		Block1,
		Block2,
		Grabbing,
		Grabbed,
		ThrowingDown,
		ThrownDown,
		ThrowingUp,
		ThrownUp,
		ComboAttackUp,
		ComboAttackDown,
		ComboAttackToward,
		ComboAttackReset,
		ComboedCounterUp,
		ComboedCounterDown,
		ComboedCounterToward,
		Resetting
	}

	public State state;

	public void Start() {
		state = State.Idle;
	}

	public void Input(GameController.Direction direction) {
		Debug.Log ("Direction pressed: " + direction);
	}
}