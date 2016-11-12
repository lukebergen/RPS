using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public enum Action {
		// basic actions
		Idle,
		Attack,
		Block,
		Grab,
		Grabbing,  // this corresponds to "grabbed". While the opponent is "grabbed" and unable to move, we're waiting for "up throw" vs "down throw"

		// While grabbing
		DownThrow,
		UpThrow,

		// Comboing
		ComboUp,
		ComboToward,
		ComboDown,

		// Stunned states
		Grabbed,         // Stunned because currently being grabbed
		BlockStun        // Stunned because we attacked and they succesfully blocked it
		// AttackClank,  // Attack vs Attack causes both characters to "reel" away giving 1 tick of reset
		// GrabClank,    // during which technically the player is "Idle" but unable to act
	}

	public string Name; // just a label for the player for debug logging
	public int hp = 20;
	public static int maxHp = 20;

	public Action currentAction;

	public Direction nextInput;

	public Player(string name) {
		Name = name;
		currentAction = Action.Idle;
		nextInput = Direction.Idle;
	}

	public static void ResolveActions(Player p1, Player p2) {
		// Based on inputs figure out what the player is doing this tick
		p1.figureOutAction ();
		p2.figureOutAction ();

		// Announce the decisions to the log
		Debug.Log (p1.currentAction + " vs " + p2.currentAction);

		// Now technically they're still in stunned states, but that effectively means that
		// they are idle for the purposes of considering conditions. Therefore...
		p1.resetFromStun();
		p2.resetFromStun();

		// And now that everybody's currentAction has been set, consider who wins
		// and what the results of that are. The winner determines what the result is
		conditions (p1, p2);
		conditions (p2, p1);
	}

	private static void conditions (Player p1, Player p2) {
		// Since this gets called for each player, figure this out from p1's perspective
		// And let the winner determine everybodys next state/health-loss/etc...
		// so we don't end up with double dipping. This is super-not-extendable but it's
		// simple for now.
		if (p1.currentAction == Action.Attack) {
			if (p2.currentAction == Action.Attack) {
				// TIE
			} else if (p2.currentAction == Action.Block) {
				// LOSE
			} else if (p2.currentAction == Action.Grab) {
				// WIN
				p2.hp -= 1;
			} else if (p2.currentAction == Action.Idle) {
				// WIN
				p2.hp -= 1;
			}
		} else if (p1.currentAction == Action.Block) {
			if (p2.currentAction == Action.Attack) {
				// WIN
				p2.currentAction = Action.BlockStun;
			} else if (p2.currentAction == Action.Block) {
				// TIE
			} else if (p2.currentAction == Action.Grab) {
				// LOSE
			} else if (p2.currentAction == Action.Idle) {
				// TIE
			}
		} else if (p1.currentAction == Action.Grab) {
			if (p2.currentAction == Action.Attack) {
				// LOSE
			} else if (p2.currentAction == Action.Block) {
				// WIN
				p1.currentAction = Action.Grabbing;
				p2.currentAction = Action.Grabbed;
			} else if (p2.currentAction == Action.Grab) {
				// TIE
			} else if (p2.currentAction == Action.Idle) {
				// WIN
				p1.currentAction = Action.Grabbing;
				p2.currentAction = Action.Grabbed;
			}
		}
	}

	public void Input(Direction direction) {
		nextInput = direction;
	}

	public void resetFromStun() {
		if (!this.ableToAct ()) {
			this.currentAction = Action.Idle;
			this.nextInput = Direction.Idle;
		}
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
		if (ableToAct()) {
			// figure out action based on input
			if (nextInput == Direction.Down && currentAction == Action.Grabbing) {
				currentAction = Action.DownThrow;
			} else if (nextInput == Direction.Up && currentAction == Action.Grabbing) {
				currentAction = Action.UpThrow;
			} else if (nextInput == Direction.Up) {
				currentAction = Action.Grab;
			} else if (nextInput == Direction.Away) {
				currentAction = Action.Block;
			} else if (nextInput == Direction.Toward) {
				currentAction = Action.Attack;
			} else {
				currentAction = Action.Idle;
			}
		}
		nextInput = Direction.Idle;
	}

	private bool ableToAct() {
		return this.currentAction != Action.Grabbed && this.currentAction != Action.BlockStun;
	}
}