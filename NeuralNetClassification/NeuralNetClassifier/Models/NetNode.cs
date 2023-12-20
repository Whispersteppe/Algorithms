using NeuralNetClassifier.Enums;

namespace NeuralNetClassifier.Models
{
    public class NetNode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public string CalculatorTypeName { get; set; }
        public Dictionary<string, double> CalculatorValues { get; set; }


    }
}
