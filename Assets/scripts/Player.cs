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
		ComboIdle,

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

	private ComboState comboState;

	public Player(string name, ComboState comboState) {
		Name = name;
		currentAction = Action.Idle;
		nextInput = Direction.Idle;
		this.comboState = comboState;
	}

	public void Conditions (Player other) {
		// Since this gets called for each player, figure this out from `this` players
		// perspective and let the winner determine everybodys next state/health-loss/etc...
		// so we don't end up with double dipping. This is super-not-extendable but it's
		// simple for now.
		if (comboState.Active ()) {
			comboConditions (other);
		} else {
			nonComboConditions (other);
		}
	}

	private void comboConditions(Player p2) {
		Player p1 = this;
		if (comboState.IsComboer(p1)) {
			// up
			if (p1.currentAction == Action.ComboUp) {
				if (p2.currentAction == Action.ComboUp) {
					// REVERSAL
					comboState.SetComboer (p2);
				} else {
					// EXTENSION
					comboState.Extend (1);
					p2.hp -= 1;
				}
			} else if (p1.currentAction == Action.ComboToward) {
				if (p2.currentAction == Action.ComboToward) {
					// REVERSAL
					comboState.SetComboer (p2);
				} else {
					// EXTENSION
					comboState.Extend (1);
					p2.hp -= 1;
				}
			} else if (p1.currentAction == Action.ComboDown) {
				if (p2.currentAction == Action.ComboDown) {
					// REVERSAL FINISHER
					comboState.SetComboer (p2);
					// do finisher damage and finish the combo
					p1.hp -= comboState.Finish();
				} else {
					// FINISHER
					p2.hp -= comboState.Finish();
				}
			} else {
				comboState.Finish ();  // since we're just resetting to neutral, the finisher damage applies to nobody
			}
		}
	}

	private void nonComboConditions(Player p2) {
		Player p1 = this;		
		if (p1.currentAction == Action.Attack) {
			if (p2.currentAction == Action.Attack) {
				// TIE1
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
		} else if (p1.currentAction == Action.DownThrow) {
			// This can only happen if we're in a grab - grabbed situation
			// No possible retort from opponent so just do the down throw
			p2.hp -= 2;
		} else if (p1.currentAction == Action.UpThrow) {
			// This can only happen if we're in a grab - grabbed situation
			// No possible retort from opponent so just do the up throw
			comboState.Start (p1);
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
			if (comboState.Active()) {
				if (nextInput == Direction.Down) {
					currentAction = Action.ComboDown;
				} else if (nextInput == Direction.Toward) {
					currentAction = Action.ComboToward;
				} else if (nextInput == Direction.Up) {
					currentAction = Action.ComboUp;
				} else {
					currentAction = Action.ComboIdle;
				}
			} else {
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
		}
		nextInput = Direction.Idle;
	}

	private bool ableToAct() {
		return this.currentAction != Action.Grabbed && this.currentAction != Action.BlockStun;
	}
}