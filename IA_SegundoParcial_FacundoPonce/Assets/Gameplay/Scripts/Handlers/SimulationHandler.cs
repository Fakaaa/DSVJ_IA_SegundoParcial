using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.View;
using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;

using InteligenciaArtificial.SegundoParcial.Utils.CameraHandler;

namespace InteligenciaArtificial.SegundoParcial.Handlers
{
    [System.Serializable]
    public class AgentsTeam
    {
        public PopulationManager PopulationManager;
        public StartConfigurationScreen StartConfiguration;
    }

    public class SimulationHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private List<AgentsTeam> teams = null;
        [SerializeField] private MapHandler map = null;
        [SerializeField] private FoodHandler food = null;
        [SerializeField] private CameraHandler cameraHandler = null;
        [SerializeField] private Button pauseBtn = null;
        [SerializeField] private Button stopBtn = null;
        #endregion

        #region PRIVATE_FIELDS
        private bool simulationStarted = false;
        private int teamsNeededForBegin = 0;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            teamsNeededForBegin = teams.Count;

            Init();
        }

        private void Update()
        {
            if(simulationStarted) return;

            int teamsReady = 0;

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null && teams[i].StartConfiguration.IsTeamReady)
                {
                    teamsReady++;
                }
            }

            if(teamsReady == teamsNeededForBegin)
            {
                simulationStarted = true;
                OnStartedSimulation();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            simulationStarted = false;

            map.Init();

            pauseBtn.onClick.AddListener(OnPauseButtonClick);
            stopBtn.onClick.AddListener(OnStopButtonClick);

            pauseBtn.gameObject.SetActive(false);
            stopBtn.gameObject.SetActive(false);

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    teams[i].StartConfiguration.Init(map.MaxGridX ,teams[i].PopulationManager, null);
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnStartedSimulation()
        {
            pauseBtn.gameObject.SetActive(true); 
            stopBtn.gameObject.SetActive(true);

            int finalAmountFoodRequeired = 0;

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    finalAmountFoodRequeired += teams[i].PopulationManager.PopulationCount;

                    teams[i].StartConfiguration.gameObject.SetActive(false);

                    List<Vector2Int> finalTeamPositions = new List<Vector2Int>();
                    
                    if(teams[i].PopulationManager.teamId == "Red")
                    {
                        List<Cell> leftToRightCells = map.GetLeftToRightBottomCells();
                        for (int j = 0; j < leftToRightCells.Count; j++)
                        {
                            if (leftToRightCells[j] != null)
                            {
                                finalTeamPositions.Add(leftToRightCells[j].Position);
                            }
                        }

                        teams[i].PopulationManager.StartSimulation(finalTeamPositions);
                    }
                    else
                    {
                        List<Cell> rightToLeftCells = map.GetRightToLeftTopCells();
                        for (int j = 0; j < rightToLeftCells.Count; j++)
                        {
                            if (rightToLeftCells[j] != null)
                            {
                                finalTeamPositions.Add(rightToLeftCells[j].Position);
                            }
                        }

                        teams[i].PopulationManager.StartSimulation(finalTeamPositions);
                    }
                }
            }

            food.Init(map.GetRandomUniquePositions(finalAmountFoodRequeired));
        }

        private void OnPauseButtonClick()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    teams[i].PopulationManager.PauseSimulation();
                }
            }
        }

        private void OnStopButtonClick()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    teams[i].StartConfiguration.gameObject.SetActive(true);
                    teams[i].StartConfiguration.OnStopSimulation();
                    teams[i].PopulationManager.StopSimulation();
                }
            }

            simulationStarted = false;

            pauseBtn.gameObject.SetActive(false);
            stopBtn.gameObject.SetActive(false);

            cameraHandler.ResetCamera();
            food.DeInit();
        }
        #endregion
    }
}