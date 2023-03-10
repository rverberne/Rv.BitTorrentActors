using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rv.BitTorrentActors.Bencoding;

namespace Rv.BitTorrentActors.UnitTests.Tests.Bencoding;

[TestClass]
public class BenValueParser_Dictionary_Tests
{
    #nullable disable
    private BenValueParser parser;
    #nullable enable

    [TestInitialize]
    public void SetUp()
    {
        parser = new BenValueParser();
    }

    [TestMethod]
    public void WhenStreamIsValidDictionary_ThenDictionaryIsCorrectlyParsed()
    {
        // Act
        BenValue result = parser.Parse("d3:Fooi42ee");

        // Assert
        BenDictionary dict = (BenDictionary)result;
        Assert.AreEqual(42, dict.GetInt("Foo"));
    }
}