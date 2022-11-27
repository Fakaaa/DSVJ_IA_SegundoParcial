using UnityEngine;

namespace InteligenciaArtificial.SegundoParcial.Handlers.Map.Food
{
    public class FoodObject : MonoBehaviour
    {
        #region PRIVATE_FIELDS
        private Food foodData = null;
        #endregion

        #region PROPERTIES
        public Food FoodData { get { return foodData; } }
        #endregion

        #region UNITY_CALLS
        private void OnDestroy()
        {
            Debug.Log("DESTROYED FOOD ?");
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetFoodData(Food data)
        {
            foodData = data;
        }
        #endregion        
    }
}