using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;
using InteligenciaArtificial.SegundoParcial.View;

namespace InteligenciaArtificial.SegundoParcial.Handlers
{
    public class SimulationHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private PopulationManager populationManager = default;
        [SerializeField] private MapHandler map = default;
        [SerializeField] private FoodHandler food = default;
        [SerializeField] private StartConfigurationScreen configurationScreen = null;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private Button stopBtn;
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

            food.Init(map.GetRandomUniquePositions(populationManager.PopulationCount));

            populationManager.StartSimulation();

            pauseBtn.onClick.AddListener(OnPauseButtonClick);
            stopBtn.onClick.AddListener(OnStopButtonClick);
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnPauseButtonClick()
        {
            PopulationManager.Instance.PauseSimulation();
        }

        private void OnStopButtonClick()
        {
            PopulationManager.Instance.StopSimulation();
            this.gameObject.SetActive(false);
            configurationScreen.gameObject.SetActive(true);
        }
        #endregion
    }
}