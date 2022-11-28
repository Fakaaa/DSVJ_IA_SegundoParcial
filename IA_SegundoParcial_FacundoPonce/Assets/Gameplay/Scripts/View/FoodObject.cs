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

        #region PUBLIC_METHODS
        public void SetFoodData(Food data)
        {
            foodData = data;
        }
        #endregion        
    }
}