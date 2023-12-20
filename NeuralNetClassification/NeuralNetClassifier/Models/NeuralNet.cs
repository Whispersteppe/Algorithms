using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetClassifier.Models
{
    public class NeuralNet
    {

        public List<NetNode> Nodes { get; set; }
        public List<NetConnection> Connections { get; set; }
        public List<ExternalCalculatorType> ExternalCalculatorTypes { get; set; }
    }
}
