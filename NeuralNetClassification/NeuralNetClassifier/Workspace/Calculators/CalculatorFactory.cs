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
    public class CalculatorFactory : ICalculatorFactory
    {
        Dictionary<string, Type> _nodeCalculatorTypeCache;
        Dictionary<string, Type> _connectionCalculatorTypeCache;
        Dictionary<string, Assembly> _assemblyCache;

        /// <summary>
        /// construct the factory from a list of assemblies and types
        /// </summary>
        /// <param name="assemblyNames"></param>
        /// <param name="types"></param>
        public CalculatorFactory(List<string>? assemblyNames = null, List<Type>? types = null, List<string> ignoreAssemblies = null) 
        { 
            _connectionCalculatorTypeCache = new Dictionary<string, Type>();
            _nodeCalculatorTypeCache = new Dictionary<string, Type>();
            _assemblyCache = new Dictionary<string, Assembly>();
            

            if (types != null)
            {
                foreach (Type type in types)
                {
                    AddCalculatorType(type);
                }
            }

            List<string> _assemblyNames = new List<string>();
            if (ignoreAssemblies != null)
            {
                _assemblyNames.AddRange(ignoreAssemblies);
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (_assemblyNames.Contains(assembly.GetName().Name) == true) continue;

                Debug.WriteLine(assembly.GetName().Name);

                _assemblyNames.Add(assembly.GetName().Name);

                if (_assemblyCache.ContainsKey(assembly.Location) == false)
                {

                    //  load items from existing assemblies
                    if (AddCalculatorTypes(assembly) == true)
                    {

                        _assemblyCache.Add(assembly.Location, assembly);
                    }
                }
            }



            if (assemblyNames != null)
            {

                foreach (string assemblyName in assemblyNames)
                {
                    if (_assemblyCache.ContainsKey(assemblyName) == false)
                    {
                        //  wwe don't have it.  
                        Assembly assembly = Assembly.LoadFrom(assemblyName);
                        _assemblyCache.Add(assembly.Location, assembly);

                        AddCalculatorTypes(assembly);
                    }

                }
            }
        }

        /// <summary>
        /// find all calculator types in an assembly
        /// </summary>
        /// <param name="assembly"></param>
        public bool AddCalculatorTypes(Assembly assembly)
        {
            bool addedTypes = false;

            foreach (Type type in assembly.GetTypes())
            {
                addedTypes = AddCalculatorType(type) || addedTypes;
            }

            return addedTypes;

        }

        /// <summary>
        /// add a single calculator type and check for duplicates
        /// </summary>
        /// <param name="type"></param>
        public bool AddCalculatorType(Type type)
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

        public bool AddCalculatorType(ExternalCalculatorType type)
        {
            bool addedType = false;

            if (_connectionCalculatorTypeCache.ContainsKey(type.CalculatorTypeName) || _nodeCalculatorTypeCache.ContainsKey(type.CalculatorTypeName))
            {
                // we've already got the cache.  
            }

            if (_assemblyCache.ContainsKey(type.AssemblyPath) == false)
            {
                //  wwe don't have it.  
                Assembly assembly = Assembly.LoadFrom(type.AssemblyPath);
                _assemblyCache.Add(assembly.Location, assembly);

                AddCalculatorTypes(assembly);
                addedType = true;
            }

            return addedType;
        }



    }
}
