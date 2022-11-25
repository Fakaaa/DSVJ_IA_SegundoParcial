using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using InteligenciaArtificial.SegundoParcial.Handlers;

using TMPro;

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
        [SerializeField] private SimulationScreen simulationScreen;

        [Header("STATE TEAM")]
        [SerializeField] private Toggle teamSettingState;
        #endregion

        #region PRIVATE_FIELDS
        private string populationText;
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

        private PopulationManager populationManager = null;
        #endregion

        #region PROPERTIES
        public bool IsTeamReady { get { return teamSettingState.isOn; } }
        #endregion

        #region PUBLIC_METHODS
        public void Init(int maxGridX,PopulationManager populationManager, Action onPressLoadSavedAgent)
        {
            this.populationManager = populationManager;

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

            populationCountSlider.minValue = 0;
            populationCountSlider.maxValue = maxGridX;

            populationCountSlider.value = populationManager.PopulationCount;
            eliteCountSlider.value = populationManager.EliteCount;
            mutationChanceSlider.value = Mathf.Round(populationManager.MutationChance * 100.0f);
            mutationRateSlider.value = Mathf.Round(populationManager.MutationRate * 100.0f);
            hiddenLayersCountSlider.value = populationManager.HiddenLayers;
            neuronsPerHLSlider.value = populationManager.NeuronsCountPerHL;
            biasSlider.value = populationManager.Bias;
            sigmoidSlopeSlider.value = populationManager.Sigmoid;
            inputsSlider.value = populationManager.InputsCount;
            outputsSlider.value = populationManager.OutputsCount;

            startButton.onClick.AddListener(() => { OnStartButtonClick(false); });
            startBestAISaved.onClick.AddListener(() => { onPressLoadSavedAgent?.Invoke(); });

            Refresh();

            teamSettingState.isOn = false;
        }

        public void OnStopSimulation()
        {
            simulationScreen.gameObject.SetActive(false);
            teamSettingState.isOn = false;
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnPopulationCountChange(float value)
        {
            populationManager.PopulationCount = (int)value;

            populationCountTxt.text = string.Format(populationText, populationManager.PopulationCount);
        }

        private void OnEliteCountChange(float value)
        {
            populationManager.EliteCount = (int)value;

            eliteCountTxt.text = string.Format(elitesText, populationManager.EliteCount);
        }

        private void OnMutationChanceChange(float value)
        {
            populationManager.MutationChance = value / 100.0f;

            mutationChanceTxt.text = string.Format(mutationChanceText, (int)(populationManager.MutationChance * 100));
        }

        private void OnMutationRateChange(float value)
        {
            populationManager.MutationRate = value / 100.0f;

            mutationRateTxt.text = string.Format(mutationRateText, (int)(populationManager.MutationRate * 100));
        }

        private void OnHiddenLayersCountChange(float value)
        {
            populationManager.HiddenLayers = (int)value;


            hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, populationManager.HiddenLayers);
        }

        private void OnNeuronsPerHLChange(float value)
        {
            populationManager.NeuronsCountPerHL = (int)value;

            neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, populationManager.NeuronsCountPerHL);
        }

        private void OnBiasChange(float value)
        {
            populationManager.Bias = -value;

            biasTxt.text = string.Format(biasText, populationManager.Bias.ToString("0.00"));
        }

        private void OnSigmoidSlopeChange(float value)
        {
            populationManager.Sigmoid = value;

            sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, populationManager.Sigmoid.ToString("0.00"));
        }


        private void OnStartButtonClick(bool bestAI)
        {
            simulationScreen.Init(populationManager);
            simulationScreen.gameObject.SetActive(true);

            teamSettingState.isOn = true;
        }

        private void OnInputsChange(float value)
        {
            populationManager.InputsCount = (int)value;

            inputsTxt.text = string.Format(inputsText, populationManager.InputsCount);
        }

        private void OnOutputsChange(float value)
        {
            populationManager.OutputsCount = (int)value;

            outputsTxt.text = string.Format(outputsText, populationManager.OutputsCount);
        }

        private void Refresh()
        {
            populationCountTxt.text = string.Format(populationText, populationManager.PopulationCount);
            eliteCountTxt.text = string.Format(elitesText, populationManager.EliteCount);
            mutationChanceTxt.text = string.Format(mutationChanceText, (int)(populationManager.MutationChance * 100));
            mutationRateTxt.text = string.Format(mutationRateText, (int)(populationManager.MutationRate * 100));
            hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, populationManager.HiddenLayers);
            neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, populationManager.NeuronsCountPerHL);
            biasTxt.text = string.Format(biasText, populationManager.Bias.ToString("0.00"));
            sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, populationManager.Sigmoid.ToString("0.00"));
            inputsTxt.text = string.Format(inputsText, populationManager.InputsCount);
            outputsTxt.text = string.Format(outputsText, populationManager.OutputsCount);
        }
        #endregion
    }
}