namespace ProjectEvolution.CommonStuff
{
    internal class Chromosome
    {
        protected float[] _genes;

        public float[] Genes => _genes;

        protected Chromosome()
        {
            _genes = new float[0];
        }

        public Chromosome(float[] genes)
        {
            if (genes != null)
            {
                _genes = genes;
            }
        }
    }
}