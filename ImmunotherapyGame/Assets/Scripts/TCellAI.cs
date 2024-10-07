using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TCellAI : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxSpeed = 1.2f;        //The speed threshold beyond which drag will increase
    [SerializeField] private float increasedDrag = .8f;   //The drag value to apply when the speed is too high
    [SerializeField] private float normalDrag = .1f;      //The normal drag value when under the speed threshold
    [SerializeField] private float chaseIntervalMin = 0.1f;
    [SerializeField] private float chaseIntervalMax = 0.3f;

    private CellFactory cellFactory;
    private CellAI target;

    private float wanderDelay;
    private float chaseDelay;

    private bool isInTutorial;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isInTutorial = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene"; // Check if current scene is TutorialScene
    }

    private void Update()
    {
        if (target == null)
        {
            CellAI nearestTarget = null;
            float nearestTargetDistance = 0f;

            foreach (CellAI cell in cellFactory.GetCells())
            {
                float distance = Vector2.Distance(cell.transform.position, transform.position);

                if (cell.IsMarked() && (nearestTarget == null || distance < nearestTargetDistance))
                {
                    nearestTarget = cell;
                    nearestTargetDistance = distance;
                }
            }

            target = nearestTarget;
            // Randomly wander when no cells are marked
            wanderDelay -= Time.deltaTime;

            if (wanderDelay <= 0)
            {
                Wander();
                wanderDelay += Random.Range(1f, 5f);
            }
        }
        else
        {
            chaseDelay -= Time.deltaTime;

            if (chaseDelay <= 0)
            {
                MoveCell(target.transform.position - transform.position);
                chaseDelay += Random.Range(chaseIntervalMin, chaseIntervalMax);
            }
        }
    }

    private void Wander()
    {
        MoveCell(Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector2.right);
    }

    private void MoveCell(Vector2 velocity)
    {
        float normalizationFactor = speed / (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y));
        rb.AddForce(velocity * normalizationFactor * 400f * Time.deltaTime * 60);
    }

    void FixedUpdate()
    {
        AdjustDragBasedOnSpeed();
    }

    private void AdjustDragBasedOnSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.drag = increasedDrag;
        }
        else
        {
            rb.drag = normalDrag;
        }
    }

    public void SetCellFactory(CellFactory cellFactoryIn)
    {
        cellFactory = cellFactoryIn;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        CellAI cell = other.gameObject.GetComponent<CellAI>();

        if (cell != null && cell.IsMarked())
        {
            cell.Die();
            cellFactory.GetCells().Remove(cell);
            if (isInTutorial && cell.GetIsCancer())
            {
                TutorialEventManager.DoCellDestroyed();
            }
        }
    }
}
