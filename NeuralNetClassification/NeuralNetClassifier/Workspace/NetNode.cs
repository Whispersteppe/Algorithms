using NeuralNetClassifier.Enums;
using NeuralNetClassifier.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetClassifier.Workspace
{
    public class NetNode
    {
        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public NodeType NodeType { get; private set; }
        public List<NetConnection> InboundLinks { get; private set; }
        public List<NetConnection> OutboundLinks { get; private set; }

        public double? CurrentValue { get; private set; }
        public double? BackPropagatedValue { get; private set; }

        INodeCalculate _calculations;

        //public NetNode(string name, Guid id, NodeType nodeType, INodeCalculate calculations)
        //{
        //    NodeType = nodeType;
        //    Name = name;
        //    _calculations = calculations;

        //    InboundLinks = new List<NetConnection>();
        //    OutboundLinks = new List<NetConnection>();

        //    ID = id;
        //}

        public NetNode(Models.NetNode model, ICalculatorFactory calculatorFactory)
        {
            ID = model.ID;
            Name = model.Name;
            NodeType = model.NodeType;
            InboundLinks = new List<NetConnection>();
            OutboundLinks = new List<NetConnection>();
            CurrentValue = null;
            BackPropagatedValue = null;
            _calculations = calculatorFactory.GetNodeCalculator(model.CalculatorTypeName, model.CalculatorValues);
        }

        public Models.NetNode GetModel()
        {
            Models.NetNode model = new Models.NetNode()
            {
                ID = ID,
                CalculatorTypeName = _calculations.GetType().FullName,
                CalculatorValues = _calculations.GetValues(),
                Name = Name,
                NodeType = NodeType
            };

            return model;
        }


        public async Task Reset()
        {
            if (CurrentValue == null) return;

            CurrentValue = null;
            BackPropagatedValue = null;

            OutboundLinks.ForEach(async link =>
            {
                await link.Reset();
            });
        }


        public async Task Calculate(double inboundValue)
        {
            CurrentValue = inboundValue;

            if (OutboundLinks != null)
            {
                OutboundLinks.ForEach(async link =>
                {
                    await link.Calculate();
                });
            }

        }

        public async Task Calculate()
        {
            if (InboundLinks == null) return;  //  there are no input nodes.  probably wanted to do a calculate with a single value
            if (InboundLinks.Any(x=>x.CurrentValue == null)) return;  //  not all inbounds have been calculated yet.  must wait

            //  do the CalculateInput function and set the value into CurrentValue
            //TODO we'll want to call out to a method to do this for us
            double newValue = InboundLinks.Sum(x => x.CurrentValue.GetValueOrDefault(0));

            await Calculate(newValue);

        }


        public async Task Train()
        {
            if (OutboundLinks == null) return;
            if(OutboundLinks.Any(x=>x.BackPropagatedValue == null)) return;

            BackPropagatedValue = await _calculations.Train(this, OutboundLinks);

            if (InboundLinks == null) return;  //  there are no input nodes.
            InboundLinks.ForEach(async link =>
            {
                await link.Train();
            });


        }



    }


}


