using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rv.BitTorrentActors.Bencoding;

namespace Rv.BitTorrentActors.UnitTests.Tests.Bencoding;

[TestClass]
public class BenValueParser_Integer_Tests
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
    public void WhenStreamIsValidInteger_ThenIntegerIsCorrectlyParsed1()
    {
        // Act
        BenValue value = parser.Parse("i1e");

        // Assert
        Assert.IsInstanceOfType(value, typeof(BenInteger));
        Assert.AreEqual(1, ((BenInteger)value).Value);
    }

    [TestMethod]
    public void WhenStreamIsValidInteger_ThenIntegerIsCorrectlyParsed2()
    {
        // Act
        BenValue value = parser.Parse("i62e");

        // Assert
        Assert.IsInstanceOfType(value, typeof(BenInteger));
        Assert.AreEqual(62, ((BenInteger)value).Value);
    }

    [TestMethod]
    public void WhenStreamIsValidInteger_ThenIntegerIsCorrectlyParsed3()
    {
        // Act
        BenValue value = parser.Parse("i480e");

        // Assert
        Assert.IsInstanceOfType(value, typeof(BenInteger));
        Assert.AreEqual(480, ((BenInteger)value).Value);
    }
}
