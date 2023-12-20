using NeuralNetClassifier.Workspace;

namespace NeuralNetClassifier.Interface
{
    public interface IConnectionCalculate
    {
        Task<double> Calculate(NetNode node);
        Task<double> Train(NetNode inboundNode, NetNode outboundNode);

        void SetValues(Dictionary<string, double> values);
        Dictionary<string, double> GetValues();
    }

}
