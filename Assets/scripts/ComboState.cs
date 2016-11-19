using UnityEngine;
using System;

public class ComboState {
	private bool active;
	private int damageCounter;
	private Player comboer;
	private GameObject counter;

	public ComboState(GameObject counter) {
		active = false;
		damageCounter = 0;
		comboer = null;
		this.counter = counter;
	}

	public void Start(Player comboer) {
		active = true;
		this.SetComboer (comboer);
		updateText ("COMBO\n0");
		damageCounter = 0;
	}

	public int Finish() {
		int finisherDamage = FinisherDamage ();
		this.active = false;
		this.damageCounter = 0;
		this.comboer = null;
		updateText ("");
		return finisherDamage;
	}

	public int FinisherDamage() {
		return (int) Math.Round (damageCounter / 2.0);
	}

	public void Extend(int damage) {
		damageCounter += damage;
		updateText ("COMBO\n" + damageCounter);
	}

	public void SetComboer(Player p) {
		this.comboer = p;
		Vector3 newPos = p.gameObject.transform.position;
		newPos.y += 2.0f;
		this.counter.transform.position = newPos;
	}

	public bool IsComboer(Player p) {
		return comboer == p;
	}

	public bool Active() {
		return active;
	}

	private void updateText(string text) {
		TextMesh tm = this.counter.GetComponent<TextMesh> ();
		tm.text = text;
	}
}
