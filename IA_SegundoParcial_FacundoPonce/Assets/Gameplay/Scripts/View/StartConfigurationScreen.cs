using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.Handlers;

using TMPro;
using System.Collections.Generic;

namespace InteligenciaArtificial.SegundoParcial.View
{
    public class StartConfigurationScreen : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TMP_Text populationCountTxt;
        [SerializeField] private Slider populationCountSlider;
        [SerializeField] private TMP_Text eliteCountTxt;
        [SerializeField] private Slider eliteCountSlider;
        [SerializeField] private TMP_Text mutationChanceTxt;
        [SerializeField] private Slider mutationChanceSlider;
        [SerializeField] private TMP_Text mutationRateTxt;
        [SerializeField] private Slider mutationRateSlider;
        [SerializeField] private TMP_Text hiddenLayersCountTxt;
        [SerializeField] private Slider hiddenLayersCountSlider;
        [SerializeField] private TMP_Text neuronsPerHLCountTxt;
        [SerializeField] private Slider neuronsPerHLSlider;
        [SerializeField] private TMP_Text biasTxt;
        [SerializeField] private Slider biasSlider;
        [SerializeField] private TMP_Text sigmoidSlopeTxt;
        [SerializeField] private Slider sigmoidSlopeSlider;
        [SerializeField] private TMP_Text inputsTxt;
        [SerializeField] private Slider inputsSlider;
        [SerializeField] private TMP_Text outputsTxt;
        [SerializeField] private Slider outputsSlider;
        [SerializeField] private Button startButton;
        [SerializeField] private Button startBestAISaved;
        [SerializeField] private List<GameObject> simulationScreen;
        #endregion

        #region PRIVATE_FIELDS
        private string populationText;
        private string minesText;
        private string generationDurationText;
        private string elitesText;
        private string mutationChanceText;
        private string mutationRateText;
        private string hiddenLayersCountText;
        private string biasText;
        private string sigmoidSlopeText;
        private string neuronsPerHLCountText;
        private string inputsText;
        private string outputsText;
        #endregion

        #region UNITY_CALLS
        void Start()
        {
            populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
            eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
            mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
            mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
            hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
            neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
            biasSlider.onValueChanged.AddListener(OnBiasChange);
            sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);
            inputsSlider.onValueChanged.AddListener(OnInputsChange);
            outputsSlider.onValueChanged.AddListener(OnOutputsChange);

            populationText = populationCountTxt.text;
            elitesText = eliteCountTxt.text;
            mutationChanceText = mutationChanceTxt.text;
            mutationRateText = mutationRateTxt.text;
            hiddenLayersCountText = hiddenLayersCountTxt.text;
            neuronsPerHLCountText = neuronsPerHLCountTxt.text;
            biasText = biasTxt.text;
            sigmoidSlopeText = sigmoidSlopeTxt.text;
            inputsText = inputsTxt.text;
            outputsText = outputsTxt.text;

            populationCountSlider.value = PopulationManager.Instance.PopulationCount;
            eliteCountSlider.value = PopulationManager.Instance.EliteCount;
            mutationChanceSlider.value = Mathf.Round(PopulationManager.Instance.MutationChance * 100.0f);
            mutationRateSlider.value = Mathf.Round(PopulationManager.Instance.MutationRate * 100.0f);
            hiddenLayersCountSlider.value = PopulationManager.Instance.HiddenLayers;
            neuronsPerHLSlider.value = PopulationManager.Instance.NeuronsCountPerHL;
            biasSlider.value = PopulationManager.Instance.Bias;
            sigmoidSlopeSlider.value = PopulationManager.Instance.Sigmoid;
            inputsSlider.value = PopulationManager.Instance.InputsCount;
            outputsSlider.value = PopulationManager.Instance.OutputsCount;

            startButton.onClick.AddListener(() => { OnStartButtonClick(false); });
            startBestAISaved.onClick.AddListener(() => { OnStartButtonClick(true); });

            Refresh();
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnPopulationCountChange(float value)
        {
            PopulationManager.Instance.PopulationCount = (int)value;

            populationCountTxt.text = string.Format(populationText, PopulationManager.Instance.PopulationCount);
        }

        private void OnEliteCountChange(float value)
        {
            PopulationManager.Instance.EliteCount = (int)value;

            eliteCountTxt.text = string.Format(elitesText, PopulationManager.Instance.EliteCount);
        }

        private void OnMutationChanceChange(float value)
        {
            PopulationManager.Instance.MutationChance = value / 100.0f;

            mutationChanceTxt.text = string.Format(mutationChanceText, (int)(PopulationManager.Instance.MutationChance * 100));
        }

        private void OnMutationRateChange(float value)
        {
            PopulationManager.Instance.MutationRate = value / 100.0f;

            mutationRateTxt.text = string.Format(mutationRateText, (int)(PopulationManager.Instance.MutationRate * 100));
        }

        private void OnHiddenLayersCountChange(float value)
        {
            PopulationManager.Instance.HiddenLayers = (int)value;


            hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, PopulationManager.Instance.HiddenLayers);
        }

        private void OnNeuronsPerHLChange(float value)
        {
            PopulationManager.Instance.NeuronsCountPerHL = (int)value;

            neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, PopulationManager.Instance.NeuronsCountPerHL);
        }

        private void OnBiasChange(float value)
        {
            PopulationManager.Instance.Bias = -value;

            biasTxt.text = string.Format(biasText, PopulationManager.Instance.Bias.ToString("0.00"));
        }

        private void OnSigmoidSlopeChange(float value)
        {
            PopulationManager.Instance.Sigmoid = value;

            sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, PopulationManager.Instance.Sigmoid.ToString("0.00"));
        }


        private void OnStartButtonClick(bool bestAI)
        {
            PopulationManager.Instance.StartSimulation(bestAI);
            this.gameObject.SetActive(false);

            for (int i = 0; i < simulationScreen.Count; i++)
            {
                if (simulationScreen[i] != null)
                {
                    simulationScreen[i].SetActive(true);
                }
            }
        }

        private void OnInputsChange(float value)
        {
            PopulationManager.Instance.InputsCount = (int)value;

            inputsTxt.text = string.Format(inputsText, PopulationManager.Instance.InputsCount);
        }

        private void OnOutputsChange(float value)
        {
            PopulationManager.Instance.OutputsCount = (int)value;

            outputsTxt.text = string.Format(outputsText, PopulationManager.Instance.OutputsCount);
        }

        private void Refresh()
        {
            populationCountTxt.text = string.Format(populationText, PopulationManager.Instance.PopulationCount);
            eliteCountTxt.text = string.Format(elitesText, PopulationManager.Instance.EliteCount);
            mutationChanceTxt.text = string.Format(mutationChanceText, (int)(PopulationManager.Instance.MutationChance * 100));
            mutationRateTxt.text = string.Format(mutationRateText, (int)(PopulationManager.Instance.MutationRate * 100));
            hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, PopulationManager.Instance.HiddenLayers);
            neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, PopulationManager.Instance.NeuronsCountPerHL);
            biasTxt.text = string.Format(biasText, PopulationManager.Instance.Bias.ToString("0.00"));
            sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, PopulationManager.Instance.Sigmoid.ToString("0.00"));
            inputsTxt.text = string.Format(inputsText, PopulationManager.Instance.InputsCount);
            outputsTxt.text = string.Format(outputsText, PopulationManager.Instance.OutputsCount);
        }
        #endregion
    }
}