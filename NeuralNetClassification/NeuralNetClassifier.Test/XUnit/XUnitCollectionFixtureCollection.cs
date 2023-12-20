namespace Whispersteppe.XUnit
{
    /// <summary>
    /// collection definition for our collection fixture
    /// </summary>
    [CollectionDefinition("Context collection")]
    public class XUnitCollectionFixtureCollection : ICollectionFixture<XUnitCollectionFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
