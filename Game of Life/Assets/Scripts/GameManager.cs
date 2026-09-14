using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

namespace StateMachine {
    public interface Action {
        void Execute();
    }

    public interface Condition {
        bool IsTrue();
    }

    public class Transition {

        public Condition Condition { get; }
        public State TargetState { get; }
        public List<Action> Actions { get; }

        public Transition(Condition condition, State targetState, List<Action> actions) {
            Condition = condition;
            TargetState = targetState;
            Actions = actions ?? new List<Action>(); // this means if actions is null, make a new list (i found this online)
        }

    }

    public class State {
        public string Name { get; }

        public List<Action> Actions { get; } = new List<Action>();
        public List<Transition> Transitions { get; } = new List<Transition>();

        public State(string name) {
            Name = name;
        }

        public void AddTransition(Condition condition, State target, List<Action> actions) {
            Transitions.Add(new Transition(condition, target, actions));
        }

        // with no action
        public void AddTransition(Condition condition, State target) {
            Transitions.Add(new Transition(condition, target, null));
        }

        public void AddAction(Action action) => Actions.Add(action);
    }

    public class StateMachine {
        private State currentState;

        public State CurrentState => currentState;
        private State nextState;

        public StateMachine(State initialState) {
            ChangeState(initialState);
        }

        public void ChangeState(State newState) {
            if (newState == null || newState == currentState) return;
            nextState = newState;

            // trigger exit actions here

            currentState = newState;

            // trigger entry actions here
            foreach (var action in currentState.Actions) {
                action.Execute();
            }
        }

        public void Update() {
            // check transitions
            foreach (var transition in currentState.Transitions) {
                if (transition.Condition != null && transition.Condition.IsTrue()) {
                    // execute transition actions
                    nextState = transition.TargetState;
                    break;
                }
            }
        }

        public void NextUpdate() {
            // switch state
            if (nextState != null && nextState != currentState) {
                ChangeState(nextState);

            }
        }
    }
    public class ColorAction : Action {

        private readonly Renderer renderer;
        private readonly Color color;
        public ColorAction(Renderer renderer, Color color) {
            this.renderer = renderer;
            this.color = color;
        }
        public void Execute() {
            if (renderer != null) {
                renderer.material.color = color;
            }
        }
    }

    public class OverpopulatedCondition : Condition {
        private readonly GridManager gridManager;
        private readonly Vector2Int gridPosition;
        private readonly State targetState;

        private static readonly Vector2Int[] NeighborOffsets = new Vector2Int[] {
        new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1), // top row
        new Vector2Int(-1,  0),                        new Vector2Int(1,  0), // middle row
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)  // bottom row
        };

        public OverpopulatedCondition(GridManager gridManager, Vector2Int gridPosition, State targetState) {
            this.gridManager = gridManager;
            this.gridPosition = gridPosition;
            this.targetState = targetState;
        }

        public bool IsTrue() {
            int aliveNeighborCount = 0;

            foreach (var offset in NeighborOffsets) {
                var offsetNew = gridManager.WrapAround((gridPosition + offset).x, (gridPosition + offset).y);

                Unit neighbor = gridManager.GetUnitAt(offsetNew);
                if (neighbor.StateMachine?.CurrentState?.Name == targetState.Name) { 
                    aliveNeighborCount++;
                }
            }
            if (aliveNeighborCount >= 4) {
                return true;
            }
            return false;
        }
    }
    public class IsolatedCondition : Condition {
        private readonly GridManager gridManager;
        private readonly Vector2Int gridPosition;
        private readonly State targetState;

        private static readonly Vector2Int[] NeighborOffsets = new Vector2Int[] {
        new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1), // top row
        new Vector2Int(-1,  0),                        new Vector2Int(1,  0), // middle row
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)  // bottom row
        };

        public IsolatedCondition(GridManager gridManager, Vector2Int gridPosition, State targetState) {
            this.gridManager = gridManager;
            this.gridPosition = gridPosition;
            this.targetState = targetState;
        }

        public bool IsTrue() {
            int aliveNeighborCount = 0;

            foreach (var offset in NeighborOffsets) {
                var offsetNew = gridManager.WrapAround((gridPosition + offset).x, (gridPosition + offset).y);

                Unit neighbor = gridManager.GetUnitAt(offsetNew);
                if (neighbor.StateMachine?.CurrentState?.Name == targetState.Name) { 
                    aliveNeighborCount++;
                }
            }
            if (aliveNeighborCount <= 1) {
                return true;
            }
            return false;
        }
    }
    public class PopulateCondition : Condition {
        private readonly GridManager gridManager;
        private readonly Vector2Int gridPosition;
        private readonly State targetState;

        private static readonly Vector2Int[] NeighborOffsets = new Vector2Int[] {
        new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1), // top row
        new Vector2Int(-1,  0),                        new Vector2Int(1,  0), // middle row
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)  // bottom row
        };

        public PopulateCondition(GridManager gridManager, Vector2Int gridPosition, State targetState) {
            this.gridManager = gridManager;
            this.gridPosition = gridPosition;
            this.targetState = targetState;
        }

        public bool IsTrue() {
            int aliveNeighborCount = 0;

            foreach (var offset in NeighborOffsets) {
                var offsetNew = gridManager.WrapAround((gridPosition + offset).x, (gridPosition + offset).y);

                Unit neighbor = gridManager.GetUnitAt(offsetNew);
                if (neighbor != null && neighbor.StateMachine != null) {
                    if (neighbor.StateMachine?.CurrentState?.Name == targetState.Name) {
                        aliveNeighborCount++;
                    }
                }
            }
            if (aliveNeighborCount == 3) {
                return true;
            }
            return false;
        }
    }
}

public class GameManager : MonoBehaviour {

}
