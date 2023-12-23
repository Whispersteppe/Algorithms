using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using NeuralNetClassifier.Interface;
using NeuralNetClassifier.Models;

namespace NeuralNetClassifier.Workspace.Calculators
{
    public class SimpleCalculatorFactory : ICalculatorFactory
    {
        Dictionary<string, Type> _nodeCalculatorTypeCache;
        Dictionary<string, Type> _connectionCalculatorTypeCache;
        Dictionary<string, Assembly> _assemblyCache;

        /// <summary>
        /// construct the factory from a list of assemblies and types
        /// </summary>
        /// <param name="assemblyNames"></param>
        /// <param name="types"></param>
        public SimpleCalculatorFactory(List<Type>? types = null) 
        { 
            _connectionCalculatorTypeCache = new Dictionary<string, Type>();
            _nodeCalculatorTypeCache = new Dictionary<string, Type>();
            _assemblyCache = new Dictionary<string, Assembly>();
            

            if (types != null)
            {
                foreach (Type type in types)
                {
                    RegisterCalculator(type);
                }
            }
        }

        /// <summary>
        /// find all calculator types in an assembly
        /// </summary>
        /// <param name="assembly"></param>
        public bool RegisterCalculator(Assembly assembly)
        {
            throw new NotImplementedException("WWe're not doing external types");
        }

        /// <summary>
        /// add a single calculator type and check for duplicates
        /// </summary>
        /// <param name="type"></param>
        public bool RegisterCalculator(Type type)
        {
            bool addedTypes = false;

            if (type.IsInterface == true) return false;  //  we don't need the interfaces.  just the classes

            if (type.IsAssignableTo(typeof(IConnectionCalculate)) == true)
            {
                if (_connectionCalculatorTypeCache.ContainsKey(type.FullName) == false)
                {
                    _connectionCalculatorTypeCache.Add(type.FullName, type);
                    addedTypes = true;
                }
            }
            else if (type.IsAssignableTo(typeof(INodeCalculate)) == true)
            {
                if (_nodeCalculatorTypeCache.ContainsKey(type.FullName) == false)
                {
                    _nodeCalculatorTypeCache.Add(type.FullName, type);
                    addedTypes = true;
                }

            }

            return addedTypes;

        }

        /// <summary>
        /// get an instance of a connection calculator
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public IConnectionCalculate GetConnectionCalculator(string typeName, Dictionary<string, double> values)
        {
            Type type = _connectionCalculatorTypeCache[typeName];

            if (type == null)
            {
                throw new KeyNotFoundException(typeName);
            }

            IConnectionCalculate connectionCalculator = (IConnectionCalculate)type.Assembly.CreateInstance(typeName);
            connectionCalculator.SetValues(values);

            return connectionCalculator;
        }

        /// <summary>
        /// get an instance of a node calculator
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public INodeCalculate GetNodeCalculator(string typeName, Dictionary<string, double> values)
        {
            Type type = _nodeCalculatorTypeCache[typeName];

            if (type == null)
            {
                throw new KeyNotFoundException(typeName);
            }

            INodeCalculate nodeCalculator = (INodeCalculate)type.Assembly.CreateInstance(typeName);

            nodeCalculator.SetValues(values);

            return nodeCalculator;
        }

        public List<Type> NodeCalculatorTypes
        {
            get
            {
                var rslts = _nodeCalculatorTypeCache.Select(x => x.Value).ToList();
                return rslts;
            }
        }

        public List<Type> ConnectionCalculatorTypes
        {
            get
            {
                var rslts = _connectionCalculatorTypeCache.Select(x => x.Value).ToList();
                return rslts;
            }
        }

        public bool RegisterCalculator(ExternalCalculatorType type)
        {
            throw new NotImplementedException("WWe're not doing external types");

        }



    }
}
