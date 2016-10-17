using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.NeuronalNetwork
{
    class NeuronalNetwork
    {
        private List<InputNeuron> inputNeurons = new List<InputNeuron>();
        private List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        private List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();


        public void AddInputNeuron(InputNeuron neuron)
        {
            inputNeurons.Add(neuron);
        }
        public void AddHiddenNeuron(WorkingNeuron neuron)
        {
            hiddenNeurons.Add(neuron);
        }
        public void AddOutputNeuron(WorkingNeuron neuron)
        {
            outputNeurons.Add(neuron);
        }

        public void GenerateHiddenNeurons(int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                hiddenNeurons.Add(new WorkingNeuron());
            }
        }

        public void GenerateFullMesh()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                foreach (InputNeuron input in inputNeurons)
                {
                    wn.AddNeuronConnection(input, 1);
                }
            }

            foreach (WorkingNeuron wn in outputNeurons)
            {
                foreach (WorkingNeuron wn2 in hiddenNeurons)
                {
                    wn.AddNeuronConnection(wn2, 1);
                }
            }
        }

        public NeuronalNetwork clone()
        {
            //TODO
            return null;
        }
    }
}
