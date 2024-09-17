using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private AnimationClip[] cellAnimations;

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxSpeed = 1.2f;        // The speed threshold beyond which drag will increase
    [SerializeField] private float increasedDrag = .8f;   // The drag value to apply when the speed is too high
    [SerializeField] private float normalDrag = .1f;      // The normal drag value when under the speed threshold

    private bool goingToDoActivity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        double rand = Random.value;
        if (rand > .66) goingToDoActivity = true;
    }

    public void DecideActivity()
    {
        if (goingToDoActivity) DoActivity();
        else MoveCell(); //Move cell if not animating - every action animation will have an event at the end that will tell the cell to move.

        //Randomly decides if, before the next movement, the cell is going to do an animation
        double rand = Random.value;
        if (rand > .66) goingToDoActivity = true;
        else goingToDoActivity = false;
    }

    private void DoActivity()
    {
        int rand = Random.Range(0, cellAnimations.Length);
        animator.Play(cellAnimations[rand].name);
    }

    private void MoveCell()
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


    public void SetCellAnimations(AnimationClip[] animations)
    {
        cellAnimations = animations;
    }
}
