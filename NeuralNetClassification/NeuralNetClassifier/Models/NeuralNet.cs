namespace NeuralNetClassifier.Models
{
    public class NeuralNet
    {

        public List<NetNode> Nodes { get; set; }
        public List<NetConnection> Connections { get; set; }
        public List<ExternalCalculatorType> ExternalCalculatorTypes { get; set; }
    }
}
