using NeuralNetClassifier.Enums;
using NeuralNetClassifier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetClassifier.Test
{
    public static class Fakes
    {

        public static List<NetNode> GenerateLayer(int layerCount, NodeType nodeType, string calculatorName, List<string> valuesList)
        {
            var layer = Fakes.NodeBuilder.Generate(layerCount, nodeType, calculatorName, valuesList);
            return layer;
        }

        public static List<NetConnection> FullConnectLayers(List<NetNode> inLayer, List<NetNode> outLayer, string calculatorName, List<string> valuesList)
        {
            List<NetConnection> connections = new List<NetConnection>();

            foreach(var inNode in inLayer)
            {
                foreach(var outNode in outLayer)
                {
                    var connection = Fakes.ConnectionBuilder.Generate(inNode.ID, outNode.ID, calculatorName, valuesList);
                    connections.Add(connection);
                }
            }

            return connections;
        }

        public static class ValuesListBuilder
        {
            public static Dictionary<string, double> Generate(List<string> valuesList)
            {
                Bogus.Faker<Dictionary<string, double>> faker = new Bogus.Faker<Dictionary<string, double>>()
                    .Rules((f, dict) =>
                    {
                        foreach(var item in valuesList)
                        {
                            dict.Add(item, f.Random.Double(-1, 1));
                        }
                    });

                return faker.Generate();

                    
            }
        }

        public static class NodeBuilder
        {
            public static NetNode Generate(NodeType nodeType, string calculatorName, List<string> valuesList)
            {
                return Generate(1, nodeType, calculatorName, valuesList)[0];
            }

            public static List<NetNode> Generate(int count, NodeType nodeType, string calculatorName, List<string> valuesList)
            {
                Bogus.Faker<NetNode> faker = new Bogus.Faker<NetNode>()
                    .RuleFor(x => x.Name, f => f.Lorem.Word())
                    .RuleFor(x => x.ID, f => Guid.NewGuid())
                    .RuleFor(x => x.NodeType, f => nodeType)
                    .RuleFor(x => x.CalculatorTypeName, f => calculatorName)
                    .RuleFor(x => x.CalculatorValues, f => Fakes.ValuesListBuilder.Generate(valuesList))
                    ;

                return faker.Generate(count);
            }
        }


        public static class ConnectionBuilder
        {


            public static NetConnection Generate(Guid inNode, Guid outNode, string calculatorName, List<string> valuesList)
            {
                Bogus.Faker<NetConnection> faker = new Bogus.Faker<NetConnection>()
                    .RuleFor(x => x.ID, f => Guid.NewGuid())
                    .RuleFor(x => x.InboundNodeID, inNode)
                    .RuleFor(x => x.OutboundNodeId, outNode)
                    .RuleFor(x => x.CalculatorTypeName, f => calculatorName)
                    .RuleFor(x => x.CalculatorValues, f => Fakes.ValuesListBuilder.Generate(valuesList))
                    ;

                return faker.Generate();
            }
        }
    }
}
