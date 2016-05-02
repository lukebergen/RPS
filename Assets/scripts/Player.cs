using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public int hp = 20;
	public static int maxHp = 20;

	public Action currentAction;

	public Direction nextInput;

	public Player() {
		currentAction = new Action(Action.Type.Idle, fromInput: true);
		nextInput = Direction.Idle;
	}

	public static void Tick(Player player1, Player player2) {
		player1.figureOutAction ();
		player2.figureOutAction ();

		Player.ResolveActions (player1, player2);

		player1.Tick ();
		player2.Tick ();
	}

	public static void ResolveActions(Player p1, Player p2) {
		string msg = p1.currentAction.name + "(" + p1.currentAction.currentTick + ") (";
		if (p1.currentAction.Active ()) {
			msg += "active) VS ";
		} else {
			msg += "inactive) VS ";
		}
		msg += p2.currentAction.name + "(" + p2.currentAction.currentTick + ") (";
		if (p2.currentAction.Active ()) {
			msg += "active)";
		} else {
			msg += "inactive)";
		}
		Debug.Log (msg);

		if (p1.currentAction.name == Action.Type.Attack && p2.currentAction.name == Action.Type.Attack) {
			// Attack clank
			p1.currentAction = new Action (Action.Type.AttackClank);
			p2.currentAction = new Action (Action.Type.AttackClank);
		} else if (p1.currentAction.name == Action.Type.Grab && p2.currentAction.name == Action.Type.Grab && p1.currentAction.Active() && p2.currentAction.Active ()) {
			// Grab clank (not really functionally different from just letting the grabs complete
			// but possibly useful in future for animations and such
			p1.currentAction = new Action (Action.Type.Resetting);
			p2.currentAction = new Action (Action.Type.Resetting);
		} else if (p1.currentAction.name == Action.Type.Attack && p1.currentAction.Active () && p2.currentAction.Attackable ()) {
			// p1 attacks p2
			p2.hp -= 1;
			p2.currentAction = new Action (Action.Type.Stunned);
		} else if (p2.currentAction.name == Action.Type.Attack && p2.currentAction.Active () && p1.currentAction.Attackable ()) {
			// p2 attacks p1
			p1.hp -= 1;
			p1.currentAction = new Action (Action.Type.Stunned);
		} else if (p1.currentAction.name == Action.Type.Block && p1.currentAction.Active () && p2.currentAction.Blockable ()) {
			// p1 blocks p2
			p1.currentAction = new Action (Action.Type.Idle);
			p2.currentAction = new Action (Action.Type.Stunned);
		} else if (p2.currentAction.name == Action.Type.Block && p2.currentAction.Active () && p1.currentAction.Blockable ()) {
			// p2 blocks p1
			p2.currentAction = new Action (Action.Type.Idle);
			p1.currentAction = new Action (Action.Type.Stunned);
		} else if (p1.currentAction.name == Action.Type.Grab && p1.currentAction.Active () && p2.currentAction.Grabbable ()) {
			// p1 grabs p2
			p1.currentAction = new Action (Action.Type.Grabbing);
			p2.currentAction = new Action (Action.Type.Grabbed);
		} else if (p2.currentAction.name == Action.Type.Grab && p2.currentAction.Active () && p1.currentAction.Grabbable ()) {
			// p2 grabs p1
			p2.currentAction = new Action (Action.Type.Grabbing);
			p1.currentAction = new Action (Action.Type.Grabbed);
		} else if (p1.currentAction.name == Action.Type.DownThrow) {
			// p1 down throws p2
			p1.currentAction = new Action (Action.Type.Resetting);
			p2.currentAction = new Action (Action.Type.Stunned);
			p2.hp -= 2;
		} else if (p2.currentAction.name == Action.Type.DownThrow) {
			// p2 down throws p1
			p2.currentAction = new Action (Action.Type.Resetting);
			p1.currentAction = new Action (Action.Type.Stunned);
			p1.hp -= 2;
		}
	}

	public void Tick() {
		currentAction.Tick ();
	}

	public void Input(Direction direction) {
		nextInput = direction;
	}

	// TODO: standin for AI at this point.
	public void figureOutActionCpu() {
		float rand = UnityEngine.Random.value;
		if (rand > 0.75) {
			nextInput = Direction.Up;
		} else if (rand > 0.5) {
			nextInput = Direction.Down;
		} else if (rand > 0.25) {
			nextInput = Direction.Toward;
		} else {
			nextInput = Direction.Away;
		}
		figureOutAction ();
	}

	public void figureOutAction() {
		if (currentAction.Complete ()) {
			// figure out action based on input
			if (nextInput == Direction.Up) {
				currentAction = new Action (Action.Type.Grab, fromInput: true);
			} else if (nextInput == Direction.Away) {
				currentAction = new Action (Action.Type.Block, fromInput: true);
			} else if (nextInput == Direction.Toward) {
				currentAction = new Action (Action.Type.Attack, fromInput: true);
			} else if (nextInput == Direction.Down && currentAction.name == Action.Type.Grabbing) {
				currentAction = new Action (Action.Type.DownThrow, fromInput: true);
			} else {
				currentAction = new Action (Action.Type.Idle, fromInput: true);
			}
		}
		nextInput = Direction.Idle;
	}
}