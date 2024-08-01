using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Agents;
using InteligenciaArtificial.SegundoParcial.Handlers.Map;
using InteligenciaArtificial.SegundoParcial.Handlers.Map.Food;
using System;

namespace InteligenciaArtificial.SegundoParcial.Handlers
{
    public class PopulationManager : MonoBehaviour
    {
        [Header("TEAM SETTINGS")]
        public string teamId = null;
        public GameObject AgentPrefab;
        public int PopulationCount = 40;
        public int IterationCount = 1;

        public int EliteCount = 4;
        public float MutationChance = 0.10f;
        public float MutationRate = 0.01f;

        public int InputsCount = 4;
        public int HiddenLayers = 1;
        public int OutputsCount = 2;
        public int NeuronsCountPerHL = 7;
        public float Bias = 1f;
        public float Sigmoid = 0.5f;

        [Space(20)]
        [Header("SAVE DATA")]
        public bool saveDataOnNextEvolve = false;

        GeneticAlgorithm genAlg;

        private const string idDataBestBird = "bestBrainInGenerations";
        private Genome bestGenome = null;
        private int actualTurn = 0;
        private MapHandler map = null;
        private FoodHandler food = null;

        List<AgentBase> teamAIs = new List<AgentBase>();

        List<Genome> population = new List<Genome>();
        List<NeuralNetwork> brains = new List<NeuralNetwork>();

        bool isRunning = false;

        public int generation
        {
            get; private set;
        }

        public float bestFitness
        {
            get; private set;
        }

        public float actualPopulation
        {
            get; private set;
        }

        public float avgFitness
        {
            get; private set;
        }

        public float worstFitness
        {
            get; private set;
        }

        public Action onAllAIsDead = null;

        private float GetBestFitness()
        {
            float fitness = 0;
            foreach (Genome g in population)
            {
                if (fitness < g.fitness)
                {
                    fitness = g.fitness;
                    bestGenome = g;
                }
            }

            return fitness;
        }

        private float GetAvgFitness()
        {
            float fitness = 0;
            foreach (Genome g in population)
            {
                fitness += g.fitness;
            }

            return fitness / population.Count;
        }

        private float GetWorstFitness()
        {
            float fitness = float.MaxValue;
            foreach (Genome g in population)
            {
                if (fitness > g.fitness)
                    fitness = g.fitness;
            }

            return fitness;
        }

        public AgentBase GetBestAgent()
        {
            if (teamAIs.Count == 0)
            {
                Debug.Log("Population count is zero");
                return null;
            }

            AgentBase agent = teamAIs[0];
            Genome bestGenome = population[0];

            for (int i = 0; i < population.Count; i++)
            {
                if (i < teamAIs.Count)
                {
                    if (teamAIs[i].state == State.Alive && population[i].fitness > bestGenome.fitness)
                    {
                        bestGenome = population[i];
                        agent = teamAIs[i];
                    }
                }
            }

            return agent;
        }

        public List<AgentBase> SearchForAgentsThatCanCross()
        {
            List<AgentBase> agents = new List<AgentBase>();
            return agents;
        }

        public bool AllAgentsAlreadyThink()
        {
            List<AgentBase> agentsThatNotThink = teamAIs.Where(agent => agent.CurrentTurn != actualTurn).ToList();

            return agentsThatNotThink.Count < 1;
        }

        void Awake()
        {
            Load();
        }

        public void Load()
        {
            if (teamId == null)
                return;

            PopulationCount = PlayerPrefs.GetInt("PopulationCount_"+ teamId, 100);
            EliteCount = PlayerPrefs.GetInt("EliteCount_" + teamId, 0);
            MutationChance = PlayerPrefs.GetFloat("MutationChance_" + teamId, 5);
            MutationRate = PlayerPrefs.GetFloat("MutationRate_" + teamId, 3);
            InputsCount = PlayerPrefs.GetInt("InputsCount_" + teamId, 25);
            HiddenLayers = PlayerPrefs.GetInt("HiddenLayers_" + teamId, 2);
            OutputsCount = PlayerPrefs.GetInt("OutputsCount_" + teamId, 4);
            NeuronsCountPerHL = PlayerPrefs.GetInt("NeuronsCountPerHL_" + teamId, 14);
            Bias = PlayerPrefs.GetFloat("Bias_" + teamId, -2);
            Sigmoid = PlayerPrefs.GetFloat("P_" + teamId, 0.27f);
        }

