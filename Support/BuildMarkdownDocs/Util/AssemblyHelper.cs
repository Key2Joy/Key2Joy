using System.IO;
using System.Reflection;
using Key2Joy;

namespace BuildMarkdownDocs.Util;

public class AssemblyHelper
{
    public AssemblyHelper(string assemblyDirectory, string assemblyName)
    {
        this.AssemblyDirectory = assemblyDirectory;
        this.AssemblyName = assemblyName;
    }

    public string AssemblyDirectory { get; }
    public string AssemblyName { get; }

    public void LoadAssemblyWithRelated()
    {
        var assemblyPath = this.DetermineAssemblyPath();
        this.LoadReferencedAssemblies(assemblyPath);
    }

    public string DetermineAssemblyPath()
    {
        var assemblyFileName = $"{this.AssemblyName}.dll";
        var mainPath = Path.Combine(this.AssemblyDirectory, assemblyFileName);

        if (File.Exists(mainPath))
        {
            return mainPath;
        }

        throw new FileNotFoundException($"Assembly {this.AssemblyName} not found in main directory.");
    }

    public void LoadReferencedAssemblies(string assemblyPath)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        var referencedAssemblies = assembly.GetReferencedAssemblies();

        foreach (var referencedAssemblyName in referencedAssemblies)
        {
            var referencedAssemblyPath = Path.Combine(this.AssemblyDirectory, $"{referencedAssemblyName.Name}.dll");

            if (File.Exists(referencedAssemblyPath))
            {
                Assembly.LoadFrom(referencedAssemblyPath);
            }
        }
    }
}
