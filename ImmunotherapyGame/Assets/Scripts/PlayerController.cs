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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
                    infoPanel.SetActive(true);
                    infoPanel.GetComponent<InfoPanel>().UpdateInfo(cellAI);
                }
            }
        }

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

    public void ToggleBindingMode()
    {
        bindingMode = !bindingMode;
        bindingModeText.text = bindingMode ? "Binding Mode Enabled" : "";
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!bindingMode)
            return;

        CellAI cell = collision.gameObject.GetComponent<CellAI>();

        if (cell != null && !cell.IsMarked())
            cell.Mark();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Currency"))
        {
            ++currency;
            currencyAmountText.text = $"{currency}";
            Destroy(other.gameObject);
        }
    }
}
