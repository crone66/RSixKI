using Microsoft.Xna.Framework;
using RSixKI;

namespace Genetics
{
    public class BalancingGeneticAlgorithm : GeneticAlgorithm<Decision>
    {
        private Weapon[] primary;
        private Weapon[] secondary;
        private Vector2[] entries;
        private GameScene scene;

        public BalancingGeneticAlgorithm(Weapon[] primary, Weapon[] secondary, Vector2[] entries, GameScene scene, int populationSize, double mutationChance, double crossoverChance, double elitistChance, double surviveChance, int childCount, int iterations) : base(populationSize, mutationChance, crossoverChance, elitistChance, surviveChance, childCount, iterations)
        {
            this.primary = primary;
            this.secondary = secondary;
            this.entries = entries;
            this.scene = scene;
            Initzialize();
        }

        public BalancingGeneticAlgorithm(Weapon[] primary, Weapon[] secondary, Vector2[] entries, GameScene scene, int populationSize, double mutationChance, double crossoverChance, double elitistChance, double surviveChance, int childCount, int iterations, Individuum<Decision>[] dna) : base(populationSize, mutationChance, crossoverChance, elitistChance, surviveChance, childCount, iterations, dna)
        {
            this.primary = primary;
            this.secondary = secondary;
            this.entries = entries;
            this.scene = scene;
            Initzialize();
        }

        /// <summary>
        /// Runs a threaded game session (Fitness = health)
        /// </summary>
        /// <param name="dna"></param>
        /// <returns>Returns fitness</returns>
        protected override int CalculateFitness(Individuum<Decision> dna)
        {
            return scene.DoTraining(dna.Data);
        }

        /// <summary>
        /// Create a individuum and Setup decision data
        /// </summary>
        /// <returns></returns>
        protected override Individuum<Decision> CreateGen()
        {
            Decision[] decisions = new Decision[6];

            decisions[0] = new ChoiceDecision(primary.Length, rand); //primary weapon
            decisions[1] = new ChoiceDecision(primary[decisions[0].Value].SuspressAble ? 2 : 1, rand); // primary suspressor
            decisions[2] = new ChoiceDecision(secondary.Length, rand); //secondary weapon
            decisions[3] = new ChoiceDecision(secondary[decisions[2].Value].SuspressAble ? 2 : 1, rand); //secondary suspressor
            decisions[4] = new ChoiceDecision(entries.Length, rand); //entry
            decisions[5] = new ChoiceDecision(entries.Length, rand); //exit
            
            Individuum <Decision> dna = new Individuum<Decision>(DeepCopy, decisions);
            return dna;
        }

        /// <summary>
        /// Creates a new individuum with crossed decision data from parent1 and parent2
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns>retruns a new individuum with crossed decision data from parent1 and parent2</returns>
        protected override Individuum<Decision> Crossover(Individuum<Decision> parent1, Individuum<Decision> parent2)
        {
            Individuum<Decision> child = (Individuum<Decision>)parent1.Clone();
            int crossoverIndex = rand.Next(child.Data.Length);

            for (int i = 0; i < crossoverIndex; i++)
            {
                child.Data[i] = parent2.Data[i];
            }

            return child;
        }

        protected override void Mutation(Individuum<Decision> dna)
        {
            for (int j = 0; j < dna.Data.Length; j++)
            {
                if (rand.NextDouble() <= mutationChance)
                {
                    dna.Data[j].Init(); //re-initzialize a choice
                }
            }
        }

        /// <summary>
        /// Deep copy of individuums decision array
        /// </summary>
        /// <param name="data">decisions array (data)</param>
        /// <returns>returns a copy of decisions</returns>
        private Decision[] DeepCopy(Decision[] data)
        {
            Decision[] arr = new Decision[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                arr[i] = data[i].Clone() as Decision;
            }
            return arr;
        }
    }
}
