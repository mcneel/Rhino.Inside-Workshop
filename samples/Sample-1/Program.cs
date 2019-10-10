using System.Linq;

namespace Sample1
{
  class Program
  {
    static Program()
    {
      RhinoInside.Resolver.Initialize();
    }

    [System.STAThread]
    static int Main(string[] args)
    {
      var windowStyle = Rhino.Runtime.InProcess.WindowStyle.NoWindow;
      if (args.Where(arg => string.Equals(arg, "/WindowStyle=Normal", System.StringComparison.OrdinalIgnoreCase)).Any())
        windowStyle = Rhino.Runtime.InProcess.WindowStyle.Normal;

      try
      {
        using (var core = new Rhino.Runtime.InProcess.RhinoCore(args, windowStyle))
        {
          var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10);
          var mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);
          System.Console.WriteLine("Created a mesh with {0} vertices and {1} Faces.", mesh.Vertices.Count, mesh.Faces.Count);

          if (windowStyle != Rhino.Runtime.InProcess.WindowStyle.NoWindow)
          {
            return core.Run();
          }
          else
          {
            System.Console.WriteLine("press any key to exit");
            System.Console.ReadKey();
            return 0;
          }
        }
      }
      catch (System.Exception ex)
      {
        System.Console.Error.WriteLine(ex.Message);
      }

      return -1;
    }
  }
}