        void Save()
        {
            if (teamId == null)
                return;

            PlayerPrefs.SetInt("PopulationCount_" + teamId, PopulationCount);
            PlayerPrefs.SetInt("EliteCount_" + teamId, EliteCount);
            PlayerPrefs.SetFloat("MutationChance_" + teamId, MutationChance);
            PlayerPrefs.SetFloat("MutationRate_" + teamId, MutationRate);
            PlayerPrefs.SetInt("InputsCount_" + teamId, InputsCount);
            PlayerPrefs.SetInt("HiddenLayers_" + teamId, HiddenLayers);
            PlayerPrefs.SetInt("OutputsCount_" + teamId, OutputsCount);
            PlayerPrefs.SetInt("NeuronsCountPerHL_" + teamId, NeuronsCountPerHL);
            PlayerPrefs.SetFloat("Bias_" + teamId, Bias);
            PlayerPrefs.SetFloat("P_" + teamId, Sigmoid);
        }

        public void StartSimulation(List<Vector2Int> initialPositions, MapHandler map, FoodHandler food, AgentData loadedAgentData)
        {
            this.map = map;
            this.food = food;

            Save();
            // Create and confiugre the Genetic Algorithm
            genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

            GenerateInitialPopulation(initialPositions, loadedAgentData);

            isRunning = true;
        }

        public void EndedGeneration()
        {
            Epoch();
        }

        public void PauseSimulation()
        {
            isRunning = !isRunning;

            Debug.Log(!isRunning? "Paused simulation" : "Simulation resumed");
        }

        public void StopSimulation()
        {
            Save();

            isRunning = false;

            generation = 0;

            DestroyBadAgents();
        }

        public void UpdateTurn(int currentTurn)
        {
            actualTurn = currentTurn;
        }

        // Generate the random initial population
        void GenerateInitialPopulation(List<Vector2Int> positions, AgentData loadedAgentData = null)
        {
            generation = 0;
            DestroyBadAgents();

            for (int i = 0; i < PopulationCount; i++)
            {
                NeuralNetwork brain = null;
                Genome genome = null;

                if (loadedAgentData != null)
                {
                    brain = CreateBrain(loadedAgentData.brain);
                    genome = loadedAgentData.genome;                    
                }
                else
                {
                    brain = CreateBrain();
                    genome = new Genome(brain.GetTotalWeightsCount());
                    brain.SetWeights(genome.genome);
                }

                brains.Add(brain);

                population.Add(genome);

                AgentBase generatedAgent = CreateAgent(positions[i], genome, brain);
                teamAIs.Add(generatedAgent);
            }

            bestFitness = GetBestFitness();
            avgFitness = GetAvgFitness();
            worstFitness = GetWorstFitness();
        }

        // Creates a new NeuralNetwork
        NeuralNetwork CreateBrain()
        {
            NeuralNetwork brain = new NeuralNetwork(NeuronsCountPerHL, OutputsCount);

            // Add first neuron layer that has as many neurons as inputs
            brain.AddFirstNeuronLayer(InputsCount, Bias, Sigmoid);

            for (int i = 0; i < HiddenLayers; i++)
            {
                // Add each hidden layer with custom neurons count
                brain.AddNeuronLayer(NeuronsCountPerHL, Bias, Sigmoid);
            }

            // Add the output layer with as many neurons as outputs
            brain.AddNeuronLayer(OutputsCount, Bias, Sigmoid);

            return brain;
        }

        NeuralNetwork CreateBrain(NeuralNetwork loadedBrain)
        {
            NeuralNetwork brain = new NeuralNetwork(loadedBrain.neuronCountsPerHL, loadedBrain.outputsCount);

            if (loadedBrain.layers.Count < 1)
                return null;

            Bias = loadedBrain.layers[0].bias;
            Sigmoid = loadedBrain.layers[0].p;
            NeuronsCountPerHL = loadedBrain.neuronCountsPerHL;

            // Add first neuron layer that has as many neurons as inputs
            brain.AddFirstNeuronLayer(loadedBrain.inputsCount, Bias, Sigmoid);

            for (int i = 0; i < loadedBrain.layers.Count; i++)
            {
                // Add each hidden layer with custom neurons count
                brain.AddNeuronLayer(NeuronsCountPerHL, loadedBrain.layers[i].bias, loadedBrain.layers[i].p);
            }

            // Add the output layer with as many neurons as outputs
            brain.AddNeuronLayer(loadedBrain.outputsCount, Bias, Sigmoid);

            return brain;
        }

