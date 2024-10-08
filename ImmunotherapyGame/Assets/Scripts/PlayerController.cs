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
    private bool allowBinding = true;

    [SerializeField] private TextMeshProUGUI currencyAmountText;

    [SerializeField] private bool isInTutorial;

    private CellAI clickedCell;
    private float clickedCellAt;

    [SerializeField] private float doubleClickDelay = 0.4f;

    private Animation anim;

    [SerializeField] private Transform spritePivot;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animation>();
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

    private void FixedUpdate()
    {
        rb.AddForce(movementInput * acceleration);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed);

        if (rb.velocity.magnitude > 0.1f)
        {
            RotateTowardsMovement(rb.velocity);
        }
    }

    public void MovePlayer(float moveHorizontal, float moveVertical)
    {
        movementInput = new Vector2(moveHorizontal, moveVertical).normalized;
    }

    private void RotateTowardsMovement(Vector2 movementDirection)
    {
        float targetAngle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;

        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationLerpSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
    }

    public void ToggleBindingMode()
    {
        if (!allowBinding)
            return;

        bindingMode = !bindingMode;
        spritePivot.localRotation = Quaternion.Euler(0, 0, bindingMode ? -90 : 90);

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

    public void SetAllowBinding(bool allow)
    {
        allowBinding = allow;
    }

    public bool IsBindingModeEnabled()
    {
        return bindingMode;
    }
}
