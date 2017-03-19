
using System;

namespace Genetics
{
    public abstract class GeneticAlgorithm<T>
    {
        protected static Random rand = new Random();

        protected Individuum<T>[] dna;
        protected Individuum<T> best;
        protected int populationSize;
        protected double crossoverChance;
        protected double mutationChance;
        protected double elitistChance;
        protected double surviveChance;
        protected int iterations;
        protected int childCount;

        public event EventHandler<IterationArgs<T>> IterationCompleted;
        public event EventHandler IterationStarted;

        public Individuum<T>[] DNA
        {
            get
            {
                return dna;
            }
        }

        public Individuum<T> Best
        {
            get
            {
                return best;
            }
        }

        public GeneticAlgorithm(int populationSize, double mutationChance, double crossoverChance, double elitistChance, double surviveChance, int childCount, int iterations)
        {
            this.populationSize = populationSize;
            this.mutationChance = mutationChance;
            this.crossoverChance = crossoverChance;
            this.elitistChance = elitistChance;
            this.surviveChance = surviveChance;
            this.childCount = childCount;
            this.iterations = iterations;
        }

        public GeneticAlgorithm(int populationSize, double mutationChance, double crossoverChance, double elitistChance, double surviveChance, int childCount, int iterations, Individuum<T>[] dna)
        {
            this.dna = dna;
            this.populationSize = populationSize;
            this.mutationChance = mutationChance;
            this.crossoverChance = crossoverChance;
            this.elitistChance = elitistChance;
            this.surviveChance = surviveChance;
            this.childCount = childCount;
            this.iterations = iterations;
        }        

        protected virtual void Initzialize()
        {
            Individuum<T>[] newDna = new Individuum<T>[populationSize];
            if(dna != null)
                ChooseSurvivors(ref newDna);

            dna = newDna;

            for (int i = 0; i < dna.Length; i++)
            {
                if (dna[i] == null)
                    dna[i] = CreateGen();
            }
        }

        protected virtual void ChooseSurvivors(ref Individuum<T>[] newDna)
        {
            for (int i = 0; i < Math.Ceiling(dna.Length * elitistChance) && i < newDna.Length; i++)
            {
                newDna[i] = (Individuum<T>)dna[i].Clone();
            }
        }

        protected virtual void CreateDNA()
        {
            dna = new Individuum<T>[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                dna[i] = CreateGen();
            }
        }

        protected virtual double CalculateSummedFitness()
        {
            double SumFitness = 0;
            for (int i = 0; i < populationSize; i++)
            {
                dna[i].Fitness = CalculateFitness(dna[i]);
            }
            Array.Sort(dna);

            for (int i = 0; i < populationSize; i++)
            {
                if (best == null || dna[i].Fitness > best.Fitness)
                {
                    best = dna[i];
                }
                SumFitness += dna[i].Fitness;
            }

            return SumFitness;
        }

        protected virtual void ChooseParents(double summedFitness, ref Individuum<T> parent1, ref Individuum<T> parent2)
        {
            do
            {
                for (int j = 0; j < populationSize; j++)
                {
                    if (parent1 != null && parent2 != null)
                        return;

                    if (summedFitness <= 0)
                    {
                        int value = rand.Next(populationSize);
                        parent1 = dna[value];
                        if (value + 1 < populationSize)
                            parent2 = dna[value + 1];
                        else if (value - 1 > 0)
                            parent2 = dna[value - 1];
                        else
                            parent2 = parent1;
                    }
                    else
                    {
                        if (parent1 == null && parent2 != dna[j] && 1 - (dna[j].Fitness / summedFitness) >= rand.NextDouble())
                            parent1 = dna[j];

                        if (parent2 == null && parent1 != dna[j] && 1 - (dna[j].Fitness / summedFitness) >= rand.NextDouble())
                            parent2 = dna[j];
                    }
                }
            } while (parent1 == null || parent2 == null);
        }

        protected virtual Individuum<T>[] GetElitist()
        {
            Individuum<T>[] newDna = new Individuum<T>[populationSize];
            int count = (int)Math.Ceiling(populationSize * elitistChance); //number of elitiest that survives
            for (int i = 0; i < count; i++)
            {
                newDna[i] = (Individuum<T>)dna[i].Clone();
            }

            if (newDna[0] == null)
                newDna[0] = (Individuum<T>)dna[0].Clone();

            return newDna;
        }

        /// <summary>
        /// Starts calculations
        /// </summary>
        /// <returns>Returns best indviduum</returns>
        public virtual Individuum<T> Do()
        {
            for (int i = 0; i < iterations; i++)
            {
                IterationStarted?.Invoke(this, EventArgs.Empty);
                double SummedFitness = CalculateSummedFitness();
                Individuum<T>[] NewPopulation = GetElitist();

                for (int j = (int)Math.Ceiling(populationSize * elitistChance); j < populationSize; j++)
                {
                    bool born = false;
                    if (rand.NextDouble() <= crossoverChance)
                    {
                        Individuum<T> parent1 = null;
                        Individuum<T> parent2 = null;
                        ChooseParents(SummedFitness, ref parent1, ref parent2);

                        Individuum<T> child = Crossover(parent1, parent2);
                        child.Fitness = CalculateFitness(child);
                        if (child.Fitness != parent1.Fitness && child.Fitness != parent2.Fitness)
                        {
                            NewPopulation[j] = child;
                            born = true;
                        }
                    }

                    if(!born)
                    {
                        if (surviveChance >= rand.NextDouble())
                        {
                            Mutation(dna[j]);
                            NewPopulation[j] = dna[j];
                        }
                        else
                        {
                            NewPopulation[j] = CreateGen();
                        }
                    }
                }
                dna = NewPopulation;

                IterationCompleted?.Invoke(this, new IterationArgs<T>(Best, i));
            }
            return Best;
        }

        protected virtual void Mutation(Individuum<T> dna)
        {
            for (int j = 0; j < dna.Data.Length; j++)
            {
                if (rand.NextDouble() <= mutationChance)
                {
                    int swapIndex = rand.Next(dna.Data.Length);
                    T swapValue = dna.Data[j];
                    dna.Data[j] = dna.Data[swapIndex];
                    dna.Data[swapIndex] = swapValue;
                }
            }
        }

        protected abstract Individuum<T> CreateGen();
        
        protected abstract int CalculateFitness(Individuum<T> dna);

        protected abstract Individuum<T> Crossover(Individuum<T> dna1, Individuum<T> dna2);
    }
}
