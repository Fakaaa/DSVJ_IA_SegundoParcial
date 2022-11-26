using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        #region PRIVATE_FIELDS
        private Vector3 lastAgentPosition = default;
        private int foodColected = 0;
        #endregion

        #region PROTECTED_METHODS
        protected Genome genome;
        protected NeuralNetwork brain;
        protected AgentBehaviour behaviour;
        protected int currentTurn = 0;
        protected int foodEaten = 0;
        protected int currentIteration = 0;
        protected AgentData agentData;
        #endregion

        #region PROPERTIES
        public State state
        {
            get; private set;
        }
        public Genome Genome { get { return genome; } }
        public NeuralNetwork Brain { get { return brain; } }
        public AgentData AgentData { get { return agentData; } }
        public int CurrentTurn { get { return currentTurn; } }
        public int FoodEaten { get { return foodEaten; } }
        public int CurrentIteration { get { return currentIteration; } }
        #endregion

        #region UNITY_CALLS
        private void Awake()
        {
            behaviour = GetComponent<AgentBehaviour>();
            lastAgentPosition = default;
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetBrain(Genome genome, NeuralNetwork brain)
        {
            this.genome = genome;
            this.brain = brain;
            state = State.Alive;

            agentData = new AgentData();

            agentData.genome = genome;
            agentData.brain = brain;

            lastAgentPosition = behaviour.transform.position;
        }

        public void Think(float dt, int actualTurn, int iteration, MapHandler map, FoodHandler food)
        {
            if (state == State.Alive)
            {
                currentTurn = actualTurn;
                currentIteration = iteration;

                OnThink(dt, map, food);
            }
        }
        #endregion

        #region PROTECTED_METHODS
        protected virtual void OnThink(float dt, MapHandler map, FoodHandler food)
        {
            List<Cell> adjacentCellsToAgent = map.GetAdjacentsCellsToPosition(new Vector2Int((int)behaviour.transform.position.x, (int)behaviour.transform.position.y));
            float[] inputs = new float[adjacentCellsToAgent.Count];
            float[] outputs;

            lastAgentPosition = behaviour.transform.position;

            if (adjacentCellsToAgent.Any())
            {
                for (int i = 0; i < adjacentCellsToAgent.Count; i++)
                {
                    inputs[i] = (behaviour.transform.position - new Vector3(adjacentCellsToAgent[i].Position.x, adjacentCellsToAgent[i].Position.y, behaviour.transform.position.z)).magnitude / 2f;
                }
            }

            outputs = brain.Synapsis(inputs);

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > -1.0f && outputs[i] < -0.75f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.UP, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] > -0.75f && outputs[i] < -0.5f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.DOWN, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] > -0.5f && outputs[i] < -0.25f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.RIGHT, map.MaxGridX, map.MaxGridY);
                }
                else if(outputs[i] > -0.25f && outputs[i] < 0f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.LEFT, map.MaxGridX, map.MaxGridY);
                }
                else if(outputs[i] > 0)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.NONE, map.MaxGridX, map.MaxGridY);
                }
            }

            if(behaviour.transform.position != lastAgentPosition)
            {
                genome.fitness += 1;
            }

            if(map != null)
            {
                Vector2Int agentPosition = new Vector2Int((int)behaviour.transform.position.x, (int)behaviour.transform.position.y);
                if (map.Map.ContainsKey(agentPosition))
                {
                    if (map.Map[agentPosition].FoodInCell != null)
                    {
                        genome.fitness *= 2;
                        food.EatedFood(map.Map[agentPosition].FoodInCell);
                        foodColected++;

                        map.Map[agentPosition].SetFoodOnCell(null);
                    }
                }
            }

            if(foodColected > 2)
            {
                genome.fitness -= 1;
            }
        }
        protected virtual void OnDead()
        {

        }

        protected virtual void OnReset()
        {
            genome.fitness = 0.0f;
        }
        #endregion
    }
}