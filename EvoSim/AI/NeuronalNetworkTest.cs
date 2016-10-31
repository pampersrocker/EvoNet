using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    class NeuronalNetworkTest
    {
        public static void Test()
        {
            Console.WriteLine("Begin NN Test!");

            NeuronalNetwork nn = new NeuronalNetwork();
            InputNeuron in1 = new InputNeuron();
            InputNeuron in2 = new InputNeuron();
            InputNeuron in3 = new InputNeuron();

            WorkingNeuron out1 = new WorkingNeuron();
            WorkingNeuron out2 = new WorkingNeuron();
            WorkingNeuron out3 = new WorkingNeuron();

            nn.AddInputNeuron(in1);
            nn.AddInputNeuron(in2);
            nn.AddInputNeuron(in3);

            nn.GenerateHiddenNeurons(3);

            nn.AddOutputNeuron(out1);
            nn.AddOutputNeuron(out2);
            nn.AddOutputNeuron(out3);

            nn.GenerateFullMesh();

            nn.RandomizeAllWeights();


            NeuronalNetwork nn2 = nn.CloneFullMesh();

            for (int i = 0; i < 3; i++)
            {
                Debug.Assert(nn2.GetOutputNeuronFromIndex(i).GetValue() == nn.GetOutputNeuronFromIndex(i).GetValue());
            }

            Console.WriteLine("NN Test success! <(^.^)>");
        }
    }
}
