using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialDriver : MonoBehaviour
{
    public static TutorialDriver Instance { get; private set; } // Singleton instance

    [SerializeField] private Transform player;

    [SerializeField] private InputListener inputListener;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AIController aiController;

    [SerializeField] private TextMeshProUGUI dialogueBox;
    private BoxCollider2D dialogueCollider; // Reference to the BoxCollider2D
    private List<string> dialogue;
    private int currentDialogueIndex = 0;

    private Coroutine autoAdvanceCoroutine;

    private void Awake()
    {
        // Implementing Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializeTutorialDialogue();
        DisplayCurrentDialogue();
        dialogueCollider = dialogueBox.GetComponent<BoxCollider2D>();
        SubscribeToCurrentDialogueEvents();
        ActivateSceneObjects(); // Activate scene objects at the start
    }

    private void InitializeTutorialDialogue()
    {
        dialogue = new List<string>
        {
            /*0*/ "Welcome to ImmunoDetective! This is a game where you'll experience the wonders of immunotherapy, a method of treatment used to treat certain kinds of cancer, such as lymphoma, leukemia, brain cancer, and others.",
            /*1*/ "Immunotherapy works by strengthening the immune system, effectively teaching it how to deal with unwanted cells, such as cancer cells.",
            /*2*/ "In this game, you'll be playing as a monoclonal antibody, a lab-made protein that is used to mimic natural antibodies in the human body. Your job is to find the cancer cells, and mark them for immune cells to eradicate.",
            /*3*/ "First, let's start with the basics. Tap and hold the screen to move in the direction you want to go.",
            /*4*/ "Great job! Now, let's take a look at the different kinds of cells you'll be encountering.",
            /*5*/ "These are normal cells. They're very important; they perform tasks that are important in maintaining a healthy body.",
            /*6*/ "Now, these are cancer cells. They're very dangerous. If they are allowed to multiply enough, they can be devastating to the body.",
            /*7*/ "You can tell these cells apart based on certain characteristics and behaviors. We can see that these cancer cells are different based on the way they are moving.",
            /*8*/ "Next, let's teach you how to take care of these pesky cells. The job of the monoclonal antibody is to bind to cancer cells, marking them for the immune system to destroy.",
            /*9*/ "Press the 'BIND' button to enter binding mode.",
            /*10*/ "With binding mode, you can bind to any cell and mark them to be destroyed. But be careful! We only want to destroy the cancer cells. Try binding to a cancer cell.",
            /*11*/ "Now, the cancer cell is emitting a signal that will get the attention of the immune system, which will send its cells to destroy it.",
            /*12*/ "This is the main goal. We want to destroy all of the cancer cells, and ensure that we maintain a healthy normal cell count, which is why it's important to make sure you're marking the correct cells!",
            /*13*/ "Observe the body's response to the marked cell: ",
            /*14*/ "Look there. The cancer cell that was destroyed has dropped a sample! Move over to it and pick it up.",
            /*15*/ "These are genetic samples of a cancer cell. They are crucial to our immunotherapy research, and they allow us to use more tools to dispose of cancer cells.",
            /*16*/ "These samples can be used to buy useful items to help you track down those pesky cancer cells. Let's open the shop and take a look!",
            /*17*/ "Let's purchase a vaccine!",
            /*18*/ "The vaccine will spawn a harmless cancer cell, which will allow you to analyze it. It is important to take note of the appearance and behaviors of this cell, as it will give you a better idea of which cells to target.",
            /*19*/ "Let's try using the vaccine! Tap the vaccine in your inventory to use it.",
            /*20*/ "This is a harmless cancer cell. Observe it carefully to have a good idea of which cells you should be targeting!",
            /*21*/ "Now, let's take a look at the second item in the shop. Let's purchase a protein analysis!",
            /*22*/ "The protein analysis will allow you to get a look at what proteins are present inside of a cell, which will help you determine if it is a normal cell or a cancer cell. Cancer cells will use certain proteins to hide themselves from the other cells.",
            /*23*/ "Use the protein analysis on this cell.",
            /*24*/ "As you can see, there are many proteins that are present in this cell. One to note in particular is the CD47 protein. This protein sends signals to other cells, as a way to protect the cancer cell from being destroyed.",
            /*25*/ "That means that this is a cancer cell, and it must be destroyed. Bind to the cell to mark it to be destroyed.",
            /*26*/ "Finally, let's take a look at the last item in the shop. Let's purchase a CAR T Cell!",
            /*27*/ "A CAR T Cell is a chimeric antigen receptor T cell that is produced from T cells in the patient's blood. It can find and kill cancer cells in the area.",
            /*28*/ "Let's use the CAR T Cell!",
            /*29*/ "As you can see, the CAR T Cell destroyed the cancer cells in this vicinity, dropping more genetic samples for you to use.",
            /*30*/ "This is the end of the tutorial level. Good luck!"
        };
    }

    private void DisplayCurrentDialogue()
    {
        dialogueBox.text = dialogue[currentDialogueIndex];
    }

    public void AdvanceDialogue()
    {
        currentDialogueIndex++;

        // Stop any running auto-advance coroutine if we're advancing manually
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        // If we've reached the end of dialogue, do nothing further
        if (currentDialogueIndex >= dialogue.Count)
        {
            Debug.Log("Tutorial completed");
            return;
        }

        DisplayCurrentDialogue();
        SubscribeToCurrentDialogueEvents(); // Update subscriptions for the new line
        ActivateSceneObjects(); // Activate scene objects for the new line
    }

    private void SubscribeToCurrentDialogueEvents()
    {
        // Unsubscribe from all events first to avoid duplicates
        UnsubscribeAllEvents();

        // Subscribe based on which dialogue line is currently displayed
        switch (currentDialogueIndex)
        {
            case 3: // Player movement advances this part
                TutorialEventManager.PlayerMoved += AdvanceDialogue;
                break;
            case 9: // Activating binding advances this part
                TutorialEventManager.BindingActivated += AdvanceDialogue;
                break;
            case 10: // Binding to a cancer cell advances this part
                TutorialEventManager.CellBound += AdvanceDialogue;
                break;
            case 13: // The T-cell destroying a marked cancer cell advances this part
                TutorialEventManager.CellDestroyed += AdvanceDialogue;
                break;
            case 14: // Picking up a sample advances this part
                TutorialEventManager.PlayerPickedSample += AdvanceDialogue;
                break;
            // Add cases for each part that needs special event handling
            default:
                // Start the auto-advance timer for this dialogue line
                autoAdvanceCoroutine = StartCoroutine(AutoAdvanceDialogue());
                break;
        }
    }
    private void ActivateSceneObjects()
    {
        // FIXME Cells can be spawned outside of the level bounds
        // The fix is simple: limit the positions to be within a certain radius of the origin   

        // Activate scene objects based on the current dialogue index
        switch (currentDialogueIndex)
        {
            case 3:
                inputListener.enabled = true;
                playerController.SetAllowBinding(false);
                break;
            case 5:
                aiController.CreateCell(0, new Vector3(0, 2, 0) + player.position);
                aiController.CreateCell(0, new Vector3(1.3f, 1.3f, 0) + player.position);
                aiController.CreateCell(0, new Vector3(2, 0, 0) + player.position);
                aiController.CreateCell(0, new Vector3(1.3f, -1.3f, 0) + player.position);
                aiController.CreateCell(0, new Vector3(0, -2, 0) + player.position);
                aiController.CreateCell(0, new Vector3(-1.3f, -1.3f, 0) + player.position);
                aiController.CreateCell(0, new Vector3(-2, 0, 0) + player.position);
                aiController.CreateCell(0, new Vector3(-1.3f, 1.3f, 0) + player.position);
                break;
            case 6:
                aiController.DestroyAllCells();
                aiController.CreateCell(1, new Vector3(2, 0, 0) + player.position);
                aiController.CreateCell(1, new Vector3(-2, 0, 0) + player.position);
                break;
            case 9:
                playerController.SetAllowBinding(true);
                break;
            case 13:
                aiController.CreateCell(2, new Vector3(0, 5, 0) + player.position);
                break;

            default:
                // No specific objects to activate
                break;
        }
    }

    private void UnsubscribeAllEvents()
    {
        TutorialEventManager.PlayerMoved -= AdvanceDialogue;
        TutorialEventManager.BindingActivated -= AdvanceDialogue;
        TutorialEventManager.CellBound -= AdvanceDialogue;
        TutorialEventManager.CellDestroyed -= AdvanceDialogue;
        TutorialEventManager.PlayerPickedSample -= AdvanceDialogue;
    }

    // Coroutine to handle auto-advancing dialogue after a set time
    private IEnumerator AutoAdvanceDialogue()
    {
        string currentText = dialogueBox.text;
        int textLength = currentText.Length;

        // Calculate timer length: 2 seconds + 0.05 seconds per character
        float timer = 2f + (0.05f * textLength);

        // Wait for the calculated amount of time
        yield return new WaitForSeconds(timer);

        // Automatically advance the dialogue
        AdvanceDialogue();
    }

    public bool CanClickToAdvance()
    {
        List<int> promptDialogueIndices = new List<int> { 3, 9, 10, 13, 14 }; // Indices where clicking is not allowed
        return !promptDialogueIndices.Contains(currentDialogueIndex);
    }
}
