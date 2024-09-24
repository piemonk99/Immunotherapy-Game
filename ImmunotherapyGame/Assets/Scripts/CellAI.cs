using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private AnimationClip[] cellAnimations;
    private CellSpecifications cellSpecifications;

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxSpeed = 1.2f;        //The speed threshold beyond which drag will increase
    [SerializeField] private float increasedDrag = .8f;   //The drag value to apply when the speed is too high
    [SerializeField] private float normalDrag = .1f;      //The normal drag value when under the speed threshold

    private bool isCancer { get; set; }
    private float replicationTime; //Time in seconds before the cell replicates
    private float replicationTimer; //Countdown timer for replication

    private bool goingToDoActivity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //Initialize random decision for activity
        double rand = Random.value;
        if (rand > .66) goingToDoActivity = true;
    }

    private void Start()
    {
        //Initialize the replication timer
        float randomTimeCoefficient = (Random.value * .4f) + .8f;
        replicationTimer = replicationTime * randomTimeCoefficient;
    }

    private void Update()
    {
        HandleReplicationTimer();
    }

    //Method to handle the replication countdown
    private void HandleReplicationTimer()
    {
        replicationTimer -= Time.deltaTime;

        if (replicationTimer <= 0)
        {
            //Trigger replication when the timer reaches zero
            Replicate();
            float randomTimeCoefficient = (Random.value * .4f) + .8f;
            replicationTimer = replicationTime * randomTimeCoefficient; //Reset the timer after replication
        }
    }

    private void Replicate()
    {
        FindObjectOfType<AIController>().ReplicateCell(gameObject); // Call replication from AIController
    }

    public void DecideActivity()
    {
        if (goingToDoActivity) DoActivity();
        else MoveCell(); // Move cell if not animating - every action animation will have an event at the end that will tell the cell to move.

        // Randomly decides if, before the next movement, the cell is going to do an animation
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

        // Normalize velocity to match the set speed
        float normalizationFactor = speed / (Mathf.Abs(xVelocity) + Mathf.Abs(yVelocity));

        rb.AddForce(new Vector2(xVelocity * normalizationFactor, yVelocity * normalizationFactor) * 400f);
    }

    void FixedUpdate()
    {
        AdjustDragBasedOnSpeed();
    }

    //This function increases the drag when velocity exceeds maxSpeed
    private void AdjustDragBasedOnSpeed()
    {
        //Check if the velocity exceeds maxSpeed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.drag = increasedDrag;  //Apply increased drag to slow down the object
        }
        else
        {
            rb.drag = normalDrag;     //Reset to normal drag
        }
    }

    public void SetRandomizedCellParameters(Color borderColor, Color centerColor, GameObject[] randomShapes, Color[] randomShapeColors)
    {
        cellSpecifications = new CellSpecifications(borderColor, centerColor, randomShapes, randomShapeColors);

        Transform border = transform.Find("Border");
        Transform center = transform.Find("Center");

        // Sets border and center colors
        border.GetComponent<SpriteRenderer>().color = borderColor;
        center.GetComponent<SpriteRenderer>().color = centerColor;

        // Instantiates all random shapes in the cell and sets their colors
        for (int i = 0; i < randomShapes.Length; i++)
        {
            GameObject shape = Instantiate(randomShapes[i], center.Find($"Shape{i + 1}"));
            SpriteRenderer[] shapeSpriteRenderers = shape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer renderer in shapeSpriteRenderers)
            {
                renderer.color = randomShapeColors[i];
            }
        }
    }

    public void SetCellBehavior(bool isCancer, AnimationClip[] animations)
    {
        this.isCancer = isCancer;

        if (isCancer)
        {
            cellAnimations = animations; //Cancer cells get all 5 used animations 
            replicationTime = 10; //Cancer cells replicate time
        }
        else
        {
            cellAnimations = animations.Take(animations.Length - 1).ToArray(); //Regular cells get all but the last animation
            replicationTime = 40; //Regular cell replication time
        }
    }

    public bool GetIsCancer()
    {
        return isCancer;
    }

    public CellSpecifications GetSpecifications()
    {
        return cellSpecifications;
    }
}


public class CellSpecifications
{
    public Color borderColor;
    public Color centerColor;
    public GameObject[] randomShapes;
    public Color[] randomShapeColors;

    public CellSpecifications(Color borderColor, Color centerColor, GameObject[] randomShapes, Color[] randomShapeColors)
    {
        this.borderColor = borderColor;
        this.centerColor = centerColor;
        this.randomShapes = randomShapes;
        this.randomShapeColors = randomShapeColors;
    }
}

