using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject infoPanel;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    private bool bindingMode;
    private int currency;

    [SerializeField] private TextMeshProUGUI bindingModeText;
    [SerializeField] private TextMeshProUGUI currencyAmountText;

    private bool isInTutorial; // Flag to check if we are in the tutorial scene

    private CellAI clickedCell;
    private float clickedCellAt;

    [SerializeField] private float doubleClickDelay = 0.4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isInTutorial = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TutorialScene"; // Check if current scene is TutorialScene
    }

    private void Update()
    {
        mainCamera.transform.position = transform.position + new Vector3(0, 0, -10);

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                CellAI cellAI = hit.transform.GetComponent<CellAI>();
                if (cellAI != null)
                {
                    if (clickedCell != cellAI || Time.time - clickedCellAt > doubleClickDelay)
                    {
                        clickedCell = cellAI;
                        clickedCellAt = Time.time;
                    }
                    else
                    {
                        infoPanel.SetActive(true);
                        infoPanel.GetComponent<InfoPanel>().UpdateInfo(cellAI);
                        clickedCell = null;
                    }
                }
            }
        }
    }

    public void MovePlayer(float moveHorizontal, float moveVertical)
    {
        movementInput = new Vector2(moveHorizontal, moveVertical).normalized;

        rb.AddForce(movementInput * acceleration * Time.deltaTime * 60);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed);

        if (rb.velocity.magnitude > 0.1f)
        {
            RotateTowardsMovement(rb.velocity);
        }
    }

    private void RotateTowardsMovement(Vector2 movementDirection)
    {
        float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;

        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationLerpSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
    }

    public void ToggleBindingMode()
    {
        bindingMode = !bindingMode;
        bindingModeText.text = bindingMode ? "Binding Mode Enabled" : "";

        if (isInTutorial)
        {
            // Call the bound event for tutorial
            TutorialEventManager.DoBindingActivated();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!bindingMode)
            return;

        CellAI cell = collision.gameObject.GetComponent<CellAI>();

        if (cell != null && !cell.IsMarked())
        {
            cell.Mark();

            if (isInTutorial && cell.GetIsCancer())
            {
                // Call the bound event for tutorial
                TutorialEventManager.DoCellBound();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Currency"))
        {
            ++currency;
            currencyAmountText.text = $"{currency}";
            Destroy(other.gameObject);

            if (isInTutorial)
            {
                // Call the bound event for tutorial
                TutorialEventManager.DoPlayerPickedSample();
            }
        }
    }
}
