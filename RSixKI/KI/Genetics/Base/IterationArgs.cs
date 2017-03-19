using System;

namespace Genetics
{
    public class IterationArgs<T> : EventArgs
    {
        public Individuum<T> Best;
        public int Iteration;
        public IterationArgs(Individuum<T> best, int iteration)
        {
            Best = best;
            Iteration = iteration;
        }
    }
}
