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
        [SerializeField] private TMP_Text timerTxt;
        [SerializeField] private Slider timerSlider;        
        #endregion

        #region PRIVATE_FIELDS
        private string generationsCountText;
        private string bestFitnessText;
        private string avgFitnessText;
        private string worstFitnessText;
        private string timerText;
        private int lastGeneration = 0;

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