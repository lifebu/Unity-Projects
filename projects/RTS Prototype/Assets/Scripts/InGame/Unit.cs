using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Unit : MonoBehaviour
{
    /*
    TODO:
    - A Unit can get "commands" by the following sources:
        - A Player (either Human or AI)
        - It's own AI
    - The Unit should be made so that it works for AI and Human (not that important right now).
    should be easy if i thing of a unit as a "command" interface
    and the player class has the input in it (later) (right now that "interface" would be the GameController)
    - Commands can have delays?
    */

    public enum UnitState
    {
        Idle,
        PassiveMoving,
        Attacking,
        AggressiveMoving,
        Patrol,
        HoldPosition
    }

    // TODO: How to serialize the current state????
    public FiniteStateMachine<UnitState> fsm = new FiniteStateMachine<UnitState>();

    public enum AOEType
    {
        // No AOE
        None,
        // Cone emerging from the position of the Unit
        ConeInfront,
        // Circle around the Unit
        CircleAround,
        // Circle in a distant Area (bomb)
        CircleArea,
        // A straight Line
        StraightLine
    }

    public enum AttackTime
    {
        Direct,
        Channel,
        DamageOverTime
    }

    // Logic
    [ReadOnly]
    private bool isSelected;
    // Later this will be another class
    [Tooltip("0 = Player, 1 = AI")]
    public int player;
    public GameObject unitUIPrefab;
    private UnitUI unitUI;
    public bool drawDebugInfo;

    public Platoon platoon{ get; set; }

    // Audio
    // TODO: change the unitSelectClips so that it uses the resource-Folder not bound in editor!
    public AudioClip[] unitSelectClips;

    // Specific Commanddata
    private Vector3 moveTarget;
    [ReadOnly]
    private Unit attackTarget;
    private Vector3 patrolPoint1;
    private Vector3 patrolPoint2;
    private int currPatrolPoint;


    // Public Specification Data:
    public Color unSelectedColor;
    public Color selectedColor;
    public float moveSpeed;
    public float maxHealth;
    [ReadOnly]
    public float currHealth;
    [Tooltip("in Unity Units")]
    public float viewRange;
    public float autoReactRange;
    [Tooltip("Flat Armor for now")]
    public float armor;
    [Tooltip("Attacks per Second")]
    public float attackspeed;
    private float currAttackCooldown;
    public float attackDamage;
    [Tooltip("in Unity Units around the Unit (0 = Melee)")]
    public float attackRange;
    public AOEType aoeType;
    public AttackTime attackTime;
    public bool homing;
    public int amountOfAttackInstances;
    [Tooltip("not in Percent, 1 == 100%!")]
    public float hitChance;
    public bool isBalistic;



    // Use this for initialization
    void Start()
    {
        if(unitSelectClips.GetLength(0) == 0)
        {
            Debug.LogError("This Units does not have any UnitSounds associated with it: " + gameObject.ToString());
            Debug.Break();
        }
        if(unitUIPrefab == null)
        {
            Debug.LogError("This Units does not have any UnitUI Prefab associated with it: " + gameObject.ToString());
            Debug.Break();
        }

        // Create a new UnitUI
        unitUI = Instantiate(unitUIPrefab).GetComponent<UnitUI>();
        unitUI.transform.SetParent(GameObject.Find("UnitUI").transform);
        unitUI.changeVisibility(false);

        // The Start Settings for a Unit
        GetComponent<Renderer>().material.color = unSelectedColor;
        currHealth = maxHealth;
        currAttackCooldown = 0;


        fsm.Initialize(UnitState.Idle, AnyTransition, AnyUpdate);

        fsm.AddTransition(UnitState.Idle, UnitState.AggressiveMoving, null);
        fsm.AddTransition(UnitState.Idle, UnitState.Attacking, null);
        fsm.AddTransition(UnitState.Idle, UnitState.HoldPosition, null);
        fsm.AddTransition(UnitState.Idle, UnitState.PassiveMoving, null);
        fsm.AddTransition(UnitState.Idle, UnitState.Patrol, null);
        fsm.AddTransition(UnitState.Idle, UnitState.Idle, null);

        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.Idle, null);
        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.Attacking, null);
        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.HoldPosition, null);
        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.PassiveMoving, null);
        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.Patrol, null);
        fsm.AddTransition(UnitState.AggressiveMoving, UnitState.AggressiveMoving, null);

        fsm.AddTransition(UnitState.Attacking, UnitState.Idle, null);
        fsm.AddTransition(UnitState.Attacking, UnitState.HoldPosition, null);
        fsm.AddTransition(UnitState.Attacking, UnitState.AggressiveMoving, null);
        fsm.AddTransition(UnitState.Attacking, UnitState.PassiveMoving, null);
        fsm.AddTransition(UnitState.Attacking, UnitState.Patrol, null);
        fsm.AddTransition(UnitState.Attacking, UnitState.Attacking, null);

        fsm.AddTransition(UnitState.HoldPosition, UnitState.Idle, null);
        fsm.AddTransition(UnitState.HoldPosition, UnitState.Attacking, null);
        fsm.AddTransition(UnitState.HoldPosition, UnitState.AggressiveMoving, null);
        fsm.AddTransition(UnitState.HoldPosition, UnitState.PassiveMoving, null);
        fsm.AddTransition(UnitState.HoldPosition, UnitState.Patrol, null);
        fsm.AddTransition(UnitState.HoldPosition, UnitState.HoldPosition, null);

        fsm.AddTransition(UnitState.PassiveMoving, UnitState.Idle, null);
        fsm.AddTransition(UnitState.PassiveMoving, UnitState.Attacking, null);
        fsm.AddTransition(UnitState.PassiveMoving, UnitState.AggressiveMoving, null);
        fsm.AddTransition(UnitState.PassiveMoving, UnitState.HoldPosition, null);
        fsm.AddTransition(UnitState.PassiveMoving, UnitState.Patrol, null);
        fsm.AddTransition(UnitState.PassiveMoving, UnitState.PassiveMoving, null);

        fsm.AddTransition(UnitState.Patrol, UnitState.Idle, null);
        fsm.AddTransition(UnitState.Patrol, UnitState.Attacking, null);
        fsm.AddTransition(UnitState.Patrol, UnitState.AggressiveMoving, null);
        fsm.AddTransition(UnitState.Patrol, UnitState.HoldPosition, null);
        fsm.AddTransition(UnitState.Patrol, UnitState.PassiveMoving, null);
        fsm.AddTransition(UnitState.Patrol, UnitState.Patrol, null);

        fsm.AddAutoTransition(UnitState.AggressiveMoving, UpdateAggresiveMoving);
        fsm.AddAutoTransition(UnitState.Attacking, UpdateAttacking);
        fsm.AddAutoTransition(UnitState.HoldPosition, UpdateHoldingPosition);
        fsm.AddAutoTransition(UnitState.Idle, UpdateIdle);
        fsm.AddAutoTransition(UnitState.PassiveMoving, UpdatePassiveMoving);
        fsm.AddAutoTransition(UnitState.Patrol, UpdatePatrol);
    }

    void Update()
    {
        fsm.Update();
    }

    /* TODO: It would be kinda nice to have a centra UI Update function in the Unit class
     * But the Thing is that some uiUpdates happen every frame! (or do they have to???)
     * */

    // FSM Callbacks
    private void AnyTransition()
    {
        if(fsm.currState == UnitState.Patrol || fsm.currState == UnitState.HoldPosition)
        {
            unitUI.setUnitStateText(fsm.currState.ToString());
        }
        else
        {
            unitUI.setUnitStateText("");
        } 
    }
    private void AnyUpdate()
    {
        // Hack for now when i open the Ingame-Menu :(
        if(Time.deltaTime != 0)
        {
            //TODO: You should only update the Healthbar if the unit position changed or
            //the Unithealth changed and it'S not full!
            // TODO: CHANGE THAT LATER NOT QUERRY currSTATE!!!!

            // selected or health is not full!
            if(isSelected || (currHealth / maxHealth != 1 || fsm.currState == UnitState.HoldPosition || fsm.currState == UnitState.Patrol))
            {
                // Update Health Bar of the Unit!
                if(!unitUI.isVisible())
                {
                    unitUI.changeVisibility(true);
                }
                unitUI.updatePosition(transform.position);
                unitUI.updateLifebar(currHealth, maxHealth);


            }
            else
            {
                unitUI.changeVisibility(false);
            }
        }

    }
    private void UpdateAggresiveMoving()
    {
        Unit foundUnit = GetEnemyInAutoReactRange();
        if(foundUnit != null) CmdAttack(foundUnit);

        if(!Move(moveTarget))
        {
            fsm.Advance(UnitState.Idle);
        }
    }
    private void UpdateAttacking()
    {
        if(attackTarget == null || !IsOtherUnitInViewRange(attackTarget, viewRange))
        {            
            fsm.Advance(fsm.prevState);
            attackTarget = null;
            return;
        }

        if(!IsOtherUnitInAttackRange(attackTarget))
        {
            Move(attackTarget.transform.position);
        }
        else
        {
            AttackTarget(attackTarget);
        }
    }
    private void UpdateHoldingPosition()
    {
        Unit foundUnit = GetEnemyInAttackRange();
        if(foundUnit != null) AttackTarget(foundUnit);
    }
    private void UpdateIdle()
    {
        Unit foundUnit = GetEnemyInAutoReactRange();
        if(foundUnit != null) CmdAttack(foundUnit);
    }
    private void UpdatePassiveMoving()
    {
        if(!Move(moveTarget)) fsm.Advance(UnitState.Idle);
    }
    private void UpdatePatrol()
    {
        // Draw Both Patrolpoints:
        DebugExtension.DebugPoint(patrolPoint1, Color.blue, 0.5f, 0, true);
        DebugExtension.DebugPoint(patrolPoint2, Color.blue, 0.5f, 0, true);

        Unit foundUnit = GetEnemyInAutoReactRange();
        if(foundUnit != null) CmdAttack(foundUnit);

        /* TODO: The current implementation of two PatrolPoints is weird, need a list of patrol-points!
         * Maybe a Circular Buffer of PatrolPoints? Need to be able to create multiple patrol-points!
         * */
        if(currPatrolPoint == 0)
        {
            // We walk to the first Patrolpoint
            if(!Move(patrolPoint1))
            {
                // So because we just reached the patrolpoint change to the other one.
                currPatrolPoint = 1;
            }
        }
        else
        if(currPatrolPoint == 1)
        {
            if(!Move(patrolPoint2))
            {
                // So because we just reached the patrolpoint change to the other one.
                currPatrolPoint = 0;
            }
        }
    }


    public void OnDrawGizmos()
    {
        if(drawDebugInfo)
        {
            // Show the ViewRange of the Unit:
            DebugExtension.DrawCircle(transform.position, Color.black, viewRange);

            // Show the AutoRectRange of the Unit
            DebugExtension.DrawCircle(transform.position, Color.magenta, autoReactRange);

            // This is the attack Range of the Unit
            DebugExtension.DrawCircle(transform.position, Color.red, attackRange + (GetComponent<Collider>().bounds.size / 2.0f).z);
        }
    }
      
    // External Unit Commands
    public void Select()
    {
        // FIXME: DELETE ME
        /*
        if(platoon != null) platoon.messageQueue.AddMessage(
                new Message<Platoon.MessageTypes>(gameObject, Platoon.MessageTypes.UnitSelect, 0));
                */

        isSelected = true;
        GetComponent<Renderer>().material.color = selectedColor;
        SoundManager.instance.Play(unitSelectClips[Random.Range(0, unitSelectClips.GetLength(0))], SoundManager.SoundType.SFX, false);
        unitUI.changeVisibility(true);
        unitUI.updatePosition(transform.position);
        // TODO: CHANGE THIS, don't querry fsm.CurrState!
        if(fsm.currState == UnitState.HoldPosition)
        {
            IngameUIController.instance.holdToggle.isOn = true;
        }
    }
    public void Deselect()
    {
        isSelected = false;
        GetComponent<Renderer>().material.color = unSelectedColor;
        unitUI.changeVisibility(false);
        IngameUIController.instance.holdToggle.isOn = false;
    }
    public void CmdPassiveMove(Vector3 position)
    {
        if(isSelected)
        {
            IngameUIController.instance.holdToggle.isOn = false;
            moveTarget = position;
            fsm.Advance(UnitState.PassiveMoving);
        }
        else
        {
            Debug.LogError("You tried to Move an Unit without selecting it first!");
            Debug.Break();
        }
    }
    public void CmdAttack(Unit target)
    {
        if((target.transform.position - transform.position).magnitude <= viewRange)
        {
            IngameUIController.instance.holdToggle.isOn = false;
            attackTarget = target;
            fsm.Advance(UnitState.Attacking);
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
        IngameUIController.instance.holdToggle.isOn = false;
        fsm.Advance(UnitState.Idle);
    }
    public void CmdHoldPosition()
    {
        IngameUIController.instance.holdToggle.isOn = true;
        fsm.Advance(UnitState.HoldPosition);
    }
    public void CmdAggressiveMove(Vector3 position)
    {
        if(isSelected)
        {
            IngameUIController.instance.holdToggle.isOn = false;
            moveTarget = position;
            fsm.Advance(UnitState.AggressiveMoving);

        }
        else
        {
            Debug.LogError("You tried to Move an Unit without selecting it first!");
            Debug.Break();
        }
    }
    public void CmdPatrol(Vector3 p1, Vector3 p2)
    {
        IngameUIController.instance.holdToggle.isOn = false;
        currPatrolPoint = 0;
        patrolPoint1 = p1;
        patrolPoint2 = p2;
        fsm.Advance(UnitState.Patrol);
    }

    // Helper functions
    /// false == move has finished
    private bool Move(Vector3 target)
    {
        // First you need to face the correct way to the enemy Unit.
        transform.LookAt(target);

        Vector3 delta = target - transform.position;
        if(delta.magnitude > (delta.normalized * Time.deltaTime * moveSpeed).magnitude)
        {
            transform.position += delta.normalized * Time.deltaTime * moveSpeed;
            return true;
        }
        else
        {
            // Reached Target so stop move the rest and stop Moving.
            transform.position += delta.normalized * Time.deltaTime * moveSpeed;
            return false;
        }
    }

    public void GetDamage(float damage, GameObject source)
    {
        Unit aggressor = source.GetComponent<Unit>();
        // TODO: Change this, don't querry fsm.currState!!
        if(aggressor != null && aggressor.player != player && fsm.currState != UnitState.Attacking)
        {
            // An Enemy attacked you so attack back, if you aren't already in battle
            CmdAttack(aggressor);
        }

        currHealth -= damage;
        if(currHealth <= 0)
        {
            // Change this maybe? So that I pool stuff instead of deleting unitUIs?
            //unitUI.changeVisibility(false);
            if(platoon != null) platoon.messageQueue.AddMessage(
                new Message<Platoon.MessageTypes>(gameObject, Platoon.MessageTypes.UnitDead, 0));
            Destroy(unitUI.gameObject);
            Destroy(gameObject);
        }
     
    }
    // TODO: This will create different "ranges" depending on how the Unit is rotated
    private bool IsOtherUnitInAttackRange(Unit other)
    {
        Vector3 size = GetComponent<Collider>().bounds.size / 2.0f;
        float actualRange = size.z + attackRange;
        if(IsOtherUnitInViewRange(other, actualRange))
        {
            return true;
        }
        return false;
    }
    // TODO: This will create different "ranges" depending on how the Unit is rotated
    private bool IsOtherUnitInViewRange(Unit other, float specificRange)
    {
        Vector3 unitToTargetVec = other.transform.position - transform.position;
        if(unitToTargetVec.magnitude <= specificRange)
        {
            return true;
        }
        return false;
    }

    private void AttackTarget(Unit target)
    {
        // First you need to face the correct way to the enemy Unit.
        transform.LookAt(target.transform.position);

        currAttackCooldown -= Time.deltaTime;
        if(currAttackCooldown <= 0)
        {
            // you can do an attack!
            target.GetDamage(attackDamage, gameObject);
            currAttackCooldown = attackspeed;
        }
    }

    // TODO: These Physics Querries should be made way more efficient later!
    private Unit GetEnemyInAutoReactRange()
    {
        RaycastHit[] foundUnits = Physics.SphereCastAll(transform.position, autoReactRange, Vector3.up, autoReactRange, LayerMask.GetMask("Unit"));
        foreach(var hit in foundUnits)
        {
            Unit currFoundUnit = hit.transform.GetComponent<Unit>();
            if(currFoundUnit.player != player)
            {
                return currFoundUnit;
            }
        }
        return null;
    }

    private Unit GetEnemyInAttackRange()
    {
        RaycastHit[] foundUnits = Physics.SphereCastAll(transform.position, viewRange, Vector3.up, viewRange, LayerMask.GetMask("Unit"));
        foreach(var hit in foundUnits)
        {
            Unit currFoundUnit = hit.transform.GetComponent<Unit>();
            if(currFoundUnit.player != player && IsOtherUnitInAttackRange(currFoundUnit))
            {
                return currFoundUnit;
            }
        }
        return null;
    }

}