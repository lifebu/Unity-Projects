using UnityEngine;
using UnityEngine.UI;

public class IngameUIController : MonoBehaviour
{
    // The CommandDisplay Toggles
    private Toggle attackToggle;
    private Toggle patrolToggle;
    private Toggle moveToggle;
    public Toggle holdToggle;

    // The Cursors should actually be Ressources :)
    private Texture2D normalCursor;
    private Texture2D attackCursor;
    private Texture2D moveCursor;
    private Texture2D patrolCursor;

    private static IngameUIController _instance;
    public static IngameUIController instance
    {
        get
        {
            if (!_instance)
            {
                IngameUIController foundController = FindObjectOfType<IngameUIController>();
                // We never set the instance so create a new SoundManager!
                if (foundController)
                {
                    // Use the one you just found.
                    _instance = foundController;
                }
                else
                {
                    // You don't have one????
                    Debug.LogError("You used the IngameUIController.instance without having a IngameUIController in this Scene!!!");
                    Debug.Break();
                }
            }

            return _instance;
        }
    }

    private GameController gameController;
    // Use this for initialization
    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();

        // Get the Cursors :)
        Texture2D[] cursors = Resources.LoadAll<Texture2D>("Cursor/");
        foreach (Texture2D currCursor in cursors)
        {
            switch (currCursor.name)
            {
                case "Dim_norm":
                    {
                        normalCursor = currCursor;
                    }
                    break;
                case "Dim_norm_Attack":
                    {
                        attackCursor = currCursor;
                    }
                    break;
                case "Dim_norm_Move":
                    {
                        moveCursor = currCursor;
                    }
                    break;
                case "Dim_norm_Patrol":
                    {
                        patrolCursor = currCursor;
                    }
                    break;
                default:
                    {
                        Debug.LogError("unknown Cursor found in the 'Ressource/Cursor/' Folder!");
                    }
                    break;
            }
        }
        if (!normalCursor || !attackCursor || !moveCursor || !patrolCursor)
        {
            Debug.LogError("One of the Cursors was not found");
        }

        // Now get the Toggles we need :)
        GameObject unitControlPanel = GameObject.Find("UnitControlPanel");
        Toggle[] toggles = unitControlPanel.GetComponentsInChildren<Toggle>();
        foreach (Toggle currToggle in toggles)
        {
            switch (currToggle.name)
            {
                case "Attack":
                    {
                        attackToggle = currToggle;
                    }
                    break;
                case "Patrol":
                    {
                        patrolToggle = currToggle;
                    }
                    break;
                case "Hold":
                    {
                        holdToggle = currToggle;
                    }
                    break;
                case "Move":
                    {
                        moveToggle = currToggle;
                    }
                    break;
            }
        }
        if (!holdToggle || !attackToggle || !moveToggle || !patrolToggle)
        {
            Debug.LogError("One of the Toggles was not found in the UnitControlPanel GO!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Change the UI Accordingly
        // Change the Toggles!
        if (gameController.cState == GameController.ControlState.AttackMove)
        {
            attackToggle.isOn = true;
            patrolToggle.isOn = false;
            moveToggle.isOn = false;
            Cursor.SetCursor(attackCursor, new Vector2(16, 16), CursorMode.Auto);
        }
        else if (gameController.cState == GameController.ControlState.Patrol)
        {
            attackToggle.isOn = false;
            patrolToggle.isOn = true;
            moveToggle.isOn = false;
            Cursor.SetCursor(patrolCursor, new Vector2(16, 16), CursorMode.Auto);
        }
        else if (gameController.cState == GameController.ControlState.Move)
        {
            attackToggle.isOn = false;
            patrolToggle.isOn = false;
            moveToggle.isOn = true;
            Cursor.SetCursor(moveCursor, new Vector2(16, 16), CursorMode.Auto);
        }
        else if (gameController.cState == GameController.ControlState.Normal)
        {
            attackToggle.isOn = false;
            patrolToggle.isOn = false;
            moveToggle.isOn = false;
            Cursor.SetCursor(normalCursor, new Vector2(16, 16), CursorMode.Auto);
        }
    }
}
