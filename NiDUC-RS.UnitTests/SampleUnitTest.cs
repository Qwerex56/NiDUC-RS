namespace NiDUC_RS.UnitTests;

public class SampleTests {
    [SetUp]
    public void Setup() { }

    [Test]
    public void AddTests() {
        const int a = 9;
        const int b = 10;
        Assert.That(a + b, Is.EqualTo(19));
    }
}