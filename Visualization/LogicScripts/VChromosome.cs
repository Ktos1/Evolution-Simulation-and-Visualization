using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Visualization
{
    // być może ta klasa nie jest potrzebna, może dałoby się trzymać geny po prostu w tablicy, zamiast
    // oddzielnej klasy całej. Usuń ją, jak nie znajdzie się dla niej zastosowania.
    public class VChromosome : Chromosome<VGene>
    {
        public VChromosome(float[] genesValues) : base()
        {
            for (int i = 0; i < _genes.Length; i++)
            {
                _genes[i].Value = genesValues[i];
            }
        }
    }
}
