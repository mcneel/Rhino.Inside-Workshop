namespace Sample1
{
  class Program
  {
    static Program()
    {
      RhinoInside.Resolver.Initialize();
    }

    static void Main(string[] args)
    {
      try
      {
        using (new Rhino.Runtime.InProcess.RhinoCore(args))
        {
          var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10);
          var mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);
          System.Console.WriteLine("Created a mesh with {0} vertices and {1} Faces.", mesh.Vertices.Count, mesh.Faces.Count);
          System.Console.WriteLine("press any key to exit");
          System.Console.ReadKey();
        }
      }
      catch (System.Exception ex)
      {
        System.Console.Error.WriteLine(ex.Message);
      }
    }
  }
}
