using System;

namespace Genetics
{
    public class Individuum<T> : ICloneable, IComparable<Individuum<T>>
    {
        public delegate T[] DataDeepCopy(T[] Data);

        public double Fitness;
        public T[] Data;
        public DataDeepCopy DeepCopyCreator;
        
        public Individuum(T[] data)
        {
            Data = data;
        }

        public Individuum(DataDeepCopy deepCopyCreator)
        {
            DeepCopyCreator = deepCopyCreator;
        }

        public Individuum(DataDeepCopy deepCopyCreator, T[] data)
        {
            Data = data;
            DeepCopyCreator = deepCopyCreator;
        }

        public int CompareTo(Individuum<T> other)
        {
            if (Fitness > other.Fitness)
                return 1;
            else if (Fitness < other.Fitness)
                return -1;
            else
                return 0;
        }

        public object Clone()
        {
            Individuum<T> Cloned = new Individuum<T>(DeepCopyCreator, Data);
            Cloned.Data = DeepCopyCreator(Data);
            Cloned.Fitness = Fitness;
            return Cloned;
        }
    }
}
