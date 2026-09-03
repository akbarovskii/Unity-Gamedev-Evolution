using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float stepDistance = 2f;
    public float turnDuration = 0.5f;
    public float moveDuration = 0.3f;
    public LayerMask obstacleMask;

    [Header("Breathing")]
    public float idleBobbingAmount = 0.02f;   
    public float idleBobbingSpeed = 1.5f;

    [Header("Audio")]
    public AudioClip[] footstepSounds;          
    public AudioClip breathingSound;             
    public AudioSource footstepSource;           
    public AudioSource breathingSource;          

    private Camera playerCamera;
    private float currentYaw = 0f;
    private float targetYaw = 0f;
    private float turnProgress = 1f;
    private float moveProgress = 1f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private float walkBobTimer = 0f;

    private float idleBobTimer = 0f;
    private Vector3 originalCameraLocalPos;

    private float nextBreathTime = 0f;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        originalCameraLocalPos = playerCamera.transform.localPosition;

        currentYaw = transform.eulerAngles.y;
        targetYaw = currentYaw;
        startPosition = transform.position;
        targetPosition = transform.position;

        if (breathingSource != null && breathingSound != null)
        {
            breathingSource.clip = breathingSound;
            breathingSource.loop = true;
            breathingSource.volume = 0.3f;
            breathingSource.Play();
        }

        nextBreathTime = Time.time + Random.Range(2f, 5f);
        
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            targetYaw -= 90f;
            turnProgress = 0f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            targetYaw += 90f;
            turnProgress = 0f;
        }

        if (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime / turnDuration;
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, turnProgress);
            transform.eulerAngles = new Vector3(0, currentYaw, 0);
        }

        if (Input.GetKeyDown(KeyCode.W) && !isMoving)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, stepDistance + 0.1f, obstacleMask))
            {
                Debug.Log("Wall ahead: " + hit.collider.name);
                return;
            }

            
            isMoving = true;
            moveProgress = 0f;
            startPosition = transform.position;
            targetPosition = transform.position + transform.forward * stepDistance;
            
            PlayFootstepSound();
        }

        if (isMoving)
        {
            moveProgress += Time.deltaTime / moveDuration;
            if (moveProgress >= 1f)
            {
                moveProgress = 1f;
                isMoving = false;
            }
            float t = Mathf.SmoothStep(0, 1, moveProgress);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            walkBobTimer += Time.deltaTime * 12f;
            float walkBobY = Mathf.Sin(walkBobTimer) * 0.04f * (1 - Mathf.Abs(t * 2 - 1));
            playerCamera.transform.localPosition = originalCameraLocalPos + new Vector3(0, walkBobY, 0);
            
            Cursor.visible = false;
        }
        else
        {
            walkBobTimer = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            idleBobTimer += Time.deltaTime * idleBobbingSpeed;
            float idleBobY = Mathf.Sin(idleBobTimer) * idleBobbingAmount;
            float idleBobX = Mathf.Sin(idleBobTimer * 0.7f) * (idleBobbingAmount * 0.5f);

            Vector3 targetPos = originalCameraLocalPos + new Vector3(idleBobX, idleBobY, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                targetPos,
                Time.deltaTime * 5f

            );

            if (Time.time >= nextBreathTime)
            {
                BreathSound();
                nextBreathTime = Time.time + Random.Range(8f, 15f);
            }
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSource == null || footstepSounds.Length == 0) return;

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        footstepSource.pitch = Random.Range(0.9f, 1.1f); 
        footstepSource.PlayOneShot(clip);
    }

    void BreathSound()
    {
        if (breathingSource == null) return;

        
        breathingSource.volume = 0.5f;
     
        Invoke(nameof(ResetBreathVolume), 0.5f);
    }

    void ResetBreathVolume()
    {
        if (breathingSource != null)
            breathingSource.volume = 0.3f;
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null) return;

        Gizmos.color = Color.red;
        Vector3 endPoint = playerCamera.transform.position + playerCamera.transform.forward * stepDistance;
        Gizmos.DrawLine(playerCamera.transform.position, endPoint);
    }
}
