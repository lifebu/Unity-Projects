using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platoon : MonoBehaviour
{
    // Definitions
    public enum MessageTypes
    {
        UnitSelect,
        UnitDead
    }
    public enum PlatoonState
    {
        Idle,
        PassiveMoving,
        Attacking,
        AggressiveMoving,
        Patrol,
        HoldPosition
    }

    // FIXME: Platoon atm is hardcoded for the 5 Unit formation!

    // Data:
    private FiniteStateMachine<PlatoonState> fsm;
    List<Unit> assignedUnits;
    [ReadOnly]
    private bool isSelected;
    // Specific Commanddata
    private Vector3 moveTarget;
    [ReadOnly]
    private Unit attackTarget;
    private Vector3 patrolPoint1;
    private Vector3 patrolPoint2;
    private int currPatrolPoint;

    // Formation Data
    public float formationXOffset;
    public float formationYOffset;

    public MessageQueue<MessageTypes> messageQueue { get; private set; }
    // TODO: Later in Player Class
    public int player;


    public Platoon()
    {
        fsm = new FiniteStateMachine<PlatoonState>();
        messageQueue = new MessageQueue<MessageTypes>();
        assignedUnits = new List<Unit>();

        messageQueue.AddCallback(MessageTypes.UnitSelect, OnUnitAtkMessage);
        messageQueue.AddCallback(MessageTypes.UnitDead, OnUnitDeadMessage);

        fsm.Initialize(PlatoonState.Idle, null, null);

        fsm.AddTransition(PlatoonState.Idle, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);
        fsm.AddTransition(PlatoonState.Idle, PlatoonState.Attacking, null);
        fsm.AddTransition(PlatoonState.Idle, PlatoonState.HoldPosition, null);
        fsm.AddTransition(PlatoonState.Idle, PlatoonState.PassiveMoving, TransitionToPassiveMoving);
        fsm.AddTransition(PlatoonState.Idle, PlatoonState.Patrol, null);
        fsm.AddTransition(PlatoonState.Idle, PlatoonState.Idle, null);

        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.Idle, null);
        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.Attacking, null);
        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.HoldPosition, null);
        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.PassiveMoving, TransitionToPassiveMoving);
        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.Patrol, null);
        fsm.AddTransition(PlatoonState.AggressiveMoving, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);

        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.Idle, null);
        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.HoldPosition, null);
        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);
        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.PassiveMoving, TransitionToPassiveMoving);
        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.Patrol, null);
        fsm.AddTransition(PlatoonState.Attacking, PlatoonState.Attacking, null);

        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.Idle, null);
        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.Attacking, null);
        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);
        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.PassiveMoving, TransitionToPassiveMoving);
        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.Patrol, null);
        fsm.AddTransition(PlatoonState.HoldPosition, PlatoonState.HoldPosition, null);

        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.Idle, null);
        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.Attacking, null);
        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);
        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.HoldPosition, null);
        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.Patrol, null);
        fsm.AddTransition(PlatoonState.PassiveMoving, PlatoonState.PassiveMoving, TransitionToPassiveMoving);

        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.Idle, null);
        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.Attacking, null);
        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.AggressiveMoving, TransitionToAggresiveMoving);
        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.HoldPosition, null);
        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.PassiveMoving, TransitionToPassiveMoving);
        fsm.AddTransition(PlatoonState.Patrol, PlatoonState.Patrol, null);

        fsm.AddAutoTransition(PlatoonState.AggressiveMoving, UpdateAggresiveMoving);
        fsm.AddAutoTransition(PlatoonState.Attacking, UpdateAttacking);
        fsm.AddAutoTransition(PlatoonState.HoldPosition, UpdateHoldingPosition);
        fsm.AddAutoTransition(PlatoonState.Idle, UpdateIdle);
        fsm.AddAutoTransition(PlatoonState.PassiveMoving, UpdatePassiveMoving);
        fsm.AddAutoTransition(PlatoonState.Patrol, UpdatePatrol);
    }

    void Update()
    {
        messageQueue.DispatchMessages();
        fsm.Update();
    }

    // FSM Callbacks:
    private void TransitionToPassiveMoving()
    {
        moveTwoLineFormation(false);
    }
    private void TransitionToAggresiveMoving()
    {
        moveTwoLineFormation(true);
    }
    private void UpdateAggresiveMoving()
    {
        int amountOfFinishedOnes = 0;

        foreach(var unit in assignedUnits)
        {
            if(unit.fsm.currState == Unit.UnitState.Idle) amountOfFinishedOnes++;
        }

        if(amountOfFinishedOnes == assignedUnits.Count)
        {
            fsm.Advance(PlatoonState.Idle);
        }
    }
    private void UpdateAttacking()
    {
        
    }
    private void UpdateHoldingPosition()
    {
        
    }
    private void UpdateIdle()
    {
        
    }
    private void UpdatePassiveMoving()
    {
        int amountOfFinishedOnes = 0;

        foreach(var unit in assignedUnits)
        {
            if(unit.fsm.currState == Unit.UnitState.Idle) amountOfFinishedOnes++;
        }

        if(amountOfFinishedOnes == assignedUnits.Count)
        {
            fsm.Advance(PlatoonState.Idle);
        }
    }
    private void UpdatePatrol()
    {
        
    }

    // Interface Functions:
    public void AddUnitToPlatoon(Unit unit)
    {
        if(assignedUnits.Contains(unit))
        {
            Debug.LogError("Platoon.AddUnitToPlatoon(): tried to add a Unit to the platoon which is already in this platoon"); 
            return;
        }
        assignedUnits.Add(unit);
        unit.platoon = this;
    }
    public int GetAmountOfUnits()
    {
        return assignedUnits.Count;
    }

    // Commands:
    public void Select()
    {
        foreach(var unit in assignedUnits)
        {
            unit.Select();
        }
        isSelected = true;
    }
    public void Deselect()
    {
        foreach(var unit in assignedUnits)
        {
            unit.Deselect();
        }
        isSelected = false;
    }
    public void CmdPassiveMove(Vector3 position)
    {  
        if(isSelected)
        {
            moveTarget = position;
            fsm.Advance(PlatoonState.PassiveMoving);
        }
        else
        {
            Debug.LogError("You tried to Move an Platoon without selecting it first!");
            Debug.Break();
        }
    }
    public void CmdAttack(Unit target)
    {
        //FIXME: Need to calculate if the target is in viewRange for the platoon, but i need more suffisticated systems for that later.
        bool isInRange = true;
        if(isInRange)
        {
            IngameUIController.instance.holdToggle.isOn = false;
            attackTarget = target;
            fsm.Advance(PlatoonState.Attacking);
        }
        else
        {
            // Yeah that Error is buggy atm as the Distance between the units is surely larger
            // 
            Debug.Log("Hey you tried to attack a Unit that is out of the Unit's Vision Range");
        }
    }
    public void CmdStop()
    {
        fsm.Advance(PlatoonState.Idle);
    }
    public void CmdHoldPosition()
    {
        fsm.Advance(PlatoonState.HoldPosition);
    }
    public void CmdAggressiveMove(Vector3 position)
    {
        if(isSelected)
        {
            moveTarget = position;
            fsm.Advance(PlatoonState.AggressiveMoving);
        }
        else
        {
            Debug.LogError("You tried to Move an Platoon without selecting it first!");
            Debug.Break();
        }
    }
    public void CmdPatrol(Vector3 p1, Vector3 p2)
    {
        currPatrolPoint = 0;
        patrolPoint1 = p1;
        patrolPoint2 = p2;
        fsm.Advance(PlatoonState.Patrol);
    }

    // Helper functions
    private Vector3 getPlatoonMedianPos()
    {
        Vector3 median = Vector3.zero;
        foreach(var unit in assignedUnits)
        {
            median += unit.transform.position;
        }
        median = median / assignedUnits.Count;
        return median;
    }
    public void moveTwoLineFormation(bool aggresive)
    {
        Vector3 posDiff = moveTarget - getPlatoonMedianPos();
        float angle = Vector3.Angle(posDiff, Vector3.forward);
        if(posDiff.x < 0) angle = 360.0f - angle;
        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        Debug.Log(angle);

        int height = 2;
        int width = (assignedUnits.Count / height);

        for(int yCountOffset = -(height / 2); yCountOffset <= ((height / 2) - 1); yCountOffset++)
        {
            for(int xCountOffset = -(width / 2); xCountOffset <= (width / 2); xCountOffset++)
            {
                // TODO: Sometimes these Angles are weird, need to look into that!
                Vector3 unitOffset = new Vector3(xCountOffset * formationXOffset, 0.0f, -yCountOffset * formationYOffset);

                unitOffset =  m.MultiplyPoint3x4(unitOffset);
                Vector3 unitTarget = moveTarget + unitOffset;

                int xIndexOffSet = xCountOffset + (width / 2);
                int yIndexOffset = yCountOffset + 1;
                if(xIndexOffSet + yIndexOffset * width < assignedUnits.Count)
                {
                    if(aggresive)
                    {
                        assignedUnits[xIndexOffSet + yIndexOffset * width].CmdAggressiveMove(unitTarget); 
                    }
                    else
                    {
                        assignedUnits[xIndexOffSet + yIndexOffset * width].CmdPassiveMove(unitTarget); 
                    }

                }

            }
        }
    }

    // MessageQueue Callbacks:
    private void OnUnitAtkMessage(GameObject callee)
    {
        Debug.Log("Callback worked!, Callee: " + callee);
    }
    private void OnUnitDeadMessage(GameObject callee)
    {
        assignedUnits.Remove(callee.GetComponent<Unit>());
        if(assignedUnits.Count == 0)
        {
            Deselect();
        }
        Debug.Log("A Unit Died!");
    }
}
