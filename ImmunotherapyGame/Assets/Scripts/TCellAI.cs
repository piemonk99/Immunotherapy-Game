using UnityEngine;

public class TCellAI : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed = 1;
    [SerializeField] private float maxSpeed = .6f;        //The speed threshold beyond which drag will increase
    [SerializeField] private float increasedDrag = .8f;   //The drag value to apply when the speed is too high
    [SerializeField] private float normalDrag = .1f;      //The normal drag value when under the speed threshold
    [SerializeField] private float chaseIntervalMin = 0.1f;
    [SerializeField] private float chaseIntervalMax = 0.3f;
    [SerializeField] private float detectCancerChance = 0.05f; // Chance per second per cancer cell that a CAR T cell will target the cancer cell

    private CellFactory cellFactory;
    private CellAI target;

    private float wanderDelay;
    private float chaseDelay;

    private bool isInTutorial;

    private bool detectCancer;

    private Pathfinder pathfinder;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isInTutorial = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene"; // Check if current scene is TutorialScene
        pathfinder = GetComponent<Pathfinder>();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            CellAI nearestTarget = null;
            float nearestTargetDistance = 0f;

            foreach (CellAI cell in cellFactory.GetCells())
            {
                float distance = pathfinder.DistanceToNode(pathfinder.GetNearestNode(cell.transform.position));

                if ((cell.IsMarked() || (detectCancer && Random.Range(0f, 1f) < detectCancerChance * Time.fixedDeltaTime)) && (nearestTarget == null || distance < nearestTargetDistance))
                {
                    nearestTarget = cell;
                    nearestTargetDistance = distance;
                }
            }

            target = nearestTarget;

            if (target != null)
                pathfinder.SetDestination(pathfinder.GetNearestNode(target.transform.position));
            else
                pathfinder.SetDestination(null);

            // Randomly wander when no cells are marked
            wanderDelay -= Time.fixedDeltaTime;

            if (wanderDelay <= 0)
            {
                Wander();
                wanderDelay += Random.Range(1f, 5f);
            }
        }
        else
        {
            chaseDelay -= Time.fixedDeltaTime;

            if (chaseDelay <= 0)
            {
                Vector3 targetPosition = target.transform.position;
                PathfindingNode targetNode = pathfinder.GetNearestNode(targetPosition);

                if (pathfinder.GetDestination() != targetNode)
                    pathfinder.SetDestination(targetNode);

                if (pathfinder.GetNextNode() != null)
                    targetPosition = pathfinder.GetNextNode().transform.position;

                MoveCell(targetPosition - transform.position);
                chaseDelay += Random.Range(chaseIntervalMin, chaseIntervalMax);
            }
        }

        AdjustDragBasedOnSpeed();
    }

    private void Wander()
    {
        MoveCell(Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector2.right);
    }

    private void MoveCell(Vector2 velocity)
    {
        float normalizationFactor = speed / (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y));
        rb.AddForce(velocity * normalizationFactor * 400f);
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

        if (cell != null && (cell.IsMarked() || cell == target))
        {
            cell.Die();
            cellFactory.GetCells().Remove(cell);

            if (isInTutorial && cell.GetIsCancer())
            {
                TutorialEventManager.DoCellDestroyed();
            }
        }
    }

    public void SetDetectCancer(bool value)
    {
        detectCancer = value;
    }
}
