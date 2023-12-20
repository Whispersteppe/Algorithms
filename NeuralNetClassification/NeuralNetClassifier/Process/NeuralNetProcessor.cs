using NeuralNetClassifier.Interface;
using NeuralNetClassifier.Models;
using NeuralNetClassifier.Workspace;
using NeuralNetClassifier.Workspace.Calculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetClassifier.Process
{
    public class NeuralNetProcessor
    {

        ICalculatorFactory _calculatorFactory;

        Workspace.NeuralNet network;

        public Workspace.NeuralNet Network { get { return network; } }

        public NeuralNetProcessor(List<string> ignoreAssemblies = null)
        {
            _calculatorFactory = new CalculatorFactory(null, null, ignoreAssemblies);

            network = new Workspace.NeuralNet(_calculatorFactory);

        }

        public void Load(Models.NeuralNet model)
        {
            network.LoadModel(model);

        }

        public Models.NeuralNet Save()
        {
            var model = network.GetModel();

            return model;


        }

        public async Task Train(ValueSet inboundValues, ValueSet expectedValues)
        {

            await network.Train(inboundValues, expectedValues);

        }

        public async Task<ValueSet> Calculate(ValueSet inboundValues)
        {
            return await network.Calculate(inboundValues);
        }


    }
}
