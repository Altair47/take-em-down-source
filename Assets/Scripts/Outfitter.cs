using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

[Serializable]
public class Outfitter : MonoBehaviour {
	public PlayerController playerController;
	public int oldWeaponIndex;
	[SerializeField] public List<WeaponSlot> weapons;

	public void Start() {
		playerController = GetComponentInChildren<PlayerController>();

		// start scene unarmed
		for (int i = 0; i < weapons.Count; i++) {
			for (int model = 0; model < weapons[i].models.Count; model++) {
				weapons[i].models[model].SetActive(false);
			}
		}

		for (int model = 0; model < weapons[playerController.weaponSlot].models.Count; model++) {
			weapons[playerController.weaponSlot].models[model].SetActive(true);
		}

		oldWeaponIndex = playerController.weaponSlot;
	}

	public void Update() {
		if (playerController.weaponSlot != oldWeaponIndex) {
			for (int model = 0; model < weapons[oldWeaponIndex].models.Count; model++) {
				weapons[oldWeaponIndex].models[model].SetActive(false);
			}
			for (int model = 0; model < weapons[playerController.weaponSlot].models.Count; model++) {
				weapons[playerController.weaponSlot].models[model].SetActive(true);
			}
			oldWeaponIndex = playerController.weaponSlot;
		}
	}
}

[Serializable]
public class WeaponSlot {
	//[SerializeField] public List<Renderer> models = new List<Renderer>();
	[SerializeField] public List<GameObject> models = new List<GameObject>();
}
