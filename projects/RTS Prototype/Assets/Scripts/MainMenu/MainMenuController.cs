using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.ComponentModel;

public class MainMenuController : MonoBehaviour
{
    // The State of the Mainmenu!
    [System.Serializable]
    public enum MainMenuState
    {
        Main,
        Tutorial,
        Singleplayer,
        Multiplayer,
        Options,
        Profiles
    }

    private FiniteStateMachine<MainMenuState> fsm = new FiniteStateMachine<MainMenuState>();

    [Tooltip("0 = Main, 1 = Tutorial, 2 = Singleplayer, 3 = Multiplayer, 4 = Options, 5 = Profiles")]
    public GameObject[] Canvases = new GameObject[6];


    void Start ()
    {
        fsm.Initialize(MainMenuState.Main, AnyTransition, null);
        fsm.AddTransition(MainMenuState.Main, MainMenuState.Tutorial, null);
        fsm.AddTransition(MainMenuState.Main, MainMenuState.Singleplayer, null);
        fsm.AddTransition(MainMenuState.Main, MainMenuState.Multiplayer, null);
        fsm.AddTransition(MainMenuState.Main, MainMenuState.Options, null);
        fsm.AddTransition(MainMenuState.Main, MainMenuState.Profiles, null);
        fsm.AddTransition(MainMenuState.Tutorial, MainMenuState.Main, null);
        fsm.AddTransition(MainMenuState.Singleplayer, MainMenuState.Main, null);
        fsm.AddTransition(MainMenuState.Multiplayer, MainMenuState.Main, null);
        fsm.AddTransition(MainMenuState.Options, MainMenuState.Main, null);
        fsm.AddTransition(MainMenuState.Profiles, MainMenuState.Main, null);

        SoundManager.instance.Play("Menu", SoundManager.SoundType.Music, true);
	}

    // FSM Callbacks:
    public void AnyTransition()
    {
        foreach (GameObject currCanves in Canvases)
        {
            if (currCanves.name == fsm.currState.ToString())
            {
                // This is the current State so set the canvas to turn it on!
                currCanves.SetActive(true);
            }
            else
            {
                // This is one of the anvas not belonging to the active state, so turn it off!
                currCanves.SetActive(false);
            }
        }
    }

    public void ButtonClicked(string buttonName)
    {
        switch (buttonName)
        {
            case "Tutorial":
                {
                    fsm.Advance(MainMenuState.Tutorial);
                }
                break;
            case "Singleplayer":
                {
                    fsm.Advance(MainMenuState.Singleplayer);   
                }
                break;
            case "Multiplayer":
                {
                    fsm.Advance(MainMenuState.Multiplayer); 
                }
                break;
            case "Options":
                {
                    fsm.Advance(MainMenuState.Options);
                }
                break;
            case "Profile":
                {
                    fsm.Advance(MainMenuState.Profiles);
                }
                break;
            case "Quit":
                {
                    Application.Quit();
                }
                break;
            case "Outerworld":
                {
                    Debug.Log("Outerworld Tutorial Clicked!");
                }
                break;
            case "Anthill":
                {
                    Debug.Log("Anthill Tutorial Clicked!");
                }
                break;
            case "Back":
                {
                    fsm.Advance(MainMenuState.Main);
                }
                break;
            case "NewCampaign":
                {
                    SceneManager.LoadScene("MainGame");
                }
                break;
            case "ResumeCampaign":
                {
                    Debug.Log("Resume Campaign Clicked!");
                }
                break;
            case "SelectMission":
                {
                    Debug.Log("Select Mission Clicked!");
                }
                break;
            case "Skirmish":
                {
                    Debug.Log("Skirmish Clicked!");
                }
                break;
            case "Load":
                {
                    Debug.Log("Load Clicked!");
                }
                break;
            case "Local":
                {
                    Debug.Log("Local Multiplayer Clicked!");
                }
                break;
            case "Internet":
                {
                    Debug.Log("Online Multiplayer Clicked!");
                }
                break;
            default:
                {
                    Debug.LogError("Unknown buttonName: " +  buttonName);
                }
                break;
        }

        SoundManager.instance.Play("Click", SoundManager.SoundType.SFX, false);
    }
}
