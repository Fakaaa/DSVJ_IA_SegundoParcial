using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;

namespace InteligenciaArtificial.SegundoParcial.Handlers
{
    public class PopulationHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private MapHandler map = default;
        [SerializeField] private FoodHandler food = default;
        [SerializeField] private int initialPopulation = 50;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            Init();
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            map.Init();

            food.Init(map.GetRandomUniquePositions(initialPopulation));
        }
        #endregion
    }
}