using NeuralNetClassifier.Interface;

namespace NeuralNetClassifier.Workspace
{
    public class NetConnection
    {
        public Guid ID { get; private set; }
        public double? CurrentValue { get; private set; }
        public double? BackPropagatedValue { get; private set; }
        public NetNode InboundNode { get; private set; }
        public NetNode OutboundNode { get; private set; }

        IConnectionCalculate _connectionCalculate;

        //public NetConnection(
        //    Guid id,
        //    NetNode inboundNode, 
        //    NetNode outboundNode, 
        //    IConnectionCalculate connectionCalculate)
        //{
        //    InboundNode = inboundNode;
        //    OutboundNode = outboundNode;
        //    _connectionCalculate = connectionCalculate;
        //    ID = id;

        //}

        public NetConnection(Models.NetConnection connection, ICalculatorFactory calculatorFactory, NeuralNet network)
        {
            ID = connection.ID;
            CurrentValue = null; 
            BackPropagatedValue = null;

            _connectionCalculate = calculatorFactory.GetConnectionCalculator(connection.CalculatorTypeName, connection.CalculatorValues);

            InboundNode = network.GetNode(connection.InboundNodeID);
            OutboundNode = network.GetNode(connection.OutboundNodeId);

        }

        public Models.NetConnection GetModel()
        {
            Models.NetConnection connection = new Models.NetConnection()
            {
                ID = ID,
                CalculatorTypeName = _connectionCalculate.GetType().FullName,
                InboundNodeID = InboundNode.ID,
                OutboundNodeId = OutboundNode.ID,
                CalculatorValues = _connectionCalculate.GetValues()

            };

            return connection;
        }

        public async Task Reset()
        {
            if (CurrentValue == null) return;

            CurrentValue = null;
            BackPropagatedValue = null;

            await OutboundNode.Reset();

        }

        public async Task Calculate()
        {
            CurrentValue = await _connectionCalculate.Calculate(InboundNode);

            if (OutboundNode != null)
            {
                OutboundNode.Calculate();
            }
        }

        public async Task Train()
        {
            BackPropagatedValue = await _connectionCalculate.Train(InboundNode, OutboundNode);

            InboundNode.Train();
        }

    }
}
