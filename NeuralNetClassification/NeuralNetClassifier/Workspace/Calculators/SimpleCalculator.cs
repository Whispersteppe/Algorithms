using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetClassifier.Interface;

namespace NeuralNetClassifier.Workspace.Calculators
{
    public class SimpleCalculator : IConnectionCalculate
    {
        public double Weight { get; set; }
        public double WeightChangeFactor { get; set; }

        public void SetValues(Dictionary<string, double> values)
        {
            Weight = values["Weight"];
            WeightChangeFactor = values["WeightChangeFactor"];
        }
        public Dictionary<string, double> GetValues()
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("Weight", Weight);
            values.Add("WeightChangeFactor", WeightChangeFactor);

            return values;

        }

        public async Task<double> Calculate(NetNode node)
        {
            double value = node.CurrentValue.Value * Weight;
            return value;
        }

        public async Task<double> Train(NetNode inboundNode, NetNode outboundNode)
        {
            double currentResult = await Calculate(inboundNode);

            double newWeight = 
                Weight + 
                ((outboundNode.BackPropagatedValue.Value - currentResult * Weight) / outboundNode.BackPropagatedValue.Value) 
                * WeightChangeFactor
                * Weight;

            Weight = newWeight;
            return newWeight;
        }
    }

    public class SimpleNodeCalculator : INodeCalculate
    {
        public double Weight { get; set; }
        public double WeightChangeFactor { get; set; }

        public void SetValues(Dictionary<string, double> values)
        {
            Weight = values["Weight"];
            WeightChangeFactor = values["WeightChangeFactor"];
        }
        public Dictionary<string, double> GetValues()
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            values.Add("Weight", Weight);
            values.Add("WeightChangeFactor", WeightChangeFactor);

            return values;

        }
        public async Task<double> Calculate(List<NetConnection> inboundConnections)
        {
            double result = inboundConnections.Average(x=>x.CurrentValue.Value) * Weight;
            return result;
        }

        public async Task<double> Train(NetNode inboundNode, List<NetConnection> outboundConnections)
        {
            //  get the unweighted average of the connections
            double averageWeightDifference = outboundConnections.Average(x=> (x.BackPropagatedValue.Value - x.CurrentValue.Value) / x.BackPropagatedValue.Value);

            double newWeight =
                Weight +
                averageWeightDifference * WeightChangeFactor * Weight;

            Weight = newWeight;
            return newWeight * inboundNode.CurrentValue.Value;

        }
    }
}