        void Epoch() //Evolve
        {
            // Increment generation counter
            generation++;

            // Calculate best, average and worst fitness
            bestFitness = GetBestFitness();
            avgFitness = GetAvgFitness();
            worstFitness = GetWorstFitness();

            List<Genome> genomesThatSurvived = new List<Genome>();
            for (int i = 0; i < teamAIs.Count; i++)
            {
                if (teamAIs[i] != null)
                {
                    teamAIs[i].OnGenerationEnded(out Genome genomeSurvived);

                    if(genomeSurvived != null)
                    {
                        genomeSurvived.generationsSurvived++;

                        if(genomeSurvived.generationsSurvived < 4)
                        {
                            genomesThatSurvived.Add(genomeSurvived);
                        }
                    }
                }
            }

            // Evolve each genome and create a new array of genomes
            List<Genome> newGenomes = genAlg.Epoch(genomesThatSurvived.ToArray(), PopulationCount).ToList();

            // Clear current population
            population.Clear();

            // Add new population
            for (int i = 0; i < newGenomes.Count; i++)
            {
                if(i < PopulationCount)
                {
                    population.Add(newGenomes[i]);
                }
            }

            if(population.Count < PopulationCount)
            {
                int difference = PopulationCount - population.Count;
                DestroyUselessAgents(difference);
            }

            actualPopulation = population.Count;

            // Set the new genomes as each NeuralNetwork weights 
            for (int i = 0; i < population.Count; i++)
            {
                NeuralNetwork brain = brains[i];
                brain.SetWeights(newGenomes[i].genome);
                teamAIs[i].SetBrain(newGenomes[i], brain);
            }
        }

        // Update is called once per frame
        public void UpdatePopulation()
        {
            if (!isRunning)
                return;

            float dt = Time.fixedDeltaTime;

            for (int i = 0; i < Mathf.Clamp((float)IterationCount, 1, 100); i++)
            {
                foreach (AgentBase agent in teamAIs)
                {
                    // Think!! 
                    if(agent.state == State.Alive)
                    {
                        agent.Think(dt, actualTurn, IterationCount, map, food);
                    }
                }
            }
        }

        #region Helpers
        AgentBase CreateAgent(Vector2Int position,Genome genome, NeuralNetwork brain)
        {
            Vector3 finalPosition = new Vector3(position.x, position.y, 0f);
            GameObject go = Instantiate<GameObject>(AgentPrefab, finalPosition, Quaternion.identity);
            AgentBase b = go.GetComponent<AgentBase>();
            b.SetBrain(genome, brain, false);
            b.SetInitialiPosition(finalPosition);
            return b;
        }

        void DestroyBadAgents()
        {
            foreach (AgentBase go in teamAIs)
                Destroy(go.gameObject);

            teamAIs.Clear();
            population.Clear();
            brains.Clear();
        }

        void DestroyUselessAgents(int amount)
        {
            List<AgentBase> toDestroyAgents = new List<AgentBase>();

            int originalAmountAIs = teamAIs.Count;

            for (int i = 0; i < amount; i++)
            {
                if(amount < originalAmountAIs)
                {
                    toDestroyAgents.Add(teamAIs[i]);
                }
                else
                {
                    toDestroyAgents.AddRange(teamAIs);
                }
            }

            for (int i = 0; i < toDestroyAgents.Count; i++)
            {
                if (toDestroyAgents[i] != null)
                {
                    teamAIs.Remove(toDestroyAgents[i]);
                    Destroy(toDestroyAgents[i].gameObject);
                }
            }

            if(teamAIs.Count < 1)
            {
                onAllAIsDead?.Invoke();
                population.Clear();
            }
        }
        #endregion

    }
}