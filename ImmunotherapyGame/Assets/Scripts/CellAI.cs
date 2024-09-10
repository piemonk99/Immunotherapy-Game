using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAI : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 1;

    [SerializeField] private float maxSpeed = 1.2f;        // The speed threshold beyond which drag will increase
    [SerializeField] private float increasedDrag = .8f;   // The drag value to apply when the speed is too high
    [SerializeField] private float normalDrag = .1f;      // The normal drag value when under the speed threshold

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void DecideActivity()
    {
        float xVelocity = Random.Range(-1f, 1f);
        float yVelocity = Random.Range(-1f, 1f);

        //This factor represents a fraction we can multiply by to get the total velocity to the cell's speed
        float normilizationFactor = speed / (Mathf.Abs(xVelocity) + Mathf.Abs(yVelocity));

        rb.AddForce(new Vector2(xVelocity * normilizationFactor, yVelocity * normilizationFactor) * 400f);
    }

    

    void FixedUpdate()
    {
        AdjustDragBasedOnSpeed();
    }

    // This function increases the drag when velocity exceeds maxSpeed
    private void AdjustDragBasedOnSpeed()
    {
        // Check if the velocity exceeds the specified threshold
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.drag = increasedDrag;  // Apply increased drag to slow down the object
        }
        else
        {
            rb.drag = normalDrag;     // Reset to normal drag
        }
    }
}
