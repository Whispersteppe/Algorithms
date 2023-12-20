using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetClassifier.Workspace;

namespace NeuralNetClassifier.Interface
{
    public interface INodeCalculate
    {
        Task<double> Calculate(List<NetConnection> inboundConnections);
        Task<double> Train(NetNode inboundNode, List<NetConnection> outboundConnections);
        void SetValues(Dictionary<string, double> values);
        Dictionary<string, double> GetValues();
    }

}
