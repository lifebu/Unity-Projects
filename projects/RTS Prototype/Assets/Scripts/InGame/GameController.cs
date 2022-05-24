using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private List<Unit> currSelectedUnits;
    private bool lmbWasHeld;
    private float mouseButtonDownTime;
    private Vector2 MouseDragStartPos;
    public float mouseHoldThresholdInSeconds;
    public GameObject HUD;
    public GameObject SelectiobarPrefab;
    private GameObject SelectionbarInstance;
    public GameObject InGameMenu;

    private Vector3 firstPatrolPos;
    private Vector3 secondPatrolPos;

    // TODO: Change the GameController to use the new FSM System!

    // TODO: I would need an Extension to Input, to querry if the mouse was held or just clicked!

    public enum ControlState
    {
        Normal,
        AttackMove,
        Move,
        Patrol
    }

    public enum GameState
    {
        InGame,
        InMenu
    }

    [ReadOnly]
    public GameState state = GameState.InGame;
    [ReadOnly]
    public ControlState cState = ControlState.Normal;

    private List<Platoon> platoons;
    private Platoon currSelectedPlatoon;
    public Unit[] PlatoonUnits;
    public bool usePlatoonControlls;


    void Start()
    {
        currSelectedUnits = new List<Unit>();
        SoundManager.instance.Play("Empire of the Ants - It's a Big World", SoundManager.SoundType.Music, true);
        SoundManager.instance.Play("Sunny2", SoundManager.SoundType.Background, true);

        HUD.SetActive(true);
        InGameMenu.SetActive(false);

        Debug.developerConsoleVisible = true;
        firstPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        secondPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);


        platoons = new List<Platoon>();
        Platoon testPlatoon = gameObject.AddComponent<Platoon>();
        testPlatoon.player = 0;
        testPlatoon.formationXOffset = 1.4f;
        testPlatoon.formationYOffset = 1.4f;
        // TODO: Later have functions to add a Unit to a platoon
        foreach(Unit unit in PlatoonUnits)
        {
            if(unit.player == testPlatoon.player)
            {
                testPlatoon.AddUnitToPlatoon(unit);
            }
        }

        platoons.Add(testPlatoon);
    }

    void Update()
    {

        ConfineCursor();

        /*
        TODO: Input: I only care about if the lmb was clicked or was held!
        change the way Input works to allow for checks of clicking and holding.
        */

        switch(state)
        {
            case GameState.InGame:
                {
                    InGameLogic();
                }
                break;
            case GameState.InMenu:
                {
                    InMenuLogic();
                }
                break;
        }
    }
        
    // Menu Logic
    private void InMenuLogic()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1.0f;
            SoundManager.instance.StopAllSounds();
            SoundManager.instance.Play("Empire of the Ants - It's a Big World", SoundManager.SoundType.Music, true);
            SoundManager.instance.Play("Sunny2", SoundManager.SoundType.Background, true);
            state = GameState.InGame;
            InGameMenu.SetActive(false);
            HUD.SetActive(true);
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void BackToMenuButton()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    // Ingame Logic
    private void InGameLogic()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(cState == ControlState.Normal)
            {
                SoundManager.instance.StopAllSounds();
                SoundManager.instance.Play("LSDmenu", SoundManager.SoundType.Music, true);
                Time.timeScale = 0.0f;

                state = GameState.InMenu;
                InGameMenu.SetActive(true);
                HUD.SetActive(false);
            }
            else
            {
                cState = ControlState.Normal;
            }

        }

        // Mouse Click and Held Logic
        if(Input.GetMouseButtonDown(0))
        {
            // This Time is important to check if the mousebutton was held or just clicked
            mouseButtonDownTime = Time.time;
            lmbWasHeld = false;
            MouseDragStartPos = Input.mousePosition;
        }
        if(Input.GetMouseButton(0))
        {
            if(cState == ControlState.Normal)
            {
                if(Time.time > mouseHoldThresholdInSeconds + mouseButtonDownTime)
                {
                    lmbWasHeld = true;
                    // This is a Mouseheld situation
                    if(SelectionbarInstance == null)
                    {
                        SelectionbarInstance = Instantiate(SelectiobarPrefab);
                        SelectionbarInstance.transform.SetParent(HUD.transform, true);
                        SelectionbarInstance.transform.localScale = Vector3.one;
                        SelectionbarInstance.transform.localEulerAngles = Quaternion.identity.eulerAngles;
                    }

                    Vector2 currMousePosInCanvas = HUD.GetComponent<Canvas>().ScreenToCanvasPoint(Input.mousePosition);
                    Vector2 startMousePosInCanvas = HUD.GetComponent<Canvas>().ScreenToCanvasPoint(MouseDragStartPos);
                    ((RectTransform)SelectionbarInstance.transform).anchoredPosition3D =
                        Vector3.Min(currMousePosInCanvas, startMousePosInCanvas);
                    Vector3 positiveOne = currMousePosInCanvas - startMousePosInCanvas;
                    if(positiveOne.x < 0)
                    {
                        positiveOne.x = -positiveOne.x;
                    }
                    if(positiveOne.y < 0)
                    {
                        positiveOne.y = -positiveOne.y;
                    }
                    ((RectTransform)SelectionbarInstance.transform).sizeDelta = positiveOne;
                }
            }
        }

        // Unit Control
        if(Input.GetMouseButtonUp(0) && lmbWasHeld)
        {
            if(cState == ControlState.Normal)
            {
                if(usePlatoonControlls)
                {
                    PlatoonControllLmbHeld();
                }
                else
                {
                    UnitControllLmbHeld();
                }
                Destroy(SelectionbarInstance);
            }
        }
        if(Input.GetMouseButtonUp(0) && !lmbWasHeld)
        {
            if(usePlatoonControlls)
            {
                PlatoonControllLmbClicked();
            }
            else
            {
                UnitControllLmbClicked();
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllRmbClicked();
            }
            else
            {
                UnitControllRmbClicked();
            }
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllStopKey();
            }
            else
            {
                UnitControllStopKey();
            }
        }
        if(Input.GetKeyDown(KeyCode.H))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllHoldKey();
            }
            else
            {
                UnitControllHoldKey();
            }
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllAttackKey();
            }
            else
            {
                UnitControllAttackKey();
            }
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllPatrolKey();
            }
            else
            {
                UnitControllPatrolKey();
            }
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(usePlatoonControlls)
            {
                PlatoonControllMoveKey();
            }
            else
            {
                UnitControllMoveKey();
            }
        }

        // Debug:
        if(!firstPatrolPos.AllValuesAreThis(Mathf.Infinity))
        {
            DebugExtension.DebugPoint(firstPatrolPos, Color.blue, 0.5f, 0, true);
        }
    }
    // FIXME: Later these helper function will disappear once i fully transitioned to the platoon logic
    private void UnitControllLmbHeld()
    {
        // Here you can do something in the very frame the lmb is lifted
        // only when you held the button down for long enough
        Vector3 center = new Vector3();
        HUD.GetComponent<Canvas>().CastRayFromCanvas(
            ((RectTransform)SelectionbarInstance.transform).anchoredPosition
            + ((RectTransform)SelectionbarInstance.transform).sizeDelta / 2.0f,
            out center, 100.0f, LayerMask.GetMask("Ground"));

        Vector3 farCorner = new Vector3();
        HUD.GetComponent<Canvas>().CastRayFromCanvas(
            ((RectTransform)SelectionbarInstance.transform).anchoredPosition
            + ((RectTransform)SelectionbarInstance.transform).sizeDelta,
            out farCorner, 100.0f, LayerMask.GetMask("Ground"));

        Vector3 halfExtents = farCorner - center;
        // This is a small hack for now
        halfExtents.y = 0.5f;
        Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);

        RaycastHit[] hitUnits = Physics.BoxCastAll(center, halfExtents, direction, Quaternion.identity, 100.0f, LayerMask.GetMask("Unit"));

        bool oneWasOfPlayer0 = false;
        for(int hitIndex = 0; hitIndex < hitUnits.Length; ++hitIndex)
        {
            if(hitUnits[hitIndex].transform.GetComponent<Unit>().player == 0)
            {
                oneWasOfPlayer0 = true;
                break;
            }
        }

        if(oneWasOfPlayer0)
        {
            // Deselect any previously selected units
            for(int i = 0; i < currSelectedUnits.Count; ++i)
            {
                if(currSelectedUnits[i])
                {
                    currSelectedUnits[i].Deselect();

                }
                currSelectedUnits.RemoveAt(i);
            }

            // This is one of your units so you can select it!
            for(int hitIndex = 0; hitIndex < hitUnits.Length; ++hitIndex)
            {
                Unit hitUnitScript = hitUnits[hitIndex].transform.GetComponent<Unit>();
                if(hitUnitScript.player == 0)
                {
                    hitUnitScript.Select();
                    currSelectedUnits.Add(hitUnitScript);
                }

            }
        }


    }
    private void UnitControllLmbClicked()
    {
        //Debug.Log("LMB Click");
        // Cast a Ray!
        Ray CamRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(CamRay, out hit, 100.0f, LayerMask.GetMask("Unit")))
        {
            if(cState == ControlState.Normal)
            {
                Unit hitUnitScript = hit.transform.GetComponent<Unit>();
                if(hitUnitScript.player == 0)
                {
                    // Deselect any previously selected units
                    for(int i = 0; i < currSelectedUnits.Count; ++i)
                    {
                        if(currSelectedUnits[i])
                        {
                            currSelectedUnits[i].Deselect();

                        }
                        currSelectedUnits.RemoveAt(i);
                    }

                    // This is one of your units so you can select it!
                    hitUnitScript.Select();
                    currSelectedUnits.Add(hitUnitScript);
                }
                else if(hitUnitScript.player == 1)
                {
                    // The enemy! attack them!
                    for(int i = 0; i < currSelectedUnits.Count; ++i)
                    {
                        currSelectedUnits[i].CmdAttack(hitUnitScript);
                    }

                }

            }
            else if(cState == ControlState.AttackMove)
            {
                // It doesn't mater which unit you a-clicked on!
                // This always means that you will attack a unit
                // despite it might be yours!
                Unit hitUnitScript = hit.transform.GetComponent<Unit>();
                for(int i = 0; i < currSelectedUnits.Count; ++i)
                {
                    currSelectedUnits[i].CmdAttack(hitUnitScript);
                    cState = ControlState.Normal;
                }
            }


        }
        else if(Physics.Raycast(CamRay, out hit, 100.0f, LayerMask.GetMask("Ground")))
        {
            // TODO: Use a Unit Eventsystem to signal that a unit was destroyed!
            if(cState == ControlState.Patrol)
            {
                if(firstPatrolPos.AllValuesAreThis(Mathf.Infinity))
                {
                    // So we haven't set the first Positon yet so do that!
                    firstPatrolPos = hit.point;
                }
                else if(secondPatrolPos.AllValuesAreThis(Mathf.Infinity))
                {
                    secondPatrolPos = hit.point;
                    for(int i = 0; i < currSelectedUnits.Count; i++)
                    {
                        currSelectedUnits[i].CmdPatrol(firstPatrolPos, secondPatrolPos);
                    }

                    cState = ControlState.Normal;
                    firstPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                    secondPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                }
                else
                {
                    Debug.LogError("Somehow you was able to set all Patrolpositions without ussuing the Patrol Command, whaaat?");
                }
            }
            else
            {
                for(int i = 0; i < currSelectedUnits.Count; i++)
                {
                    if(currSelectedUnits[i])
                    {
                        if(cState == ControlState.Normal)
                        {
                            currSelectedUnits[i].CmdPassiveMove(hit.point);
                        }
                        else
                        if(cState == ControlState.Move)
                        {
                            currSelectedUnits[i].CmdPassiveMove(hit.point);
                            cState = ControlState.Normal;
                        }
                        else
                        if(cState == ControlState.AttackMove)
                        {
                            currSelectedUnits[i].CmdAggressiveMove(hit.point);
                            cState = ControlState.Normal;
                        }
                    }
                    else
                    {
                        currSelectedUnits.RemoveAt(i);
                    }

                }
            }

        }
    }
    private void UnitControllRmbClicked()
    {
        cState = ControlState.Normal;
        while(currSelectedUnits.Count > 0)
        {
            if(currSelectedUnits[0])
            {
                currSelectedUnits[0].Deselect();

            }
            currSelectedUnits.RemoveAt(0);
        }

    }
    private void UnitControllStopKey()
    {
        for(int i = 0; i < currSelectedUnits.Count; i++)
        {
            // Stop anything the Unit is doing right now.  
            currSelectedUnits[i].CmdStop();
        }
    }
    private void UnitControllHoldKey()
    {
        for(int i = 0; i < currSelectedUnits.Count; i++)
        {
            // Any Selected Unit will go into the "Hold Position" Command and will absolutly do nothing!
            currSelectedUnits[i].CmdHoldPosition();
        }
    }
    private void UnitControllAttackKey()
    {
        if(currSelectedUnits.Count > 0)
        {
            if(cState != ControlState.AttackMove)
            {
                cState = ControlState.AttackMove;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }
    private void UnitControllPatrolKey()
    {
        if(currSelectedUnits.Count > 0)
        {
            if(cState != ControlState.Patrol)
            {
                firstPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                secondPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

                cState = ControlState.Patrol;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }
    private void UnitControllMoveKey()
    {
        if(currSelectedUnits.Count > 0)
        {
            if(cState != ControlState.Move)
            {
                cState = ControlState.Move;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }

    private void PlatoonControllLmbHeld()
    {
        // Here you can do something in the very frame the lmb is lifted
        // only when you held the button down for long enough
        Vector3 center = new Vector3();
        HUD.GetComponent<Canvas>().CastRayFromCanvas(
            ((RectTransform)SelectionbarInstance.transform).anchoredPosition
            + ((RectTransform)SelectionbarInstance.transform).sizeDelta / 2.0f,
            out center, 100.0f, LayerMask.GetMask("Ground"));

        Vector3 farCorner = new Vector3();
        HUD.GetComponent<Canvas>().CastRayFromCanvas(
            ((RectTransform)SelectionbarInstance.transform).anchoredPosition
            + ((RectTransform)SelectionbarInstance.transform).sizeDelta,
            out farCorner, 100.0f, LayerMask.GetMask("Ground"));

        Vector3 halfExtents = farCorner - center;
        // This is a small hack for now
        halfExtents.y = 0.5f;
        Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);
        RaycastHit[] hitUnits = Physics.BoxCastAll(center, halfExtents, direction, Quaternion.identity, 100.0f, LayerMask.GetMask("Unit"));

        Unit firstPlayerUnit = null;
        for(int hitIndex = 0; hitIndex < hitUnits.Length; ++hitIndex)
        {
            if(hitUnits[hitIndex].transform.GetComponent<Unit>().player == 0)
            {
                firstPlayerUnit = hitUnits[hitIndex].transform.GetComponent<Unit>();
                break;
            }
        }

        if(firstPlayerUnit != null)
        {
            // Deselect any previously selected platoon
            if(currSelectedPlatoon != null)
            {
                currSelectedPlatoon.Deselect();
                currSelectedPlatoon = null;
            }

            // This is one of your units so you can select it!
            // This is one of your platoons so you can select it!
            firstPlayerUnit.platoon.Select();
            currSelectedPlatoon = firstPlayerUnit.platoon;
        }
    }
    private void PlatoonControllLmbClicked()
    {
        Ray CamRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(CamRay, out hit, 100.0f, LayerMask.GetMask("Unit")))
        {
            if(cState == ControlState.Normal)
            {
                Unit hitUnitScript = hit.transform.GetComponent<Unit>();
                if(hitUnitScript.player == 0)
                {
                    // Deselect any previously selected platoon
                    if(currSelectedPlatoon != null)
                    {
                        currSelectedPlatoon.Deselect();
                        currSelectedPlatoon = null;
                    }

                    // This is one of your platoons so you can select it!
                    hitUnitScript.platoon.Select();
                    currSelectedPlatoon = hitUnitScript.platoon;
                }
                else
                if(hitUnitScript.player == 1)
                {
                    // The enemy! attack them!
                    if(currSelectedPlatoon != null) currSelectedPlatoon.CmdAttack(hitUnitScript);
                }

            }
            else
            if(cState == ControlState.AttackMove)
            {
                
                // It doesn't mater which unit you a-clicked on!
                // This always means that you will attack a unit
                // despite it might be yours!
                /*
                Unit hitUnitScript = hit.transform.GetComponent<Unit>();
                for(int i = 0; i < currSelectedUnits.Count; ++i)
                {
                    currSelectedUnits[i].CmdAttack(hitUnitScript);
                    cState = ControlState.Normal;
                }
                */
            }
        }
        else if(Physics.Raycast(CamRay, out hit, 100.0f, LayerMask.GetMask("Ground")))
        {
            if(currSelectedPlatoon != null)
            {
                // TODO: Use a Unit Eventsystem to signal that a unit was destroyed!
                if(cState == ControlState.Patrol)
                {   
                    if(firstPatrolPos.AllValuesAreThis(Mathf.Infinity))
                    {
                        // So we haven't set the first Positon yet so do that!
                        firstPatrolPos = hit.point;
                    }
                    else if(secondPatrolPos.AllValuesAreThis(Mathf.Infinity))
                    {
                        secondPatrolPos = hit.point;
                        if(currSelectedPlatoon != null) currSelectedPlatoon.CmdPatrol(firstPatrolPos, secondPatrolPos);

                        cState = ControlState.Normal;
                        firstPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                        secondPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                    }
                    else
                    {
                        Debug.LogError("Somehow you was able to set all Patrolpositions without ussuing the Patrol Command, whaaat?");
                    }
                }
                else
                {
                    if(cState == ControlState.Normal)
                    {
                        currSelectedPlatoon.CmdPassiveMove(hit.point);
                    }
                    else if(cState == ControlState.Move)
                    {
                        currSelectedPlatoon.CmdPassiveMove(hit.point);
                        cState = ControlState.Normal;
                    }
                    else if(cState == ControlState.AttackMove)
                    {
                        currSelectedPlatoon.CmdAggressiveMove(hit.point);
                        cState = ControlState.Normal;
                    }
                }
            }
        }
    }
    private void PlatoonControllRmbClicked()
    {
        cState = ControlState.Normal;
        if(currSelectedPlatoon != null)
        {
            currSelectedPlatoon.Deselect();
            currSelectedPlatoon = null; 
        }
    }
    private void PlatoonControllStopKey()
    {
        if(currSelectedPlatoon != null) currSelectedPlatoon.CmdStop();
    }
    private void PlatoonControllHoldKey()
    {
        if(currSelectedPlatoon != null) currSelectedPlatoon.CmdHoldPosition();
     }
    private void PlatoonControllAttackKey()
    {
        if(currSelectedPlatoon != null)
        {
            if(cState != ControlState.AttackMove)
            {
                cState = ControlState.AttackMove;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }
    private void PlatoonControllPatrolKey()
    {
        if(currSelectedPlatoon != null)
        {
            if(cState != ControlState.Patrol)
            {
                firstPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                secondPatrolPos = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

                cState = ControlState.Patrol;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }
    private void PlatoonControllMoveKey()
    {
        if(currSelectedPlatoon != null)
        {
            if(cState != ControlState.Move)
            {
                cState = ControlState.Move;
            }
            else
            {
                cState = ControlState.Normal;
            }
        }
    }


    // Help Functions
    private void ConfineCursor()
    {
        if(Cursor.lockState != CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

    }

}
