using System.Collections.Generic;

using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Utils;

namespace InteligenciaArtificial.SegundoParcial.Handlers.Map.Food
{
    [System.Serializable]
    public class Food
    {
        #region PRIVATE_FIELDS
        private Vector2Int position;
        #endregion

        #region PORPERTIES
        public Vector2Int Position => position;
        #endregion

        #region CONSTRUCTOR
        public Food(Vector2Int position)
        {
            this.position = position;
        }
        #endregion
    }

    public class FoodHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private MapHandler mapHandler;
        [SerializeField] private FoodObject prefabFood = null;
        [SerializeField] private Transform holder = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<Food> foodInMap = null;
        private List<FoodObject> foodObjects = null;
        private int foodAmount = 0;
        #endregion

        #region PROPERTIES
        public List<Food> FoodInMap => foodInMap;
        #endregion


        #region PUBLIC_METHODS
        public void Init(List<Vector2Int> foodPositions)
        {
            foodAmount = foodPositions.Count;

            foodInMap = new List<Food>();
            foodObjects = new List<FoodObject>();

            for (int i = 0; i < foodPositions.Count; i++)
            {
                if (foodPositions[i] != SimulationConstants.InvalidPosition)
                {
                    Food food = new Food(foodPositions[i]);

                    foodInMap.Add(food);

                    FoodObject foodGo = Instantiate(prefabFood, new Vector3(food.Position.x, food.Position.y, 2), Quaternion.identity);
                    foodGo.SetFoodData(food);
                    foodGo.name = "Food_" + foodPositions[i].x + "_" + foodPositions[i].y;
                    foodObjects.Add(foodGo);
                }
            }
        }

        public void DeInit()
        {
            for (int i = 0; i < foodObjects.Count; i++)
            {
                if (foodObjects[i] != null)
                {
                    Destroy(foodObjects[i].gameObject);
                }
            }

            foodObjects.Clear();
            foodInMap.Clear();

            foodObjects = null;
            foodInMap = null;
        }

        public void AteFood(Vector2Int agentPosition)
        {
            for (int i = 0; i < foodObjects.Count; i++)
            {
                if (foodObjects[i] != null)
                {
                    if (agentPosition == new Vector2Int((int)foodObjects[i].transform.position.x, (int)foodObjects[i].transform.position.y))
                    {
                        Destroy(foodObjects[i].gameObject);
                        foodObjects.Remove(foodObjects[i]);
                        break;
                    }
                }
            }

            Food toRemove = foodInMap.Find(food => food.Position == agentPosition);
            if (foodInMap.Contains(toRemove))
            {
                foodInMap.Remove(toRemove);
            }
        }
        #endregion
    }
}