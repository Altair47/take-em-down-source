using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cthulu : MonoBehaviour {
    private PlayerController player;
    private Animator animator;
    private Rigidbody rb;
    private Collider col;
    private Transform playerPosition;
    private UnityEngine.AI.NavMeshAgent agent;
    public ParticleSystem bloodspray;
    public AudioSource audioSource = default;
    public bool canAttack = true;
    public bool isDead = false;
    [SerializeField] public float cHealth = 1000f;
    [SerializeField] public bool cCanBeDamaged = false;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    public void Update() {
        HandleHealth();

        // messure distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition.position);
        HandlePosition(distanceToPlayer);
        HandleAttack(distanceToPlayer);
    }

    private void HandleAttack(float distance) {
        if (distance < 15 && canAttack && !isDead) StartCoroutine(Attack());
    }

    private void HandlePosition(float distance) {
        // look player
        if (distance < 20 && !isDead) transform.LookAt(new Vector3(player.transform.position.x, 1, player.transform.position.z));            
    }

    private void HandleHealth() {
        if (cHealth <= 0) {
            animator.SetBool("isAlive", false);
            agent.enabled = false;  // stop the agent
            col.enabled = false;  // disable collider
            isDead = true;

            transform.Translate(Vector3.down * Time.deltaTime * 0.66f);
            Destroy(gameObject, 12.5f);
        }
    }

    private IEnumerator Attack() {
        switch (Random.Range(0, 3)) {
            case 0:
                animator.SetTrigger("R");
                break;
            case 1:
                animator.SetTrigger("L");
                break;
        }

        canAttack = false;
        yield return new WaitForSeconds(1);
        canAttack = true;
    }

    private void OnTriggerEnter(Collider col) {
        if (!player.isDead && col.gameObject.tag == "Hitbox") cCanBeDamaged = true;
    }

    private void OnTriggerStay(Collider col) {
        if (!player.isDead && cCanBeDamaged && Input.GetMouseButtonDown(0)) {
            animator.SetTrigger("gettingHit");
            Instantiate(bloodspray, transform.position, bloodspray.transform.rotation);
            cHealth -= player.playerDamage;
        }
    }

    private void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Hitbox") cCanBeDamaged = false;
    }
}
