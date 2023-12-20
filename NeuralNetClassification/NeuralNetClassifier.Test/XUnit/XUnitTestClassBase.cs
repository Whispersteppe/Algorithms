using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Whispersteppe.XUnit
{
    /// <summary>
    /// our xUnit test class base.  contains references to a class fixture and a collection fixture, as well as the output helper.
    /// </summary>
    [Collection("Context collection")]
    public class XUnitTestClassBase : IClassFixture<XUnitClassFixture>, IDisposable
    {
        readonly ITestOutputHelper _output;
        readonly XUnitClassFixture _fixture;
        readonly XUnitCollectionFixture _collectionFixture;

        readonly Stopwatch _testStopwatch;
        public XUnitTestClassBase(XUnitClassFixture fixture, XUnitCollectionFixture collectionFixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
            _collectionFixture = collectionFixture;
            _output.WriteLine($"Start at: {DateTime.Now}");
            _testStopwatch = Stopwatch.StartNew();
        }

        public virtual void Dispose()
        {
            _testStopwatch.Stop();
            _output.WriteLine($"Stop at: {DateTime.Now}");
            _output.WriteLine($"Total elapsed time: {_testStopwatch.Elapsed}");
        }

        /// <summary>
        /// write an object out.  uses newtonsoft to serialize and format it.
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks>
        /// added settings to ignore reference loop handling, since this is a thing when running into serializing database objects
        /// </remarks>
        public void WriteLine(object obj)
        {
            _output.WriteLine($"{obj.GetType().Name}:  ");

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            var serializedObject = JsonConvert.SerializeObject(obj, settings); // (obj, Formatting.Indented);

            _output.WriteLine(serializedObject);
            _output.WriteLine("=================");
        }

        /// <summary>
        /// just write out the text
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            _output.WriteLine(text);
        }

        /// <summary>
        /// get the class fixture
        /// </summary>
        public XUnitClassFixture Fixture
        {
            get
            {
                return _fixture;
            }
        }

        /// <summary>
        /// get the repository factory
        /// </summary>
        /// <returns></returns>


        /// <summary>
        /// get a configuration from the collection fixture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        public T GetConfig<T>(string configName) where T : new()
        {
            return _collectionFixture.GetConfig<T>(configName);
        }

        /// <summary>
        /// get a logger
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public ILogger GetLogger(LogLevel logLevel = LogLevel.Trace, string memberName = "")
        {
            var logger = _output.BuildLogger(logLevel, memberName);
            return logger;
        }

        /// <summary>
        /// get a class specific logger
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public ILogger<T> GetLogger<T>(LogLevel logLevel = LogLevel.Trace)
        {
            var logger = _output.BuildLoggerFor<T>(logLevel);
            return logger;
        }

        /// <summary>
        /// gets json from a file and loads it into a object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T? LoadJsonItemFromFile<T>(string filename)
        {
            string data = File.ReadAllText(filename);
            T? item = JsonConvert.DeserializeObject<T>(data);

            return item;
        }


    }
}
