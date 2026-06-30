using EightDirectionalSpriteSystem;
using Unity.Mathematics;
using UnityEngine;
enum PlayerState
{
    Moving,
    InDialogue
}

public class PlayerController : MonoBehaviour
{
    PlayerState currentState;
    Transform cameraTransform;
    Animator anim;
    Rigidbody rb;

    bool isGrounded;
    float verticalLookRotation;

    [SerializeField] private LayerMask isGroundedLayer;

    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 6;
    [SerializeField] float jumpForce = 100;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = CheckIsGrounded();

        if (isGrounded && !(currentState == PlayerState.InDialogue)
            && Input.GetButtonUp("Jump"))
        {
            ApplyJump();
        }
        else currentState = PlayerState.Moving;

        switch (currentState)
        {
            case PlayerState.Moving:
                HandleMovementInput();
                break;
            case PlayerState.InDialogue:
                HandleDialogue();
                break;
        }
    }
    void FixedUpdate()
    {
        if (currentState != PlayerState.Moving) return;
        ApplyMovement();
    }

    void SetState(PlayerState newState)
    {
        currentState = newState;
    }
    bool CheckIsGrounded()
    {

        Ray ray = new Ray(transform.position, Vector3.down);


        Debug.DrawRay(ray.origin, ray.direction * 0.6f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.6f, isGroundedLayer))
        {
            return isGrounded = true;
        }
        else
        {
            return isGrounded = false;
        }
    }

    void HandleMovementInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;


        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();


        Vector3 targetMoveAmount = (forward * moveDir.z + right * moveDir.x) * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
    }
    void HandleDialogue()
    {

        moveAmount = Vector3.zero;

        if (!DialogueManager.instance.isDialogueActive)
        {
            SetState(PlayerState.Moving);
        }
    }
    void ApplyMovement()
    {
        Vector3 localMove = new Vector3(moveAmount.x, 0, moveAmount.z) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);

        if (moveAmount != Vector3.zero)
        {
            SoundManager.instance.StartWalking();
        }
        else
        {
            SoundManager.instance.StopWalking();
        }
    }
    void ApplyJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce);
    }

}
