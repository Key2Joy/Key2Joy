using System.IO;
using BuildMarkdownDocs.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Key2Joy.Tests.BuildMarkdownDocs.Util;

[TestClass]
public class AssemblyHelperTests
{
    private const string TestDirectoryPath = "mockDirectory";

    [TestInitialize]
    public void Initialize()
    {
        if (!Directory.Exists(TestDirectoryPath))
        {
            Directory.CreateDirectory(TestDirectoryPath);
        }

        File.WriteAllText(Path.Combine(TestDirectoryPath, "mockAssembly.dll"), "this content doesn't matter, since we wont load it");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(TestDirectoryPath))
        {
            Directory.Delete(TestDirectoryPath, true);
        }
    }

    [TestMethod]
    public void DetermineAssemblyPath_MainDirectory_ReturnsMainPath()
    {
        var helper = new AssemblyHelper(TestDirectoryPath, "mockAssembly");
        var result = helper.DetermineAssemblyPath();
        Assert.AreEqual(Path.Combine(TestDirectoryPath, "mockAssembly.dll"), result);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void DetermineAssemblyPath_NotFound_ThrowsException()
    {
        var helper = new AssemblyHelper(TestDirectoryPath, "nonExistentAssembly");
        helper.DetermineAssemblyPath();
    }
}
