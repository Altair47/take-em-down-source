using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    private PlayerController player;

    private Animator animator;
    private Rigidbody rb;
    private Collider col;
    private Transform playerPosition;
    private NavMeshAgent agent;
    public ParticleSystem bloodspray;
    public ParticleSystem onDeathParticle;

    public AudioSource audioSource = default;
    public AudioSource audioSourceLoop = default;
    public AudioClip[] zAttackClips = default;
    public AudioClip[] zDeathClips = default;
    public AudioClip[] zIdleClips = default;
    public AudioClip[] zRageClips = default;
    public AudioClip[] zFunClips = default;

    [SerializeField] private float zHealth = 1000.0f;
    [SerializeField] private float zHealthRangeMin = 150.0f;
    [SerializeField] private float zHealthRangeMax = 400.0f;
    [SerializeField] private float zDamageRangeMin = 5.0f;
    [SerializeField] private float zDamageRangeMax = 10.0f;
    [SerializeField] public bool zCanBeDamaged = false;
    [SerializeField] public bool zCanDamage = false;
    [SerializeField] public bool zRageMode = false;
    [SerializeField] public bool zVictory = false;
    
    public void Start() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        updateZombieHealth();
    }

    public void Update() {
        if (zVictory) return;

        // am I still alive?
        checkLife();

        // is hooman still alive?
        checkPlayerLife();

        // can I rage?
        handleRage();
    }

    private void OnTriggerEnter(Collider col) {
        if (!player.isDead && col.gameObject.tag == "Hitbox") zCanBeDamaged = true;
    }

    private void OnTriggerStay(Collider col) {
        if (!player.isDead && zCanBeDamaged && Input.GetMouseButtonDown(0)) {
            animator.SetBool("Pain", true);
            Instantiate(bloodspray, transform.GetChild(0).position, bloodspray.transform.rotation);

            // take zombie body parts off
            dismember(Random.Range(0, 16));
            zHealth -= player.playerDamage;

            animator.SetBool("Pain", false);

            //TODO push zombie back
            rb.AddForce(transform.forward * -10 * 5 * Time.deltaTime, ForceMode.Force);
        }
    }

    private void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Hitbox") zCanBeDamaged = false;
    }

    private void OnCollisionEnter(Collision col) {
        if (!player.isDead && col.gameObject.tag == "Player") {
            zCanDamage = true;
            StartCoroutine(damagePlayer());
        }
    }

    private void OnCollisionExit(Collision col) {
        if (col.gameObject.tag == "Player") zCanDamage = false;
    }

    private void updateZombieHealth() => zHealth = Random.Range(zHealthRangeMin, zHealthRangeMax);

    private void checkLife() {
        if (zHealth <= 0) {
            Instantiate(onDeathParticle, transform.position, onDeathParticle.transform.rotation);
            //TODO add sound maybe?
            Destroy(gameObject);
            PlayerPrefs.SetInt("zombieCount", PlayerPrefs.GetInt("zombieCount") - 1);
        }
    }

    private void checkPlayerLife() {
        if (player.isDead && !zVictory) {
            audioSourceLoop.Stop();

            agent.isStopped = true;  // idle it
            animator.SetBool("isMoving", false);  // idle it

            StartCoroutine(playSound("fun", loop: false));
            zVictory = true;
        }
    }

    private void handleRage() {
        if (!player.isDead && !zRageMode && zHealth <= zHealthRangeMin / 3) {
            zRageMode = true;

            agent.isStopped = true;  // idle it
            animator.SetBool("isMoving", false);  // idle it
            
            animator.SetBool("Attack", true);  // start raging mode
            StartCoroutine(playSound("idle", loop: false));
            animator.SetBool("Attack", false);  // start raging mode

            agent.speed *= 2;  // multiply speed by two
            agent.isStopped = false;  // start ai again
            animator.SetBool("isMoving", true);  // start moving again
            StartCoroutine(playSound("rage", loop: true));
        }
    }

    private void dismember(int bodypart) {
        switch (bodypart) {
            case 1:  // left arm
                transform.GetChild(5).gameObject.SetActive(false);
                transform.GetChild(6).gameObject.SetActive(false);
                transform.GetChild(7).gameObject.SetActive(false);
                break;
            case 2:  // left arm palm
                transform.GetChild(7).gameObject.SetActive(false);
                break;
            case 3:  // left arm forearm
                transform.GetChild(6).gameObject.SetActive(false);
                transform.GetChild(7).gameObject.SetActive(false);
                break;
            case 4:  // right arm
                transform.GetChild(8).gameObject.SetActive(false);
                transform.GetChild(9).gameObject.SetActive(false);
                transform.GetChild(10).gameObject.SetActive(false);
                break;
            case 5:  // right arm palm
                transform.GetChild(10).gameObject.SetActive(false);
                break;
            case 6:  // right arm forearm
                transform.GetChild(9).gameObject.SetActive(false);
                transform.GetChild(10).gameObject.SetActive(false);
                break;
            case 10:  // head
                transform.GetChild(1).gameObject.SetActive(false);
                zHealth /= 2.5f;
                break;
            default:
                break;
        }
    }

    private IEnumerator damagePlayer() {
        while (zCanDamage) {
            animator.SetTrigger("Attack");
            StartCoroutine(playSound("attack", loop: false));
            yield return new WaitForSeconds(0.8f);
            
            if (!zCanDamage) {
                Debug.Log("Player escaped before attack was finished, Dodged.");
                yield break;
            }
            StartCoroutine(player.PlaySound("zombie/hit"));
            player.health -= Random.Range(zDamageRangeMin, zDamageRangeMax);
        }
    }

    private IEnumerator playSound(string reason, bool loop=false) {
        switch (reason) {
            case "attack":
                if (!loop) audioSource.PlayOneShot(zAttackClips[Random.Range(0, zAttackClips.Length - 1)]);
                else {
                    audioSourceLoop.clip = zAttackClips[Random.Range(0, zAttackClips.Length - 1)];
                    audioSourceLoop.Play();
                }
                break;
            case "idle":
                if (!loop) audioSource.PlayOneShot(zIdleClips[Random.Range(0, zIdleClips.Length - 1)]);
                else {
                    audioSourceLoop.clip = zIdleClips[Random.Range(0, zIdleClips.Length - 1)];
                    audioSourceLoop.Play();
                }
                break;
            case "rage":
                if (!loop) audioSource.PlayOneShot(zRageClips[Random.Range(0, zRageClips.Length - 1)]);
                else {
                    audioSourceLoop.clip = zRageClips[Random.Range(0, zRageClips.Length - 1)];
                    audioSourceLoop.Play();
                }
                break;
            case "death":
                if (!loop) audioSource.PlayOneShot(zDeathClips[Random.Range(0, zDeathClips.Length - 1)]);
                else {
                    audioSourceLoop.clip = zDeathClips[Random.Range(0, zDeathClips.Length - 1)];
                    audioSourceLoop.Play();
                }
                break;
            case "fun":
                if (!loop) audioSource.PlayOneShot(zFunClips[Random.Range(0, zFunClips.Length - 1)]);
                else {
                    yield return new WaitForSeconds(Random.Range(7.5f, 12.5f));
                    audioSourceLoop.clip = zFunClips[Random.Range(0, zFunClips.Length - 1)];
                    audioSourceLoop.Play();
                }
                break;
            default:
                break;
        }
        yield return null;
    }
}
