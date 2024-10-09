using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject shopPanel;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementInput;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    private bool bindingMode;
    private int currency;
    private bool allowBinding = true;
    private CellAI bindingTo;
    private float bindingTime;

    [SerializeField] private float bindingDistance = 0.01f;
    [SerializeField] private float maxBindingTime = 3; // Stop trying to bind after spending this amount of time trying to bind in order to prevent softlocking when the antigen is near a wall or something

    [SerializeField] private TextMeshProUGUI currencyAmountText;

    [SerializeField] private bool isInTutorial;

    private CellAI clickedCell;
    private float clickedCellAt;
    private bool clickedShop;

    [SerializeField] private float doubleClickDelay = 0.4f;

    [SerializeField] private Transform spritePivot;
    [SerializeField] private Collider2D[] colliders;

    private bool bindingAnimationTriggered;

    [SerializeField] private Inventory inventory;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
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
                    if (clickedShop || clickedCell != cellAI || Time.time - clickedCellAt > doubleClickDelay)
                    {
                        clickedCell = cellAI;
                        clickedCellAt = Time.time;
                        clickedShop = false;
                    }
                    else
                    {
                        infoPanel.SetActive(true);
                        infoPanel.GetComponent<InfoPanel>().UpdateInfo(cellAI, this);
                        clickedCell = null;
                        clickedShop = false;
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Shop"))
                {
                    if (!clickedShop || clickedCell != null || Time.time - clickedCellAt > doubleClickDelay)
                    {
                        clickedCell = null;
                        clickedCellAt = Time.time;
                        clickedShop = true;
                    }
                    else
                    {
                        shopPanel.SetActive(true);
                        shopPanel.GetComponent<Shop>().SetShopPosition(hit.collider.transform.position);
                        clickedCell = null;
                        clickedShop = false;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = movementInput;

        if (bindingTo != null)
        {
            if (Vector2.Distance(transform.position, bindingTo.GetBindingPoint().position) <= bindingDistance)
            {
                rb.velocity = Vector2.zero;
                movement = Vector2.zero;

                if (!bindingAnimationTriggered)
                {
                    animator.SetTrigger("Bind");
                    bindingAnimationTriggered = true;
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    if (isInTutorial && bindingTo.GetIsCancer())
                        TutorialEventManager.DoCellBound();

                    bindingTo.Mark();
                    bindingTo.SetBinding(false);
                    Collider2D cellCollider = bindingTo.GetComponent<Collider2D>();

                    foreach (Collider2D collider in colliders)
                        Physics2D.IgnoreCollision(collider, cellCollider, false);

                    bindingTo = null;
                    bindingAnimationTriggered = false;
                    animator.ResetTrigger("Bind");
                }
            }
            else
            {
                movement = ((Vector2)(bindingTo.GetBindingPoint().position - transform.position)).normalized;
                bindingTime += Time.fixedDeltaTime;

                // Stop trying to bind after a certain amount of time to prevent softlocks
                if (bindingTime >= maxBindingTime)
                {
                    bindingTo.SetBinding(false);
                    Collider2D cellCollider = bindingTo.GetComponent<Collider2D>();

                    foreach (Collider2D collider in colliders)
                        Physics2D.IgnoreCollision(collider, cellCollider, false);

                    bindingTo = null;
                }
            }
        }
        else if (shopPanel.gameObject.activeInHierarchy)
            movementInput = Vector2.zero;

        rb.AddForce(movement * acceleration);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, moveSpeed);

        if (bindingTo != null)
        {
            RotateTowardsMovement(bindingTo.GetBindingPoint().up);
        }
        else if (rb.velocity.magnitude > 0.1f)
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
        if (!allowBinding || bindingTo != null)
            return;

        bindingMode = !bindingMode;
        spritePivot.localRotation = Quaternion.Euler(0, 0, bindingMode ? -90 : 90);

        if (isInTutorial)
        {
            // Call the bound event for tutorial
            TutorialEventManager.DoBindingActivated();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Currency"))
        {
            ++currency;
            currencyAmountText.text = $"{currency}";

            if (other.transform.parent.gameObject.name == "sampleParent")
            {
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }

            if (isInTutorial)
            {
                // Call the bound event for tutorial
                TutorialEventManager.DoPlayerPickedSample();
            }
        }
        else if (bindingMode)
        {
            var cell = other.GetComponentInParent<CellAI>();

            if (cell != null && !cell.IsMarked())
            {
                cell.SetBinding(true);
                bindingTo = cell;
                Collider2D cellCollider = cell.GetComponent<Collider2D>();      

                foreach (Collider2D collider in colliders)
                    Physics2D.IgnoreCollision(collider, cellCollider);

                rb.velocity = Vector2.zero;
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

    public Inventory GetInventory()
    {
        return inventory;
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void SetCurrency(int amount)
    {
        currency = amount;
        currencyAmountText.text = $"{currency}";
    }
}
