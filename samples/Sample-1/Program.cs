using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rhino.Runtime.InProcess;

namespace Sample1
{
  class Program
  {

    static readonly string SystemDir = (string) Microsoft.Win32.Registry.GetValue
    (
      @"HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install", "Path",
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "System")
    );

    
    static Program()
    {
      ResolveEventHandler OnRhinoCommonResolve = null;
      AppDomain.CurrentDomain.AssemblyResolve += OnRhinoCommonResolve = (sender, args) =>
      {
        const string rhinoCommonAssemblyName = "RhinoCommon";
        var assemblyName = new AssemblyName(args.Name).Name;

        if (assemblyName != rhinoCommonAssemblyName)
          return null;

        AppDomain.CurrentDomain.AssemblyResolve -= OnRhinoCommonResolve;
        return Assembly.LoadFrom(Path.Combine(SystemDir, rhinoCommonAssemblyName + ".dll"));
      };
    }

    static void Main(string[] args)
    {
      try
      {
        using (new RhinoCore(args))
        {
          var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10);
          var mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);
          Console.WriteLine("Created a mesh with {0} vertices and {1} Faces.", mesh.Vertices.Count, mesh.Faces.Count);
          Console.WriteLine("press any key to exit");
          Console.ReadKey();
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.Message);
      }
    }
  }
}
