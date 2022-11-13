using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using InteligenciaArtificial.SegundoParcial.Agents;

namespace InteligenciaArtificial.SegundoParcial.Handlers
{
    public class PopulationManager : MonoBehaviour
    {
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

        List<AgentBase> firstTeamAIs = new List<AgentBase>();
        List<AgentBase> secondTeamAIs = new List<AgentBase>();

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

        public float avgFitness
        {
            get; private set;
        }

        public float worstFitness
        {
            get; private set;
        }

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
            if (firstTeamAIs.Count == 0)
            {
                Debug.Log("Population count is zero");
                return null;
            }

            AgentBase bird = firstTeamAIs[0];
            Genome bestGenome = population[0];
            for (int i = 0; i < population.Count; i++)
            {
                if (firstTeamAIs[i].state == State.Alive && population[i].fitness > bestGenome.fitness)
                {
                    bestGenome = population[i];
                    bird = firstTeamAIs[i];
                }
            }

            return bird;
        }

        static PopulationManager instance = null;

        public static PopulationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<PopulationManager>();

                return instance;
            }
        }

        void Awake()
        {
            instance = this;
            Load();
        }

        public void Load()
        {
            PopulationCount = PlayerPrefs.GetInt("PopulationCount", 2);
            EliteCount = PlayerPrefs.GetInt("EliteCount", 0);
            MutationChance = PlayerPrefs.GetFloat("MutationChance", 0);
            MutationRate = PlayerPrefs.GetFloat("MutationRate", 0);
            InputsCount = PlayerPrefs.GetInt("InputsCount", 1);
            HiddenLayers = PlayerPrefs.GetInt("HiddenLayers", 5);
            OutputsCount = PlayerPrefs.GetInt("OutputsCount", 1);
            NeuronsCountPerHL = PlayerPrefs.GetInt("NeuronsCountPerHL", 1);
            Bias = PlayerPrefs.GetFloat("Bias", 0);
            Sigmoid = PlayerPrefs.GetFloat("P", 1);
        }

        void Save()
        {
            PlayerPrefs.SetInt("PopulationCount", PopulationCount);
            PlayerPrefs.SetInt("EliteCount", EliteCount);
            PlayerPrefs.SetFloat("MutationChance", MutationChance);
            PlayerPrefs.SetFloat("MutationRate", MutationRate);
            PlayerPrefs.SetInt("InputsCount", InputsCount);
            PlayerPrefs.SetInt("HiddenLayers", HiddenLayers);
            PlayerPrefs.SetInt("OutputsCount", OutputsCount);
            PlayerPrefs.SetInt("NeuronsCountPerHL", NeuronsCountPerHL);
            PlayerPrefs.SetFloat("Bias", Bias);
            PlayerPrefs.SetFloat("P", Sigmoid);
        }

        public void StartSimulation(bool bestAI = false)
        {
            Save();
            // Create and confiugre the Genetic Algorithm
            genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

            GenerateInitialPopulation();

            isRunning = true;
        }

        public void PauseSimulation()
        {
            isRunning = !isRunning;
        }

        public void StopSimulation()
        {
            Save();

            isRunning = false;

            generation = 0;

            DestroyBadAgents();
        }

        // Generate the random initial population
        void GenerateInitialPopulation()
        {
            generation = 0;
            DestroyBadAgents();

            for (int i = 0; i < PopulationCount; i++)
            {
                NeuralNetwork brain = CreateBrain();

                Genome genome = new Genome(brain.GetTotalWeightsCount());

                brain.SetWeights(genome.genome);
                brains.Add(brain);

                population.Add(genome);
                firstTeamAIs.Add(CreateAgent(genome, brain));
            }
        }

        // Creates a new NeuralNetwork
        NeuralNetwork CreateBrain()
        {
            NeuralNetwork brain = new NeuralNetwork();

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

        void Epoch() //Evolve
        {
            // Increment generation counter
            generation++;

            // Calculate best, average and worst fitness
            bestFitness = GetBestFitness();
            avgFitness = GetAvgFitness();
            worstFitness = GetWorstFitness();

            // Evolve each genome and create a new array of genomes
            Genome[] newGenomes = genAlg.Epoch(population.ToArray());

            // Clear current population
            population.Clear();

            // Add new population
            population.AddRange(newGenomes);

            // Set the new genomes as each NeuralNetwork weights
            for (int i = 0; i < PopulationCount; i++)
            {
                NeuralNetwork brain = brains[i];
                brain.SetWeights(newGenomes[i].genome);
                firstTeamAIs[i].SetBrain(newGenomes[i], brain);
            }

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isRunning)
                return;

            float dt = Time.fixedDeltaTime;

            for (int i = 0; i < Mathf.Clamp((float)IterationCount, 1, 100); i++)
            {
                bool areAllDead = true;

                foreach (AgentBase b in firstTeamAIs)
                {
                    // Think!! 
                    b.Think(dt);
                    if (b.state == State.Alive)
                        areAllDead = false;
                }

                // Check the time to evolve
                if (areAllDead)
                {
                    Epoch();
                    break;
                }
            }
        }

        #region Helpers
        AgentBase CreateAgent(Genome genome, NeuralNetwork brain)
        {
            Vector3 position = Vector3.zero;
            GameObject go = Instantiate<GameObject>(AgentPrefab, position, Quaternion.identity);
            AgentBase b = go.GetComponent<AgentBase>();
            b.SetBrain(genome, brain);
            return b;
        }

        void DestroyBadAgents()
        {
            foreach (AgentBase go in firstTeamAIs)
                Destroy(go.gameObject);

            firstTeamAIs.Clear();
            population.Clear();
            brains.Clear();
        }
        #endregion

    }
}