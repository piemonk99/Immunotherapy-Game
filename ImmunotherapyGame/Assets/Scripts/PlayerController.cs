using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        mainCamera.transform.position = transform.position + new Vector3(0, 0, -10);
    }

    public void MovePlayer(float moveHorizontal, float moveVertical)
    {
        //Get input as a normalized vector
        movementInput = new Vector2(moveHorizontal, moveVertical).normalized;

        //Apply force to the player, scaled by acceleration
        rb.AddForce(movementInput * acceleration);

        //Clamp the player's velocity to the max move speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed);

        //Rotate the player to face movement direction if moving
        if (rb.velocity.magnitude > 0.1f)
        {
            RotateTowardsMovement(rb.velocity);
        }
    }

    private void RotateTowardsMovement(Vector2 movementDirection)
    {
        //Calculate the angle to rotate based on movement direction
        float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;

        //Lerp the rotation to smoothly face the movement direction
        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationLerpSpeed * Time.deltaTime);

        //Apply the smoothed rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
    }
}
