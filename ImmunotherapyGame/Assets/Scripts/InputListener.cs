using UnityEngine;
using UnityEngine.SceneManagement;

public class InputListener : MonoBehaviour
{
    private PlayerController playerController;

    // Track the movement keys for the tutorial
    private bool horizontalMovedPositive = false;
    private bool horizontalMovedNegative = false;
    private bool verticalMovedPositive = false;
    private bool verticalMovedNegative = false;
    private bool tutorialActive = false;

    private Vector2 tapStartPosition;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found on the GameObject!");
        }

        // Set tutorialActive to true if the scene's title is 'TutorialScene'
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            tutorialActive = true;
        }
    }

    void Update()
    {
        // Capture WASD or Arrow keys input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0))
            tapStartPosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector2 tapDirection = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - tapStartPosition).normalized;
            moveHorizontal = tapDirection.x;
            moveVertical = tapDirection.y;
        }

        // Send the input to PlayerController to move the player
        if (playerController != null)
        {
            playerController.MovePlayer(moveHorizontal, moveVertical);

            // Check if the movement keys have been pressed for tutorial purposes
            if (tutorialActive)
            {
                TrackMovementForTutorial(moveHorizontal, moveVertical);
            }

            // Handle binding mode toggle
            if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonUp(0) && Vector2.Distance(Input.mousePosition, new Vector2(Screen.width / 2, Screen.height / 2)) < 50))
            {
                playerController.ToggleBindingMode();
            }
        }
    }

    // Tutorial-specific function to track movement keys and trigger event
    private void TrackMovementForTutorial(float moveHorizontal, float moveVertical)
    {
        // Track horizontal movement
        if (moveHorizontal > 0) horizontalMovedPositive = true;
        if (moveHorizontal < 0) horizontalMovedNegative = true;

        // Track vertical movement
        if (moveVertical > 0) verticalMovedPositive = true;
        if (moveVertical < 0) verticalMovedNegative = true;

        // Check if both horizontal and vertical movements have been detected in both directions
        if (horizontalMovedPositive && horizontalMovedNegative && verticalMovedPositive && verticalMovedNegative)
        {
            TutorialEventManager.DoPlayerMoved();
            // Disable further checks for this tutorial condition
            tutorialActive = false;
        }
    }

    // Method to activate tutorial-specific input tracking
    public void ActivateTutorialTracking()
    {
        tutorialActive = true;
        horizontalMovedPositive = false;
        horizontalMovedNegative = false;
        verticalMovedPositive = false;
        verticalMovedNegative = false;
    }
}
