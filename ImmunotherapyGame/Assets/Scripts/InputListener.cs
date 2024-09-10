using UnityEngine;

public class InputListener : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found on the GameObject!");
        }
    }

    void Update()
    {
        //Capture WASD or Arrow keys input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        //Send the input to PlayerController to move the player
        if (playerController != null)
        {
            playerController.MovePlayer(moveHorizontal, moveVertical);
        }
    }
}

