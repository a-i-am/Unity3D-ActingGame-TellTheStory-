using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    public float walkSpeed = 6;
    public float jumpForce = 220;
    public LayerMask groundedMask;

    bool grounded;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    Vector3 moveDir;
    Vector3 lastMoveDir;
    float verticalLookRotation;
    Transform cameraTransform;

    Animator anim;
    Rigidbody rb;


    void Awake()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Calculate movement:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector3(inputX, 0, inputY).normalized;

        // 카메라의 방향을 고려하여 이동 방향을 계산
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f; // 카메라의 y 방향을 무시하여 수평 방향만 사용
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f; // 마찬가지로 수평 방향만 사용
        right.Normalize();

        // 이동 방향을 카메라의 회전 방향을 기준으로 계산
        Vector3 targetMoveAmount = (forward * moveDir.z + right * moveDir.x) * walkSpeed;

        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        // Jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        // Grounded check
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    void FixedUpdate()
    {
            MoveRb();
    }

    void MoveRb()
    {
        // Apply movement to rb
        Vector3 localMove = new Vector3(moveAmount.x, 0, moveAmount.z) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
}
