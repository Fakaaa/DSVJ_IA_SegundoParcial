using System;
using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;

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
        #endregion

        #region UNITY_CALLS
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Food")
            {
                Debug.Log("Colision con comida");
                FoodObject food = null;

                if(collision.gameObject.TryGetComponent(out food ))
                {
                    onAteFood?.Invoke(food.FoodData.Position);
                    foodHandler.AteFood(food.FoodData.Position);

                    collision.enabled = false;
                    collision.gameObject.transform.localScale *= 1.05f;
                    Destroy(collision.gameObject, 0.25f);
                }
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetBehaviourNeeds(Action<Vector2Int> onAteFood, FoodHandler food)
        {
            this.onAteFood = onAteFood;

            if (foodHandler != null) return;

            foodHandler = food;
        }

        public void MoveOnDirection(MOVE_DIRECTIONS moveDirection, int limitX, int limitY, Action OnReachLimitY = null)
        {
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