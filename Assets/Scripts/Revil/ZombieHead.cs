using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHead : MonoBehaviour {
    [SerializeField] public AudioSource hiddenZombieHeadAudioSource;
    [SerializeField] public Collider hiddenZombieHeadCollider;

    public void Start() {
        hiddenZombieHeadAudioSource = GetComponent<AudioSource>();
        hiddenZombieHeadCollider = GetComponent<Collider>();
    }

    public void Update() {}

    public void OnCollisionEnter(Collision col) {
        hiddenZombieHeadCollider.enabled = false;

        transform.parent.position = new Vector3(-59.38f, -262.4f, 150.9f);

        hiddenZombieHeadAudioSource.Play();
        Destroy(gameObject, 5);
    }
}
