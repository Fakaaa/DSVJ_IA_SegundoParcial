using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.View;
using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;

using InteligenciaArtificial.SegundoParcial.Utils.CameraHandler;
using TMPro;
using InteligenciaArtificial.SegundoParcial.Agents;
using InteligenciaArtificial.SegundoParcial.Utils.Files;

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

        [Header("SIMULATION SETTINGS")]
        [SerializeField] private TMP_Text txtTurnAmount = null;
        [SerializeField] private int maxTurnsAmount = 0;
        [SerializeField] private bool saveBestAgentOfEachTeam = false;
        #endregion

        #region PRIVATE_FIELDS
        private bool simulationStarted = false;
        private int teamsNeededForBegin = 0;
        private int currentTurn = 0;

        private float delayPerNextTurn = 0f;
        private float time = 0.0f;

        private int totalFoodPerCountOfAIs = 0;
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            teamsNeededForBegin = teams.Count;

            Init();
        }

        private void Update()
        {
            InitializeSimulation();

            if (simulationStarted)
            {
                UpdateTurnWhenNeeded();
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
                    teams[i].StartConfiguration.Init(map.MaxGridX, teams[i].PopulationManager, null);
                }
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitializeSimulation()
        {
            if (simulationStarted) return;

            int teamsReady = 0;

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null && teams[i].StartConfiguration.IsTeamReady)
                {
                    teamsReady++;
                }
            }

            if (teamsReady == teamsNeededForBegin)
            {
                simulationStarted = true;
                OnStartedSimulation();
            }
        }

        private void UpdateTeams()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    if (teams[i].PopulationManager != null)
                    {
                        teams[i].PopulationManager.UpdatePopulation();
                    }
                }
            }
        }

        private void UpdateTurnWhenNeeded()
        {
            if (currentTurn < maxTurnsAmount)
            {
                if (CheckIfAllTeamsAgentsThink())
                {
                    if (time < delayPerNextTurn)
                    {
                        time += Time.deltaTime;
                    }
                    else
                    {
                        time = 0;
                        currentTurn++;

                        txtTurnAmount.text = "Turn: " + currentTurn.ToString();

                        for (int i = 0; i < teams.Count; i++)
                        {
                            if (teams[i] != null)
                            {
                                if (teams[i].PopulationManager != null)
                                {
                                    teams[i].PopulationManager.UpdateTurn(currentTurn);
                                }
                            }
                        }
                    }
                }
                else
                {
                    UpdateTeams();
                }

            }
            else
            {
                currentTurn = maxTurnsAmount;

                if (currentTurn == maxTurnsAmount)
                {
                    txtTurnAmount.text = "Turn: Max Turns, Simulation Paused";
                    OnEndedAllTurns();
                }
            }
        }

        private bool CheckIfAllTeamsAgentsThink()
        {
            int compleatedTeams = 0;

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    if (teams[i].PopulationManager != null)
                    {
                        if (teams[i].PopulationManager.AllAgentsAlreadyThink())
                        {
                            compleatedTeams++;
                        }
                    }
                }
            }

            return compleatedTeams == teams.Count;
        }

        private void OnStartedSimulation()
        {
            pauseBtn.gameObject.SetActive(true);
            stopBtn.gameObject.SetActive(true);

            totalFoodPerCountOfAIs = 0;

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    totalFoodPerCountOfAIs += teams[i].PopulationManager.PopulationCount;

                    teams[i].StartConfiguration.gameObject.SetActive(false);

                    List<Vector2Int> finalTeamPositions = new List<Vector2Int>();

                    if (teams[i].PopulationManager.teamId == "Red")
                    {
                        List<Cell> leftToRightCells = map.GetLeftToRightBottomCells();
                        for (int j = 0; j < leftToRightCells.Count; j++)
                        {
                            if (leftToRightCells[j] != null)
                            {
                                finalTeamPositions.Add(leftToRightCells[j].Position);
                            }
                        }
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

                    }

                    teams[i].PopulationManager.StartSimulation(finalTeamPositions, map, food);
                }
            }

            food.Init(map.GetRandomUniquePositions(totalFoodPerCountOfAIs*2));
            map.SetGeneratedFoodOnCells(food.FoodInMap);
        }

        private void OnEndedAllTurns()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    teams[i].PopulationManager.EndedGeneration();
                }
            }

            food.DeInit();
            map.ClearFoodOnCells();

            currentTurn = 0;
            txtTurnAmount.text = "Turn: " + currentTurn.ToString();

            food.Init(map.GetRandomUniquePositions(totalFoodPerCountOfAIs * 2));
            map.SetGeneratedFoodOnCells(food.FoodInMap);
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
            if (saveBestAgentOfEachTeam)
            {
                SaveBestAgentOfEachTeam();
            }

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

            currentTurn = 0;
            txtTurnAmount.text = "Turn: " + currentTurn.ToString();
        }

        private void SaveBestAgentOfEachTeam()
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] != null)
                {
                    AgentBase bestTeamAgent = teams[i].PopulationManager.GetBestAgent();

                    FileHandler<AgentData>.Save(bestTeamAgent.AgentData, teams[i].PopulationManager.teamId, bestTeamAgent.CurrentIteration.ToString(), bestTeamAgent.Genome.fitness.ToString());
                }
            }
        }
        #endregion
    }
}