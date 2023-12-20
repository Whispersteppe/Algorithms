namespace NeuralNetClassifier.Models
{
    public class NetConnection
    {
        public Guid ID { get; set; }
        public Guid InboundNodeID { get; set; }
        public Guid OutboundNodeId { get; set; }
        public string CalculatorTypeName { get; set; }
        public Dictionary<string, double> CalculatorValues { get; set; }
    }
}
