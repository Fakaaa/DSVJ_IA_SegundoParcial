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
        private Vector3 initialAgentPosition = default;
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

        private int generationsSurvived = 0;
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
            generationsSurvived = 0;
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetInitialiPosition(Vector3 position)
        {
            initialAgentPosition = position;
        }

        public void SetBrain(Genome genome, NeuralNetwork brain, bool resetAI = true)
        {
            this.genome = genome;
            this.brain = brain;
            state = State.Alive;

            agentData = new AgentData();

            agentData.genome = genome;
            agentData.brain = brain;

            lastAgentPosition = behaviour.transform.position;

            if (resetAI)
            {
                OnReset();
            }
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

        public virtual void OnGenerationEnded(out Genome genome)
        {
            genome = null;

            if (foodColected < 1)
            {
                state = State.Dead;
                return;
            }
            else
            {
                state = State.Alive;
            }

            genome = this.genome;
        }
        #endregion

        #region PROTECTED_METHODS
        protected virtual void OnThink(float dt, MapHandler map, FoodHandler food)
        {
            List<Cell> adjacentCellsToAgent = map.GetAdjacentsCellsToPosition(new Vector2Int((int)behaviour.transform.position.x, (int)behaviour.transform.position.y));
            List<float> inputs = new List<float>();
            float[] outputs;
            Vector2Int agentPosition = new Vector2Int((int)behaviour.transform.position.x, (int)behaviour.transform.position.y);

            lastAgentPosition = behaviour.transform.position;

            inputs.Add(foodColected);
            inputs.Add(behaviour.transform.position.magnitude);
            inputs.Add(FindClosestFood(food));           
            
            if (adjacentCellsToAgent.Any())
            {
                for (int i = 0; i < adjacentCellsToAgent.Count; i++)
                {
                    inputs.Add(Vector3.Distance(behaviour.transform.position, new Vector3(adjacentCellsToAgent[i].Position.x, adjacentCellsToAgent[i].Position.y, behaviour.transform.position.z)));
                }
            }

            outputs = brain.Synapsis(inputs.ToArray());

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] < 1.0f && outputs[i] > 0.75f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.UP, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] < 0.75f && outputs[i] > 0.5f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.DOWN, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] < 0.25f && outputs[i] > 0.0f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.RIGHT, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] < 0.5f && outputs[i] > 0.25f)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.LEFT, map.MaxGridX, map.MaxGridY);
                }
                else if (outputs[i] < 0)
                {
                    behaviour.MoveOnDirection(MOVE_DIRECTIONS.NONE, map.MaxGridX, map.MaxGridY);
                }
            }

            /*if (behaviour.transform.position != lastAgentPosition)
            {
                genome.fitness += 0.15f;
            }*/

            if (map != null)
            {
                if (map.Map.ContainsKey(agentPosition))
                {
                    if (map.Map[agentPosition].FoodInCell != null)
                    {
                        genome.fitness += 2;
                        food.AteFood(agentPosition);
                        foodColected++;
                        genome.foodEated = foodColected;

                        map.Map[agentPosition].SetFoodOnCell(null);
                    }
                }
            }

            /*if (foodColected > 2)
            {
                genome.fitness -= 1;
            }*/
        }

        protected virtual void OnDead()
        {
            generationsSurvived = 0;
        }

        protected virtual void OnReset()
        {
            genome.fitness = 0.0f;
            foodColected = 0;
            behaviour.transform.position = initialAgentPosition;
        }
        #endregion

        #region PRIVATE_METHODS
        private float FindClosestFood(FoodHandler food)
        {
            if(food == null || food.FoodInMap == null || food.FoodInMap.Count < 1)
            {
                return 0f;
            }

            float closestFood = Vector3.Distance(behaviour.transform.position, new Vector3(food.FoodInMap[0].Position.x, food.FoodInMap[0].Position.y, behaviour.transform.position.z));

            for (int i = 0; i < food.FoodInMap.Count; i++)
            {
                if (food.FoodInMap[i] != null)
                {
                    float newDistance = Vector3.Distance(behaviour.transform.position, new Vector3(food.FoodInMap[i].Position.x, food.FoodInMap[i].Position.y, behaviour.transform.position.z));
                    if (closestFood < newDistance)
                    {
                        closestFood = newDistance;
                    }
                }
            }

            return closestFood;
        }
        #endregion
    }
}