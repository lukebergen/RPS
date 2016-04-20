using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public int hp = 20;

	public Action currentAction = Action.Idle;

	public Direction nextInput = Direction.Idle;

	public static void Tick(Player player1, Player player2) {
		player1.figureOutAction ();
		player2.figureOutActionCpu ();

		// clanking attacks/grabs
		if ( player1.clanks(player2) || player2.clanks(player1) ){
			Debug.Log ("Clank");
			player1.currentAction = Action.Stunned2;
			player2.currentAction = Action.Stunned2;
		}

		// attacking
		else if (player1.attacks(player2)) {
			Debug.Log ("Player1 attacks Player2");
			player2.currentAction = Action.Stunned2;
			player2.hp -= 1;
		}
		else if (player2.attacks(player1)) {
			Debug.Log ("Player2 attacks Player1");
			player1.currentAction = Action.Stunned2;
			player1.hp -= 1;
		}

		// blocking
		else if (player1.blocks(player2)) {
			Debug.Log ("Player1 blocks Player2");
			player2.currentAction = Action.Stunned2;
			player1.currentAction = Action.Idle;
		}
		else if (player2.blocks(player1)) {
			Debug.Log ("Player2 blocks Player1");
			player1.currentAction = Action.Stunned2;
			player2.currentAction = Action.Idle;
		}

		// grabbing
		else if (player1.grabs(player2)) {
			Debug.Log ("Player1 grabs Player2");
			player2.currentAction = Action.Stunned2;
			player1.currentAction = Action.Idle;
		}
		else if (player2.grabs(player1)) {
			Debug.Log ("Player2 grabs Player1");
			player2.currentAction = Action.Stunned2;
			player1.currentAction = Action.Idle;
		}

		player1.nextInput = Direction.Idle;
		player2.nextInput = Direction.Idle;
	}

	private bool clanks(Player other) {
		return currentAction == Action.Attack1 && other.currentAction == Action.Attack1;
	}

	private bool attacks(Player other) {
		return currentAction == Action.Attack1 && other.attackable ();
	}

	private bool blocks(Player other) {
		return (currentAction == Action.Block1 || currentAction == Action.Block2) && other.blockable ();
	}

	private bool grabs(Player other) {
		return currentAction == Action.Grab1 && other.grabbable ();
	}

	private bool attackable() {
		Action[] a = new Action[] { Action.Idle, Action.Attack2, Action.Grab1, Action.Grab2 };
		return Array.IndexOf (a, currentAction) > -1;
	}

	private bool blockable() {
		Action[] a = new Action[] { Action.Attack1 };
		return Array.IndexOf (a, currentAction) > -1;
	}

	private bool grabbable() {
		Action[] a = new Action[] { Action.Idle, Action.Attack2, Action.Grab2, Action.Block1, Action.Block2 };
		return Array.IndexOf (a, currentAction) > -1;
	}

	public void Input(Direction direction) {
		nextInput = direction;
	}

	public void figureOutActionCpu() {
		float rand = UnityEngine.Random.value;
		if (rand > 0.75) {
			nextInput = Direction.Up;
		} else if (rand > 0.5) {
			nextInput = Direction.Down;
		} else if (rand > 0.25) {
			nextInput = Direction.Right;
		} else {
			nextInput = Direction.Left;
		}

		figureOutAction ();
	}

	public void figureOutAction() {
		// deal with transitions that don't care what the next input is due to being locked into a multi-tick move
		if (currentAction == Action.Attack1) {
			currentAction = Action.Attack2;
		} else if (currentAction == Action.Block1) {
			currentAction = Action.Block2;
		} else if (currentAction == Action.Grab1) {
			currentAction = Action.Grab2;
		} else if (currentAction == Action.Stunned1) {
			currentAction = Action.Stunned2;
		} else {
			switch (nextInput) {
			case Direction.Idle:
				currentAction = Action.Idle;
				break;
			case Direction.Right:
				currentAction = Action.Attack1;
				break;
			case Direction.Down:
				currentAction = Action.Block1;
				break;
			case Direction.Up:
				currentAction = Action.Grab1;
				break;
			}
		}
	}
}

//	Dictionary<Player.State, int[]> actions = new Dictionary<Player.State, int[]>
//	{
//		{Player.State.Idle, new int[3] {0, 0, 1}},
//		{Player.State.Attack, new int[3] {0, 1, 1}}
//	};

public enum Action {
	Idle,
	Attack1,
	Attack2,
	Block1,
	Block2,
	Grab1,
	Grab2,
	Stunned1,
	Stunned2
}