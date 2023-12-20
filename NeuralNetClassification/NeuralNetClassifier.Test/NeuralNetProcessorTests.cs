using NeuralNetClassifier.Process;
using Whispersteppe.XUnit;
using Xunit.Abstractions;

namespace NeuralNetClassifier.Test
{
    [Collection("Context collection")]
    public class NeuralNetProcessorTests : XUnitTestClassBase
    {
        public NeuralNetProcessorTests(
            XUnitClassFixture fixture, 
            XUnitCollectionFixture collectionFixture, 
            ITestOutputHelper output) : 
            base(fixture, collectionFixture, output)
        {
        }

        [Fact]
        public void BasicCreate()
        {
            List<string> ignoreAssemblies = this.GetConfig<List<string>>("ignoreAssemblies");

            NeuralNetProcessor processor = new NeuralNetProcessor(ignoreAssemblies);

            var calculators = processor.Network.Calculators;

            WriteLine(calculators);


        }

        [Fact]
        public void SimpleNetwork()
        {
            List<string> ignoreAssemblies = this.GetConfig<List<string>>("ignoreAssemblies");

            NeuralNetProcessor processor = new NeuralNetProcessor(ignoreAssemblies);

            Models.NetNode nodeIn = new Models.NetNode()
            {
                ID = Guid.NewGuid(),
                Name = "input node",
                NodeType = Enums.NodeType.Input,
                CalculatorTypeName = "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator",
                CalculatorValues = new Dictionary<string, double>() 
                {
                    {"Weight", 0 },
                    {"WeightChangeFactor", .1 }
                }
            };

            Models.NetNode nodeInternal = new Models.NetNode()
            {
                ID = Guid.NewGuid(),
                Name = "internal node",
                NodeType = Enums.NodeType.Hidden,
                CalculatorTypeName = "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator",
                CalculatorValues = new Dictionary<string, double>()
                {
                    {"Weight", 0 },
                    {"WeightChangeFactor", .1 }
                }
            };

            Models.NetNode nodeOut = new Models.NetNode()
            {
                ID = Guid.NewGuid(),
                Name = "output node",
                NodeType = Enums.NodeType.Output,
                CalculatorTypeName = "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator",
                CalculatorValues = new Dictionary<string, double>()
                {
                    {"Weight", 0 },
                    {"WeightChangeFactor", .1 }
                }
            };

            processor.Network.AddNode(nodeIn);
            processor.Network.AddNode(nodeInternal);
            processor.Network.AddNode(nodeOut);

            Models.NetConnection connection1 = new Models.NetConnection()
            {
                ID = Guid.NewGuid(),
                InboundNodeID = nodeIn.ID,
                OutboundNodeId = nodeInternal.ID,
                CalculatorTypeName = "NeuralNetClassifier.Workspace.Calculators.SimpleCalculator",
                CalculatorValues = new Dictionary<string, double>()
                {
                    {"Weight", 0 },
                    {"WeightChangeFactor", .1 }
                }
            };

            Models.NetConnection connection2 = new Models.NetConnection()
            {
                ID = Guid.NewGuid(),
                InboundNodeID = nodeInternal.ID,
                OutboundNodeId = nodeOut.ID,
                CalculatorTypeName = "NeuralNetClassifier.Workspace.Calculators.SimpleCalculator",
                CalculatorValues = new Dictionary<string, double>()
                {
                    {"Weight", 0 },
                    {"WeightChangeFactor", .1 }
                }
            };

            processor.Network.AddConnection(connection1);
            processor.Network.AddConnection(connection2);

            var model = processor.Save();

            WriteLine(model);

        }

        [Fact]
        public async Task MultiNodeConnection()
        {
            List<string> ignoreAssemblies = this.GetConfig<List<string>>("ignoreAssemblies");

            NeuralNetProcessor processor = new NeuralNetProcessor(ignoreAssemblies);

            var calculatorValues = new List<string>()
                {
                    "Weight",
                    "WeightChangeFactor"
                };

            var inLayer = Fakes.GenerateLayer(10, Enums.NodeType.Input, "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator", calculatorValues);
            var middleLayer1 = Fakes.GenerateLayer(10, Enums.NodeType.Hidden, "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator", calculatorValues);
//            var middleLayer2 = Fakes.GenerateLayer(10, Enums.NodeType.Hidden, "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator", calculatorValues);
//            var middleLayer3 = Fakes.GenerateLayer(10, Enums.NodeType.Hidden, "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator", calculatorValues);
            var outLayer = Fakes.GenerateLayer(10, Enums.NodeType.Output, "NeuralNetClassifier.Workspace.Calculators.SimpleNodeCalculator", calculatorValues);

            var connector1 = Fakes.FullConnectLayers(inLayer, middleLayer1, "NeuralNetClassifier.Workspace.Calculators.SimpleCalculator", calculatorValues);
            var connector2 = Fakes.FullConnectLayers(middleLayer1, outLayer, "NeuralNetClassifier.Workspace.Calculators.SimpleCalculator", calculatorValues);

            processor.Network.AddNodes(inLayer);
            processor.Network.AddNodes(middleLayer1);
            processor.Network.AddNodes(outLayer);

            processor.Network.AddConnections(connector1);
            processor.Network.AddConnections(connector2);

            var model = processor.Save();

            WriteLine(model);

            NeuralNetProcessor processor2 = new NeuralNetProcessor(ignoreAssemblies);

            processor2.Load(model);


        }
    }
}