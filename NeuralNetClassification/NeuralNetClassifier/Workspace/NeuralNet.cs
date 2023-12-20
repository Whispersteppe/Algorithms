using NeuralNetClassifier.Interface;
using NeuralNetClassifier.Models;

namespace NeuralNetClassifier.Workspace
{
    public class NeuralNet
    {
        public Dictionary<Guid, NetNode> Nodes { get; set; }
        public Dictionary<string, NetNode> InputNodes { get; set; }
        public Dictionary<string, NetNode> OutputNodes { get; set; }
        public Dictionary<Guid, NetConnection> Connections { get; set; }

        ICalculatorFactory _calculatorFactory;



        #region constructor

        public NeuralNet(ICalculatorFactory factory)
        {
            Nodes = new Dictionary<Guid, NetNode>();
            InputNodes = new Dictionary<string, NetNode>();
            OutputNodes = new Dictionary<string, NetNode>();
            Connections = new Dictionary<Guid, NetConnection>();

            _calculatorFactory = factory;
        }

        #endregion

        public void ValidateNetwork()
        {
        }

        #region operational

        public async Task Train(ValueSet inboundValues, ValueSet expectedValues)
        {
            var result = await Calculate(inboundValues);
            foreach (var node in OutputNodes) {
                await node.Value.Train();
            }
        }

        public async Task<ValueSet> Calculate(ValueSet inboundValues)
        {
            await Reset();

            foreach (var node in InputNodes)
            {
                await node.Value.Calculate(inboundValues[node.Value.Name]);

            }

            //  should all be calculated by the time we get here

            ValueSet outputSet = new ValueSet();

            foreach (var node in OutputNodes)
            {
                outputSet.Add(node.Value.Name, node.Value.CurrentValue.Value);
            }

            return outputSet;

        }


        public async Task Reset()
        {

            foreach (var node in InputNodes)
            {
                await node.Value.Reset();
            }
        }

        #endregion

        #region Helpers

        public ValueSet CreateEmptyInputValueSet()
        {
            ValueSet outputSet = new ValueSet();

            foreach (var node in InputNodes)
            {
                outputSet.Add(node.Value.Name, 0);
            }

            return outputSet;
        }

        public List<string> Calculators
        {
            get
            {
                List<string> calculators = new List<string>();
                var nodeCalculators = _calculatorFactory.NodeCalculatorTypes.Select(t => t.FullName);
                var connectionCalculators = _calculatorFactory.ConnectionCalculatorTypes.Select(t => t.FullName);

                calculators.AddRange(nodeCalculators);
                calculators.AddRange(connectionCalculators);

                return calculators;
            }
        }

        #endregion

        #region Setup

        public void LoadModel(Models.NeuralNet netModel)
        {
            foreach (var calculator in netModel.ExternalCalculatorTypes)
            {
                AddCalculatorType(calculator);
            }

            foreach (var node in netModel.Nodes)
            {
                AddNode(node);
            }

            foreach (var connection in netModel.Connections)
            {
                AddConnection(connection);
            }

        }

        public Models.NeuralNet GetModel()
        {
            Models.NeuralNet model = new Models.NeuralNet()
            {
                Nodes = new List<Models.NetNode>(),
                Connections = new List<Models.NetConnection>(),
                ExternalCalculatorTypes = new List<ExternalCalculatorType>()
            };

            foreach (var node in Nodes)
            {
                model.Nodes.Add(node.Value.GetModel());
            }

            foreach (var connection in Connections)
            {
                model.Connections.Add(connection.Value.GetModel());
            }

            foreach (var calculatorType in _calculatorFactory.NodeCalculatorTypes)
            {
                ExternalCalculatorType externalCalculatorType = new ExternalCalculatorType()
                {
                    AssemblyPath = calculatorType.Assembly.Location,
                    CalculatorTypeName = calculatorType.FullName
                };

                model.ExternalCalculatorTypes.Add(externalCalculatorType);
            }

            foreach (var calculatorType in _calculatorFactory.ConnectionCalculatorTypes)
            {
                ExternalCalculatorType externalCalculatorType = new ExternalCalculatorType()
                {
                    AssemblyPath = calculatorType.Assembly.Location,
                    CalculatorTypeName = calculatorType.FullName
                };

                model.ExternalCalculatorTypes.Add(externalCalculatorType);

            }

            return model;

        }

        public void AddCalculatorType(ExternalCalculatorType type)
        {
        }

        public void AddNode(Models.NetNode newNodeData)
        {
            NetNode newNode = new NetNode(newNodeData, _calculatorFactory);

            Nodes.Add(newNode.ID, newNode);

            if (newNode.NodeType == Enums.NodeType.Input)
            {
                InputNodes.Add(newNode.Name, newNode);
            }
            else if (newNode.NodeType == Enums.NodeType.Output)
            {
                OutputNodes.Add(newNode.Name, newNode);
            }
            //else { //it's already in a proper place }


        }

        public void AddNodes(List<Models.NetNode> nodes)
        {
            foreach(var node in nodes)
            {
                AddNode(node);
            }
        }

        public void RemoveNode(Guid id)
        {
            NetNode node = Nodes[id];
            if (node != null)
            {
                Nodes.Remove(id);

                if (node.NodeType == Enums.NodeType.Input)
                {
                    InputNodes.Remove(node.Name);
                }
                else if (node.NodeType == Enums.NodeType.Output)
                {
                    OutputNodes.Remove(node.Name);
                }
            }
        }

        public void AddConnection(Models.NetConnection newConnectionData)
        {
            NetNode inNode = Nodes[newConnectionData.InboundNodeID];
            NetNode outNode = Nodes[newConnectionData.OutboundNodeId];

            NetConnection newConnection = new NetConnection(newConnectionData, _calculatorFactory, this);

            inNode.OutboundLinks.Add(newConnection);
            outNode.InboundLinks.Add(newConnection);

            Connections.Add(newConnection.ID, newConnection);


        }

        public void AddConnections(List<Models.NetConnection> newConnections)
        {
            foreach (var connection in newConnections)
            {
                AddConnection(connection);
            }
        }


        public void RemoveConnection(Models.NetConnection newConnectionData)
        {
            NetConnection connection = Connections[newConnectionData.ID];

            if (connection != null)
            {
                connection.InboundNode.OutboundLinks.Remove(connection);
                connection.OutboundNode.InboundLinks.Remove(connection);

                Connections.Remove(connection.ID);
            }


        }

        public NetNode GetNode(Guid id)
        {
            return Nodes[id];

        }

        #endregion
    }
}
