using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour {
    [Header("Components stuff")]
    public Transform thirdPersonCamera;
    public Animator animator;
	public Rigidbody rb;
    public Collider col;
    public AudioSource sound;
    public TextMeshProUGUI debugText;
    private GameManager gameManager;
    public ParticleSystem fxSwordSlash = default;
    public ParticleSystem fxPunch = default;
    public ParticleSystem magicPoof = default;
    private int layerMask;

    [Header("Movement")]
    [SerializeField] public float rotationSpeed = 500.0f;
    [SerializeField] public float moveSpeed = 1.5f;
    [SerializeField] public float moveSpeedLimit = 8.5f;
    [SerializeField] public float runningSpeed = 4.5f;
	[SerializeField] public float horizontalMouseSpeed = 2.0f;
    [SerializeField] public float climbSpeed = 0.85f;
    [SerializeField] public float horizontalInput;
    [SerializeField] public float verticalInput;

    [Header("Functional options")]
    [SerializeField] public bool canJump = true;
    [SerializeField] public bool canChangeWeapons = true;
    [SerializeField] public bool canRun = true;
    [SerializeField] public bool canClimb = false;
    [SerializeField] public bool canAttack = true;
    [SerializeField] public bool isGrounded = true;
    [SerializeField] public bool wasGrounded = true;
    [SerializeField] public bool wasFalling = false;
    [SerializeField] public bool isMoving = false;
    [SerializeField] public bool isDead = false;
    [SerializeField] public bool bossBattle = false;

    [Header("Health")]
    [SerializeField] public float health = 100.0f;
    [SerializeField] public AudioSource painAudioSource = default;
    [SerializeField] public AudioClip[] painGenericClips = default;
    [SerializeField] public AudioClip[] painFallClips = default;
    [SerializeField] public AudioClip[] painDamageClips = default;

    [Header("Reactions")]
    [SerializeField] public List<AudioClip[]> reactions = new List<AudioClip[]>();
    [SerializeField] public AudioSource reactAudioSource = default;
    [SerializeField] public AudioClip[] reactShockedClips = default;
    [SerializeField] public AudioClip[] reactGaspClips = default;
    [SerializeField] public AudioClip[] reactEffortClips = default;
    [SerializeField] public AudioClip[] reactLaughClips = default;
    [SerializeField] public AudioClip[] reactNodClips = default;
    [SerializeField] public AudioClip[] reactReflexionClips = default;
    [SerializeField] public AudioClip[] reactSighClips = default;
    [SerializeField] public AudioClip[] reactCoughClips = default;

    [Header("Physics")]
    [SerializeField] public float gravityScale = 5.0f;

    [Header("Weapons")]
    [SerializeField] public int weaponSlot = 0;
    [SerializeField] public int weaponSlotOld = 1;
    [SerializeField] public int weaponState = 0;  // 0=unarmed, 1=one handed, 2=two handed, 3=spear
    [SerializeField] public static int weaponMaxSlots = 7;
    [SerializeField] public int attackRange = 0;
    [SerializeField] public float playerDamage = 7.5f;  // default for unarmed
    [SerializeField] public float playerSpearDamage = 10.5f;  // default for unarmed
    [SerializeField] public float playerOneHandedSwordDamage = 17.5f;  // default for unarmed
    [SerializeField] public float playerTwoHandedSwordDamage = 22.5f;  // default for unarmed
    [SerializeField] public AudioSource attackAudioSource = default;
    [SerializeField] public AudioClip[] swordClips = default;
    [SerializeField] public AudioClip[] swordHitMetalClips = default;
    [SerializeField] public AudioClip[] swordHitWoodClips = default;
    [SerializeField] public AudioClip[] spearClips = default;
    [SerializeField] public AudioClip[] hitBloodclips = default;
    [SerializeField] public AudioClip[] hitFleshclips = default;
    [SerializeField] public AudioClip[] punchClips = default;
    [SerializeField] public AudioClip[] punchHitClips = default;

    [Header("Footsteps")]
    [SerializeField] public float baseStepSpeed = 0.33f;
    [SerializeField] public float sprintStepMultiplier = 0.88f;
    [SerializeField] public AudioSource footstepAudioSource = default;
    [SerializeField] public AudioClip[] woodClips = default;
    [SerializeField] public AudioClip[] metalClips = default;
    [SerializeField] public AudioClip[] grassClips = default;
    [SerializeField] public AudioClip[] waterClips = default;
    [SerializeField] public AudioClip[] defaultClips = default;
    [SerializeField] public float footstepTimer = 0.0f;

    [Header("Jump")]
    [SerializeField] public float jumpAmount = 15.5f;
    [SerializeField] public float minimumFall = 9.0f;
    [SerializeField] public float minimumJump = 3.5f;
    [SerializeField] private float distToGround;
    [SerializeField] private float startOfFall;
    [SerializeField] public float jumpTimer = 0.0f;
    [SerializeField] public float baseJumpSpeed = 0.5f;
    [SerializeField] public AudioSource jumpAudioSource = default;
    [SerializeField] public AudioClip[] playerJumpClips = default;
    [SerializeField] public AudioClip[] woodJumpClips = default;
    [SerializeField] public AudioClip[] metalJumpClips = default;
    [SerializeField] public AudioClip[] grassJumpClips = default;
    [SerializeField] public AudioClip[] waterJumpClips = default;
    [SerializeField] public AudioClip[] defaultJumpClips = default;
    [SerializeField] public AudioClip[] playerJumpLandClips = default;
    [SerializeField] public AudioClip[] woodJumpLandClips = default;
    [SerializeField] public AudioClip[] metalJumpLandClips = default;
    [SerializeField] public AudioClip[] grassJumpLandClips = default;
    [SerializeField] public AudioClip[] waterJumpLandClips = default;
    [SerializeField] public AudioClip[] defaultJumpLandClips = default;
    private Vector3 scaleChange = new Vector3(0.5f, 0.5f, 0.5f);

    public void Start() {
        Vector3 localScale = transform.localScale;
        thirdPersonCamera = GameObject.Find("ThirdPersonCamera").GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // add reactions
        reactions.Add(reactShockedClips);
        reactions.Add(reactGaspClips);
        reactions.Add(reactLaughClips);
        reactions.Add(reactSighClips);

        // update distance to ground
        distToGround = col.bounds.extents.y;
    }

	public void FixedUpdate() {
        if (gameManager.gameIsPaused || isDead) return;

        layerMask = 1 << 26;
        layerMask = ~layerMask;
        CheckLife();
        CheckGround();

        // take fall damage based on distance to ground
        if (!wasFalling && isFalling) startOfFall = transform.position.y;
        if (!wasGrounded && isGrounded) {
            float fallDistance = startOfFall - transform.position.y;
            if (fallDistance >= minimumFall) PlayerTakeDamage("fall", fallDistance);
            if (fallDistance >= minimumJump) StartCoroutine(PlaySound("jump/land"));
        }

        wasGrounded = isGrounded;
        wasFalling = isFalling;

        // It has to be FixedUpdate, because it applies force to the rigidbody constantly.
        rb.AddForce(Physics.gravity * gravityScale , ForceMode.Acceleration);
    }

    public void Update() {
        if (gameManager.gameIsPaused || isDead || PlayerPrefs.GetInt("wonGame") == 1) return;

        // update distance to ground
        distToGround = col.bounds.extents.y+0.3f;

        // handle weapon states and equipped weapons
        HandleWeapons();

        // handle player's attacks
        PlayerAttack();

        // handle player's movement
        PlayerMove();

        // handle player's footsteps
        HandleFootsteps();

        // handle player's reactions
        HandleReactions();

        // Debug info on screen
        GUIUpdate();
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Ocean") health = -100f;
        if (col.gameObject.name == "BossHudTrigger") {
            bossBattle = true;
        }
    }

    public void OnCollisionEnter(Collision col) {
        if (gameManager.gameIsPaused || isDead) return;

        switch (col.gameObject.tag) {
            case "Ladder":
                canJump = false;
                canChangeWeapons = false;
                canAttack = false;
                canRun = false;
                canClimb = !canClimb;
                break;
            case "Enemy/Tentacle":
                PlayerTakeDamage("tentacle", 10f);
                break;
            default:
                break;
        }
    }

    public void OnCollisionStay(Collision col) {
        if (gameManager.gameIsPaused || isDead) return;

        switch (col.gameObject.tag) {
            case "Ladder":
                HandleClimb();
                break;
            default:
                break;
        }
    }

    public void OnCollisionExit(Collision col) {
        if (gameManager.gameIsPaused || isDead) return;

        switch (col.gameObject.tag) {
            case "Ladder":
                canJump = true;
                canChangeWeapons = true;
                canAttack = true;
                canRun = true;
                canClimb = !canClimb;
                break;
            default:
                break;
        }
    }

    private bool isFalling {
        get {
            return (!isGrounded && rb.velocity.y < 0);
        }
    }

    private void CheckLife() {
        if (health <= 0 && !isDead) StartCoroutine(Die());
    }

    private void CheckGround() {
        isGrounded = Physics.Raycast(transform.position + Vector3.up, -Vector3.up, distToGround);
    }

    public void PlayerTakeDamage(string reason, float damage) {
        switch (reason) {
            case "fall":
                health -= damage;
                StartCoroutine(PlaySound(reason));
                break;
            case "tentacle":
                health -= damage;
                StartCoroutine(PlaySound(reason));
                break;
            default:
                health -= damage;
                break;
        }
        Debug.Log("Player " + reason + " damage: " + damage);
    }

    private void PlayerAttack() {
        Debug.DrawRay(transform.position + transform.up * 2, transform.TransformDirection(Vector3.forward * attackRange), Color.yellow);

        if (isDead || !canAttack) return;

        // left mouse click attack
        if (Input.GetMouseButtonDown(0)) {
            canAttack = false;
            animator.SetTrigger("Attack");

            Vector3 fxSpawnPosition = default;
            switch (weaponState) {
                case 1:
                    fxSpawnPosition = transform.position + transform.up * 2 + transform.forward * 2.85f;
                    Instantiate(fxSwordSlash, fxSpawnPosition, transform.rotation);
                    StartCoroutine(PlaySound("attack/sword"));
                    break;
                case 2:
                    fxSpawnPosition = transform.position + transform.up * 2 + transform.forward * 3.85f;
                    Instantiate(fxSwordSlash, fxSpawnPosition, transform.rotation);
                    StartCoroutine(PlaySound("attack/sword"));
                    break;
                case 3:
                    fxSpawnPosition = transform.position + transform.up * 2 + transform.forward * 3.85f;
                    Instantiate(fxSwordSlash, fxSpawnPosition, transform.rotation);
                    StartCoroutine(PlaySound("attack/spear"));
                    break;
                default:
                    fxSpawnPosition = transform.position + transform.up * 2 + transform.forward * 1.85f;
                    Instantiate(fxPunch, fxSpawnPosition, transform.rotation);
                    StartCoroutine(PlaySound("attack/punch"));
                    break;
            }
        }

        canAttack = true;
    }

    private void PlayerMove() {
        if (isDead) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput) != 0) {
            isMoving = true;
        } else {
            isMoving = false;
        }

        animator.SetFloat("Speed_h", horizontalInput);
        animator.SetFloat("Speed_v", verticalInput);

        // rotate object using mouse
        transform.Rotate(Vector3.up, horizontalMouseSpeed * Input.GetAxis("Mouse X"));

        thirdPersonCamera.transform.Rotate(Vector3.left, horizontalMouseSpeed * Input.GetAxis("Mouse Y"));
        // NETI 360 no cap 

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            transform.localScale -= scaleChange;
            Instantiate(magicPoof,transform.position,transform.rotation);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            transform.localScale += scaleChange;
            Instantiate(magicPoof,transform.position,transform.rotation);
        }

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            currentSpeed = runningSpeed;
            canRun = false;
        } else {
            currentSpeed = moveSpeed;
            canRun = true;
        }

        // move player horizontally
        rb.AddForce(transform.right * horizontalInput * currentSpeed, ForceMode.VelocityChange);
        // add vertical force to player to move him forward o backwards
        rb.AddForce(transform.forward * verticalInput * currentSpeed, ForceMode.VelocityChange);

        HandleJump();
        HandleSpeed();
    }

    private void HandleWeapons() {
        if (isDead || !canChangeWeapons) return;

        // change sword slots (models/skins) with scroll wheel if a sword is used
        if (weaponState == 1 || weaponState == 2) {
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f) {
                if (weaponSlot == weaponMaxSlots) weaponSlot = 1;
                else weaponSlot++;
                weaponSlotOld = weaponSlot;
            } else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f) {
                if (weaponSlot == 1) weaponSlot = weaponMaxSlots;
                else weaponSlot--;
                weaponSlotOld = weaponSlot;
            }
        }

        // unarmed
        if (Input.GetKeyDown("0")) {
            attackRange = 2;
            playerDamage = 7.5f;
            weaponSlot = 0;
            weaponState = 0;
            animator.SetInteger("weaponState", 0);
        }

        // 1handed sword
        if (Input.GetKeyDown("1")) {
            attackRange = 3;
            playerDamage = playerOneHandedSwordDamage;
            weaponSlot = weaponSlotOld;
            weaponState = 1;
            animator.SetInteger("weaponState", 1);
        }

        // 2handed sword
        if (Input.GetKeyDown("2")) {
            attackRange = 4;
            playerDamage = playerTwoHandedSwordDamage;
            weaponSlot = weaponSlotOld;
            weaponState = 2;
            animator.SetInteger("weaponState", 2);
        }

        // spear
        if (Input.GetKeyDown("3")) {
            attackRange = 4;
            playerDamage = playerSpearDamage;
            weaponSlot = weaponMaxSlots + 1;
            weaponState = 3;
            animator.SetInteger("weaponState", 3);
        }
    }

	private void HandleSpeed() {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVelocity.magnitude > moveSpeedLimit) {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeedLimit;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        // stop sliding when walk or run and then idle
        if (!isMoving) rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    private void HandleFootsteps() {
        if (!isGrounded) return;
        if (!isMoving) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0) {
            StartCoroutine(PlaySound("footsteps"));

            if (isMoving) {
                if (canRun) footstepTimer = baseStepSpeed;
                else footstepTimer = baseStepSpeed * sprintStepMultiplier;
            }
        }
    }

    private void HandleReactions() {
        if (Input.GetKeyDown("r")) {
            AudioClip[] reactClips = reactions[Random.Range(0, reactions.Count)];
            reactAudioSource.PlayOneShot(reactClips[Random.Range(0, reactClips.Length - 1)]);
        }
    }

    private void HandleJump() {
        if (!isGrounded || canClimb) return;
        if (Input.GetKeyDown(KeyCode.Space) && canJump) StartCoroutine(Jump());
        // Instantiate(fxSwordSlash, transform.position, Quaternion.identity);  // particles on ground when land
    }

    private void HandleClimb() {
        if (!canClimb) return;
        if (Input.GetKey("w")) transform.position += Vector3.up * climbSpeed * Time.deltaTime;
    }

	private void GUIUpdate() {
		debugText.text
            = "Health:" + health + "%" + "\n"
            + "Velocity:" + rb.velocity + "\n"
            + "Horizontal:" + Input.GetAxis("Horizontal") + "\n"
            + "Vertical:" + Input.GetAxis("Vertical") + "\n"
            + "MouseX:" + Input.GetAxis("Mouse X") + "\n"
            + "MouseY:" + Input.GetAxis("Mouse Y") + "\n"
            + "Last fall distance:" + startOfFall + "\n"
            + "playerDamage:" + playerDamage + "\n"
            + "isGrounded:" + isGrounded + "\n"
            + "isMoving:" + isMoving + "\n"
            + "canJump:" + canJump + "\n"
            + "canRun:" + canRun + "\n"
            + "canClimb:" + canClimb + "\n"
            + "canChangeWeapons:" + canChangeWeapons + "\n"
            + "canAttack:" + canAttack + "\n"
            ;
	}

    public IEnumerator Die() {
        isDead = true;
        animator.SetTrigger("Dead");

        yield return null;
    }

    public IEnumerator Jump() {
        canJump = false;

        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0) {
            StartCoroutine(PlaySound("jump/start"));
            if (canJump) jumpTimer = baseJumpSpeed;
        }

        animator.SetTrigger("Jump");
        rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);

        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0) {
            StartCoroutine(PlaySound("jump/land"));
            if (canJump) jumpTimer = baseJumpSpeed;
        }

        canJump = true;
    }

    public IEnumerator PlaySound(string reason) {
        switch (reason) {
            case "footsteps":
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3)) {                
                    switch (hit.collider.tag) {
                        case "Footsteps/Wood":
                            footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                            break;
                        case "Footsteps/Wood and Grass":
                            footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                            break;
                        case "Footsteps/Grass":
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                            break;
                        case "Footsteps/Ground and Grass":
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length - 1)]);
                            break;
                        case "Footsteps/Metal":
                            footstepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                            break;
                        case "Footsteps/Water":
                            footstepAudioSource.PlayOneShot(waterClips[Random.Range(0, waterClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass":
                            footstepAudioSource.PlayOneShot(waterClips[Random.Range(0, waterClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Ground":
                            footstepAudioSource.PlayOneShot(waterClips[Random.Range(0, waterClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass and Ground":
                            footstepAudioSource.PlayOneShot(waterClips[Random.Range(0, waterClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                            footstepAudioSource.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length - 1)]);
                            break;
                        default:
                            footstepAudioSource.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length - 1)]);
                            break;
                    }
                }
                break;
            case "jump/start":
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit_01, 3)) {       
                    jumpAudioSource.PlayOneShot(playerJumpClips[Random.Range(0, playerJumpClips.Length - 1)]);         
                    switch (hit_01.collider.tag) {
                        case "Footsteps/Wood":
                            jumpAudioSource.PlayOneShot(woodJumpClips[Random.Range(0, woodJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Wood and Grass":
                            jumpAudioSource.PlayOneShot(woodJumpClips[Random.Range(0, woodJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpClips[Random.Range(0, grassJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Grass":
                            jumpAudioSource.PlayOneShot(grassJumpClips[Random.Range(0, grassJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Ground and Grass":
                            jumpAudioSource.PlayOneShot(grassJumpClips[Random.Range(0, grassJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpClips[Random.Range(0, defaultJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Metal":
                            jumpAudioSource.PlayOneShot(metalJumpClips[Random.Range(0, metalJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Water":
                            jumpAudioSource.PlayOneShot(waterJumpClips[Random.Range(0, waterJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass":
                            jumpAudioSource.PlayOneShot(waterJumpClips[Random.Range(0, waterJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpClips[Random.Range(0, grassJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Ground":
                            jumpAudioSource.PlayOneShot(waterJumpClips[Random.Range(0, waterJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpClips[Random.Range(0, defaultJumpClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass and Ground":
                            jumpAudioSource.PlayOneShot(waterJumpClips[Random.Range(0, waterJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpClips[Random.Range(0, grassJumpClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpClips[Random.Range(0, defaultJumpClips.Length - 1)]);
                            break;
                        default:
                            jumpAudioSource.PlayOneShot(defaultJumpClips[Random.Range(0, defaultJumpClips.Length - 1)]);
                            break;
                    }
                }
                break;
            case "jump/land":
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit_02, 3)) {  
                    jumpAudioSource.PlayOneShot(playerJumpLandClips[Random.Range(0, playerJumpLandClips.Length - 1)]);
                    switch (hit_02.collider.tag) {
                        case "Footsteps/Wood":
                            jumpAudioSource.PlayOneShot(woodJumpLandClips[Random.Range(0, woodJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Wood and Grass":
                            jumpAudioSource.PlayOneShot(woodJumpLandClips[Random.Range(0, woodJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpLandClips[Random.Range(0, grassJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Grass":
                            jumpAudioSource.PlayOneShot(grassJumpLandClips[Random.Range(0, grassJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Ground and Grass":
                            jumpAudioSource.PlayOneShot(grassJumpLandClips[Random.Range(0, grassJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpLandClips[Random.Range(0, defaultJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Metal":
                            jumpAudioSource.PlayOneShot(metalJumpLandClips[Random.Range(0, metalJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Water":
                            jumpAudioSource.PlayOneShot(waterJumpLandClips[Random.Range(0, waterJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass":
                            jumpAudioSource.PlayOneShot(waterJumpLandClips[Random.Range(0, waterJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpLandClips[Random.Range(0, grassJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Ground":
                            jumpAudioSource.PlayOneShot(waterJumpLandClips[Random.Range(0, waterJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpLandClips[Random.Range(0, defaultJumpLandClips.Length - 1)]);
                            break;
                        case "Footsteps/Water and Grass and Ground":
                            jumpAudioSource.PlayOneShot(waterJumpLandClips[Random.Range(0, waterJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(grassJumpLandClips[Random.Range(0, grassJumpLandClips.Length - 1)]);
                            jumpAudioSource.PlayOneShot(defaultJumpLandClips[Random.Range(0, defaultJumpLandClips.Length - 1)]);
                            break;
                        default:
                            jumpAudioSource.PlayOneShot(defaultJumpLandClips[Random.Range(0, defaultJumpLandClips.Length - 1)]);
                            break;
                    }
                }
                break;
            case "fall":
                painAudioSource.PlayOneShot(painFallClips[Random.Range(0, painFallClips.Length - 1)]);
                break;
            case "react/shock":
                reactAudioSource.PlayOneShot(reactShockedClips[Random.Range(0, reactShockedClips.Length - 1)]);
                break;
            case "react/gasp":
                reactAudioSource.PlayOneShot(reactGaspClips[Random.Range(0, reactGaspClips.Length - 1)]);
                break;
            case "react/effort":
                reactAudioSource.PlayOneShot(reactEffortClips[Random.Range(0, reactEffortClips.Length - 1)]);
                break;
            case "react/laugh":
                reactAudioSource.PlayOneShot(reactLaughClips[Random.Range(0, reactLaughClips.Length - 1)]);
                break;
            case "react/nod":
                reactAudioSource.PlayOneShot(reactNodClips[Random.Range(0, reactNodClips.Length - 1)]);
                break;
            case "react/reflexion":
                reactAudioSource.PlayOneShot(reactReflexionClips[Random.Range(0, reactReflexionClips.Length - 1)]);
                break;
            case "react/sigh":
                reactAudioSource.PlayOneShot(reactSighClips[Random.Range(0, reactSighClips.Length - 1)]);
                break;
            case "react/cough":
                reactAudioSource.PlayOneShot(reactCoughClips[Random.Range(0, reactCoughClips.Length - 1)]);
                break;
            case "attack/sword":
                attackAudioSource.PlayOneShot(swordClips[Random.Range(0, swordClips.Length - 1)]);
                if (Physics.Raycast(new Ray(transform.position + transform.up * 2, transform.TransformDirection(Vector3.forward * attackRange)),
                                    out RaycastHit hit_03, attackRange, layerMask)) {
                    switch (hit_03.collider.tag) {
                        case "Enemy/Zombie":
                            attackAudioSource.PlayOneShot(hitFleshclips[Random.Range(0, hitFleshclips.Length - 1)]);
                            break;
                        case "Footsteps/Wood":
                            attackAudioSource.PlayOneShot(swordHitWoodClips[Random.Range(0, swordHitWoodClips.Length - 1)]);
                            break;
                        default:
                            attackAudioSource.PlayOneShot(swordHitMetalClips[Random.Range(0, swordHitMetalClips.Length - 1)]);
                            break;
                    }
                }
                break;
            case "attack/spear":
                attackAudioSource.PlayOneShot(spearClips[Random.Range(0, spearClips.Length - 1)]);
                if (Physics.Raycast(new Ray(transform.position + transform.up * 2, transform.TransformDirection(Vector3.forward * attackRange)),
                                    out RaycastHit hit_04, attackRange, layerMask)) {
                    switch (hit_04.collider.tag) {
                        case "Enemy/Zombie":
                            attackAudioSource.PlayOneShot(hitFleshclips[Random.Range(0, hitFleshclips.Length - 1)]);
                            break;
                        case "Footsteps/Wood":
                            attackAudioSource.PlayOneShot(swordHitWoodClips[Random.Range(0, swordHitWoodClips.Length - 1)]);
                            break;
                        default:
                            attackAudioSource.PlayOneShot(swordHitMetalClips[Random.Range(0, swordHitMetalClips.Length - 1)]);
                            break;
                    }
                }
                break;
            case "attack/punch":
                attackAudioSource.PlayOneShot(punchClips[Random.Range(0, punchClips.Length - 1)]);
                if (Physics.Raycast(new Ray(transform.position + transform.up * 2, transform.TransformDirection(Vector3.forward * attackRange)),
                                    out RaycastHit hit_05, attackRange)) {
                    switch (hit_05.collider.tag) {
                        case "Enemy/Zombie":
                            attackAudioSource.PlayOneShot(punchHitClips[Random.Range(0, punchHitClips.Length - 1)]);
                            attackAudioSource.PlayOneShot(hitFleshclips[Random.Range(0, hitFleshclips.Length - 1)]);
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                painAudioSource.PlayOneShot(painGenericClips[Random.Range(0, painGenericClips.Length - 1)]);
                break;
        }

        yield return null;
    }
}
