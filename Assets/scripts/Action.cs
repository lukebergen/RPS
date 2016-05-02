using UnityEngine;
using System.Collections;

public class Action {

	public enum Type {
		Idle,
		Attack,
		Block,
		Grab,
		Grabbing,
		Grabbed,
		DownThrow,
		ComboUp,
		ComboToward,
		ComboDown,
		AttackClank,  // These last four all amount to a single tick of doing nothing
		GrabClank,    // during which technically the player is "Idle" but unable to act
		Stunned,      // Attack/Grab clank this amounts to both players resetting to neuatral
		Resetting     // with a little time to breath since both will be unable to act for 1 tick
	}

	public Type name;
	public int currentTick;
	public int startupTicks;
	public int activeTicks;
	public int endLagTicks;

	public Action(Type t, bool fromInput = false) {
		name = t;

		if (fromInput) {
			// If we're setting the next action based on input, then act immediately
			currentTick = 0;
		} else {
			// If this action occurred because of e.g. a blocked attack, then leave it to the next
			// Tick to set us to the starting state of tick 0.
			// There should exist a better way to do this. TODO: investigate less stupid way of handling
			// the two situations that cause an action change (input vs interaction effect).
			currentTick = -1;
		}
		if (name == Type.Idle) {
			startupTicks = 0;
			activeTicks = 0;
			endLagTicks = 1;
		} else if (name == Type.Attack) {
			startupTicks = 0;
			activeTicks = 1;
			endLagTicks = 1;
		} else if (name == Type.Block) {
			startupTicks = 0;
			activeTicks = 2;
			endLagTicks = 0;
		} else if (name == Type.Grab) {
			startupTicks = 0;
			activeTicks = 1;
			endLagTicks = 0;
		} else if (name == Type.Grabbed) {
			startupTicks = 0;
			activeTicks = 1;
			endLagTicks = 0;
		} else if (name == Type.Grabbed) {
			startupTicks = 0;
			activeTicks = 1;
			endLagTicks = 0;
		} else if (name == Type.DownThrow) {
			startupTicks = 0;
			activeTicks = 1;
			endLagTicks = 0;
		} else if (name == Type.AttackClank || name == Type.GrabClank || name == Type.Stunned || name == Type.Resetting || name == Type.Grabbed || name == Type.Grabbing) {
			startupTicks = 0;
			activeTicks = 0;
			endLagTicks = 1;
		}
	}

	public void Tick() {
		currentTick++;
	}

	public bool Attackable() {
		return (name == Type.Idle) ||
			(name == Type.Grab) ||
			(name == Type.Attack && !Active ());
	}

	public bool Blockable() {
		return name == Type.Attack && Active ();
	}

	public bool Grabbable() {
		return name == Type.Idle ||
			(name == Type.Attack && !Active ()) ||
			(name == Type.Grab && !Active ()) ||
			(name == Type.Block);
	}

	public bool Active() {
		return currentTick >= startupTicks && currentTick < (startupTicks + activeTicks);
	}

	public bool Complete() {
		return currentTick >= startupTicks + activeTicks + endLagTicks;
	}
}