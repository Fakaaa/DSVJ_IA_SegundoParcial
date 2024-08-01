using InteligenciaArtificial.SegundoParcial.Agents;
using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.Handlers;

using TMPro;

namespace InteligenciaArtificial.SegundoParcial.View
{
    public class SimulationScreen : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TMP_Text generationsCountTxt;
        [SerializeField] private TMP_Text bestFitnessTxt;
        [SerializeField] private TMP_Text avgFitnessTxt;
        [SerializeField] private TMP_Text worstFitnessTxt;
        [SerializeField] private TMP_Text actualPopulation;
        [SerializeField] private TMP_Text maxPopulation;
        [SerializeField] private TMP_Text timerTxt;
        [SerializeField] private Slider timerSlider;
        [SerializeField] private TMP_Text agentLoadedData;
        [SerializeField] private GameObject agentLoadedGo;
        #endregion

        #region PRIVATE_FIELDS
        private string generationsCountText;
        private string bestFitnessText;
        private string avgFitnessText;
        private string worstFitnessText;
        private string timerText;
        private int lastGeneration = 0;

        private const string agentDataFormat = "Agent Loaded\nGen: {0}, Fitness: {1}, Food Ate: {2}";
        
        private PopulationManager populationManager= null;
        #endregion

        #region UNITY_CALLS
        void OnEnable()
        {
            if (string.IsNullOrEmpty(generationsCountText))
                generationsCountText = generationsCountTxt.text;
            if (string.IsNullOrEmpty(bestFitnessText))
                bestFitnessText = bestFitnessTxt.text;
            if (string.IsNullOrEmpty(avgFitnessText))
                avgFitnessText = avgFitnessTxt.text;
            if (string.IsNullOrEmpty(worstFitnessText))
                worstFitnessText = worstFitnessTxt.text;

            generationsCountTxt.text = string.Format(generationsCountText, 0);
            bestFitnessTxt.text = string.Format(bestFitnessText, 0);
            avgFitnessTxt.text = string.Format(avgFitnessText, 0);
            worstFitnessTxt.text = string.Format(worstFitnessText, 0);

            actualPopulation.text = populationManager.PopulationCount.ToString();
            maxPopulation.text = "/ " + populationManager.PopulationCount.ToString();
        }

        void LateUpdate()
        {
            if (lastGeneration != populationManager.generation)
            {
                lastGeneration = populationManager.generation;
                generationsCountTxt.text = string.Format(generationsCountText, populationManager.generation);
                bestFitnessTxt.text = string.Format(bestFitnessText, populationManager.bestFitness);
                avgFitnessTxt.text = string.Format(avgFitnessText, populationManager.avgFitness);
                worstFitnessTxt.text = string.Format(worstFitnessText, populationManager.worstFitness);

                actualPopulation.text = populationManager.actualPopulation.ToString();
                maxPopulation.text = "/ " + populationManager.PopulationCount.ToString();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(PopulationManager populationManager)
        {
            this.populationManager = populationManager;

            timerSlider.onValueChanged.AddListener(OnTimerChange);
            timerText = timerTxt.text;

            timerTxt.text = string.Format(timerText, populationManager.IterationCount);

            if (string.IsNullOrEmpty(generationsCountText))
                generationsCountText = generationsCountTxt.text;
            if (string.IsNullOrEmpty(bestFitnessText))
                bestFitnessText = bestFitnessTxt.text;
            if (string.IsNullOrEmpty(avgFitnessText))
                avgFitnessText = avgFitnessTxt.text;
            if (string.IsNullOrEmpty(worstFitnessText))
                worstFitnessText = worstFitnessTxt.text;
        }

        public void SetAgentLoadedData(AgentData agentData)
        {
            if (agentData == null)
            {
                agentLoadedGo.SetActive(false);
            }
            else
            {
                if (!agentLoadedGo.activeInHierarchy)
                {
                    agentLoadedGo.SetActive(true);
                }
                
                agentLoadedData.text = string.Format(agentDataFormat, agentData.generation, agentData.genome.fitness, agentData.genome.foodEated);
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnTimerChange(float value)
        {
            populationManager.IterationCount = (int)value;
            timerTxt.text = string.Format(timerText, populationManager.IterationCount);
        }        
        #endregion
    }
}