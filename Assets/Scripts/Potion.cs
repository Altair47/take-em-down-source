using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {
    private PlayerController player;
    public float potionHealthBoost = 13.37f;
    public bool potionSuccess = false;

    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Update() {}

    private void OnTriggerEnter(Collider col) {
        if (potionSuccess) return;

        player.health += potionHealthBoost;
        potionSuccess = true;
        StartCoroutine(player.PlaySound("react/reflexion"));

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);
        Destroy(gameObject, 5);
    }
}
