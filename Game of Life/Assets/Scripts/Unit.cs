using StateMachine;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditorInternal.VersionControl.ListControl;

public class Unit : MonoBehaviour
{
    public StateMachine.State AliveState { get; private set; }
    public StateMachine.State DeadState { get; private set; }

    public Vector2Int gridPosition;

    private GameManager gameManager;
    private GridManager gridManager;

    private SpriteRenderer spriteRenderer;

    public StateMachine.StateMachine StateMachine { get; private set; }

    private void Start() {
        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject != null) {
            // Extract the script component from the found GameObject
            gameManager = gameManagerObject.GetComponent<GameManager>();
            gridManager = gameManagerObject.GetComponent<GridManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        AliveState = new StateMachine.State("alive");
        DeadState = new StateMachine.State("dead");

        AliveState.AddAction(new ColorAction(spriteRenderer, new Color(0.9f, 0.85f, 0.25f)));
        DeadState.AddAction(new ColorAction(spriteRenderer, new Color(0.18f, 0.16f, 0.2f)));

        Condition overPopulation = new OverpopulatedCondition(
            gridManager,
            gridPosition,
            targetState: AliveState
        );
        Condition isolated = new IsolatedCondition(
            gridManager,
            gridPosition,
            targetState: AliveState
        );
        Condition populate = new PopulateCondition(
            gridManager,
            gridPosition,
            targetState: AliveState
        );

        DeadState.Transitions.Add(new StateMachine.Transition(populate, AliveState, null));
        AliveState.Transitions.Add(new StateMachine.Transition(overPopulation, DeadState, null));
        AliveState.Transitions.Add(new StateMachine.Transition(isolated, DeadState, null));

        StateMachine = new StateMachine.StateMachine(DeadState);
    }
    public void StepOne() {
        StateMachine?.Update();
    }

    public void StepTwo() {
        StateMachine?.NextUpdate();
    }

    public void Initialize(GridManager grid, Vector2Int pos) {
        gridManager = grid;
        gridPosition = pos;
        gridManager.SetUnitAt(pos, this);
    }
    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        if (StateMachine == null) return;

        // toggle state on click
        if (StateMachine.CurrentState == AliveState) {
            StateMachine.ChangeState(DeadState);
        } else {
            StateMachine.ChangeState(AliveState);
        }
    }

}
