using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;

namespace InteligenciaArtificial.SegundoParcial.Agents
{
    [System.Serializable]
    public class AgentData
    {
        public Genome genome;
        public NeuralNetwork brain;
    }

    public enum State
    {
        Alive,
        Dead
    }
    
    public class AgentBase : MonoBehaviour
    {
        #region PROTECTED_METHODS
        protected Genome genome;
        protected NeuralNetwork brain;
        protected AgentBehaviour behaviour;
        protected int currentTurn = 0;
        protected int foodEaten = 0;
        protected int currentIteration = 0;
        #endregion

        #region PROPERTIES
        public State state
        {
            get; private set;
        }
        public Genome Genome { get { return genome; } }
        public NeuralNetwork Brain { get { return brain; } }
        public int CurrentTurn { get { return currentTurn; } }
        public int FoodEaten { get { return foodEaten; } }
        public int CurrentIteration { get { return currentIteration; } }
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            behaviour = GetComponent<AgentBehaviour>();
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetBrain(Genome genome, NeuralNetwork brain)
        {
            this.genome = genome;
            this.brain = brain;
            state = State.Alive;
        }

        public void Think(float dt, int actualTurn, int iteration)
        {
            if (state == State.Alive)
            {
                currentTurn = actualTurn;
                currentIteration = iteration;
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected virtual void OnThink(float dt, AgentBehaviour birdBehaviour, Food closestFood)
        {

        }
        #endregion
    }
}