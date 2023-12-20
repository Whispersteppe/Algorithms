using NeuralNetClassifier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetClassifier.Interface
{
    public interface ICalculatorFactory
    {
        INodeCalculate GetNodeCalculator(string typeName, Dictionary<string, double> values);
        IConnectionCalculate GetConnectionCalculator(string typeName, Dictionary<string, double> values);
        List<Type> NodeCalculatorTypes { get; }
        List<Type> ConnectionCalculatorTypes { get; }

        bool AddCalculatorType(Type type);
        bool AddCalculatorTypes(Assembly assembly);

        bool AddCalculatorType(ExternalCalculatorType type);



    }
}
