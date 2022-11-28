using System;
using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;
using Random = UnityEngine.Random;

namespace InteligenciaArtificial.SegundoParcial.Agents
{
    public enum MOVE_DIRECTIONS
    {
        UP,
        LEFT,
        RIGHT,
        DOWN,
        NONE
    }

    public class AgentBehaviour : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private FoodHandler foodHandler = null;
        private Action<Vector2Int> onAteFood = null;
        private Action onAgentDie = null;
        private string teamID = null;
        private Vector3 lastPosition = default;

        public  bool foodDisput = false;
        #endregion

        #region EXPOSED_FIELDS
        public Action<FoodObject, Collider2D ,AgentBehaviour,AgentBehaviour> onFoodDisput = null;
        #endregion
        
        #region UNITY_CALLS
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Food")
            {
                FoodObject food = null;

                if(collision.gameObject.TryGetComponent(out food ))
                {
                    RaycastHit2D[] allHits = Physics2D.BoxCastAll(gameObject.transform.position, Vector2.one, 0f, Vector2.up);

                    bool collideWithEnemyAgent = false;
                    AgentBehaviour enemyAgent = null;
                    
                    foreach (RaycastHit2D hit in allHits)
                    {
                        if (hit.collider.gameObject != gameObject)
                        {
                            if (hit.collider.gameObject.TryGetComponent(out AgentBehaviour agent))
                            {
                                if (agent.teamID != teamID)
                                {
                                    collideWithEnemyAgent = true;
                                    enemyAgent = agent;
                                    Debug.Log("Other agent try eat.");
                                }
                            }
                        }
                    }

                    //if (!collideWithEnemyAgent)
                    //{
                    //    EatFood(food, collision);
                    //}
                    EatFood(food, collision);
                    //else
                    //{
//                    //    int desicion = CheckLuck();
////
                    //    if (desicion < 50)
                    //    {
                    //        Debug.Log("Chose eat");
                    //        onFoodDisput?.Invoke(food, collision,this,enemyAgent);
                    //        enemyAgent.foodDisput = true;
                    //        foodDisput = true;
                    //        return; 
                    //    }
                    //    else
                    //    {
                    //        MoveToLastPositon();
                    //    }
                    //}
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void AgentDie()
        {
            foodDisput = false;
            onAgentDie?.Invoke();
        }
        
        public int CheckLuck()
        {
            return Random.Range(0, 100);
        }
        
        public void EatFood(FoodObject foodGo,Collider2D collision)
        {
            onAteFood?.Invoke(foodGo.FoodData.Position);
            foodHandler.AteFood(foodGo.FoodData.Position);

            foodDisput = false;

            collision.enabled = false;
            collision.gameObject.transform.localScale *= 1.05f;
            Destroy(collision.gameObject, 0.25f);
        }
        
        public void SetBehaviourNeeds(Action<Vector2Int> onAteFood, Action onDie ,FoodHandler food, string teamID)
        {
            this.onAteFood = onAteFood;
            this.teamID = teamID;
            this.onAgentDie = onDie;
            
            if (foodHandler != null) return;

            foodHandler = food;
        }

        public void MoveToLastPositon()
        {
            foodDisput = false;
            transform.position = lastPosition;
        }
        
        public void MoveOnDirection(MOVE_DIRECTIONS moveDirection, int limitX, int limitY, Action OnReachLimitY = null)
        {
            lastPosition = transform.position;

            switch (moveDirection)
                {
                    case MOVE_DIRECTIONS.UP:
    
                        if(transform.position.y +1 < limitY)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                        }
                        else
                        {
                            OnReachLimitY?.Invoke();
                        }
    
                        break;
                    case MOVE_DIRECTIONS.LEFT:
    
                        if (transform.position.x - 1 > 0)
                        {
                            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                        }
                        else
                        {
                            transform.position = new Vector3(limitY-1, transform.position.y, transform.position.z);
                        }
    
                        break;
                    case MOVE_DIRECTIONS.RIGHT:
    
                        if(transform.position.x + 1 < limitY)
                        {
                            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                        }
                        else
                        {
                            transform.position = new Vector3(0, transform.position.y, transform.position.z);
                        }
    
                        break;
                    case MOVE_DIRECTIONS.DOWN:
    
                        if(transform.position.y -1 > 0)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                        }
                        else
                        {
                            OnReachLimitY?.Invoke();
                        }
    
                        break;
                    case MOVE_DIRECTIONS.NONE:
                        transform.position = transform.position;
                        break;
                }
        }
        #endregion
    }
}