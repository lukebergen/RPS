using UnityEngine;
using System;

public class ComboState {
	private bool active;
	private int damageCounter;
	private Player comboer;

	public ComboState() {
		active = false;
		damageCounter = 0;
		comboer = null;
	}

	public void Start(Player comboer) {
		active = true;
		this.comboer = comboer;
		damageCounter = 0;
	}

	public int Finish() {
		int finisherDamage = FinisherDamage ();
		this.active = false;
		this.damageCounter = 0;
		this.comboer = null;
		return finisherDamage;
	}

	public int FinisherDamage() {
		return (int) Math.Round (damageCounter / 2.0);
	}

	public void Extend(int damage) {
		damageCounter += damage;
	}

	public void SetComboer(Player p) {
		comboer = p;
	}

	public bool IsComboer(Player p) {
		return comboer == p;
	}

	public bool Active() {
		return active;
	}
}
