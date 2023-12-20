namespace Whispersteppe.XUnit
{
    /// <summary>
    /// xunit class fixture, for things that need to be kept around for all unit tests in a single class
    /// </summary>
    public class XUnitClassFixture : IDisposable
    {

        /// <summary>
        /// constructor
        /// </summary>
        public XUnitClassFixture()
        {
        }


        /// <summary>
        /// dispose of anything in use in the class
        /// </summary>
        public void Dispose()
        {
            //  nothing to dispose.  carry on
        }
    }
}
