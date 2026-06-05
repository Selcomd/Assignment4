using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float lookLimit = 80f;

    [Header("Smoke Screen")]
    [SerializeField] private KeyCode smokeKey = KeyCode.R;
    [SerializeField] private float smokeDuration = 5f;
    [SerializeField] private ParticleSystem smokeEffect;

    [Header("Player Audio")]
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip smokeBombSound;
    [SerializeField] private float runSoundInterval = 0.4f;
    [SerializeField] [Range(0f, 1f)] private float audioVolume = 1f;

    private bool smokeAvailable = true;
    private bool smokeActive = false;
    private float smokeTimer = 0f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;

    private bool wasGrounded = true;
    private float runSoundTimer = 0f;

    public bool SmokeAvailable => smokeAvailable;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) playerCamera = cam.transform;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleSmokeScreen();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && !wasGrounded) PlaySound(landSound);

        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        bool isMoving = moveDirection.magnitude > 0.1f;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (isGrounded && isMoving)
        {
            runSoundTimer -= Time.deltaTime;
            if (runSoundTimer <= 0f)
            {
                PlaySound(runSound);
                runSoundTimer = runSoundInterval;
            }
        }
        else
        {
            runSoundTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlaySound(jumpSound);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        wasGrounded = isGrounded;
    }

    private void HandleSmokeScreen()
    {
        if (Input.GetKeyDown(smokeKey) && smokeAvailable && !smokeActive)
        {
            smokeAvailable = false;
            smokeActive = true;
            smokeTimer = smokeDuration;

            GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);
            foreach (GuardAI guard in guards) guard.ApplySmokeScreen(smokeDuration);

            if (smokeEffect != null)
            {
                smokeEffect.gameObject.SetActive(true);
                smokeEffect.Play();
            }

            PlaySound(smokeBombSound);

            Debug.Log("Smoke screen activated!");
        }

        if (smokeActive)
        {
            smokeTimer -= Time.deltaTime;
            if (smokeTimer <= 0f)
            {
                smokeActive = false;

                if (smokeEffect != null) smokeEffect.Stop();

                Debug.Log("Smoke screen worn off.");
            }
        }
    }

    public void RestoreSmokeScreen()
    {
        smokeAvailable = true;
        smokeActive = false;
        smokeTimer = 0f;

        if (smokeEffect != null) smokeEffect.Stop();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, transform.position, audioVolume);
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookLimit, lookLimit);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}