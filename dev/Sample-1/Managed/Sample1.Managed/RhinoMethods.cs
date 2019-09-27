using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino.PlugIns;
using Rhino.Runtime.InProcess;
using UnrealEngine.Runtime;

namespace Sample1
{
  class RhinoMethods
  {
    static RhinoCore m_rhino_core;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    static bool m_rhino_init = false;

    public static Rhino.Geometry.Mesh mesh { get; set; }

    static RhinoMethods()
    {

      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (Sample1.RhinoMethods:Static Constructor)");

      // Resolve RhinoCommon assembly reference
      ResolveEventHandler OnRhinoCommonResolve = null;
      AppDomain.CurrentDomain.AssemblyResolve += OnRhinoCommonResolve = (sender, args) =>
      {
        const string rhino_common_assembly_name = "RhinoCommon";
        var assembly_name = new AssemblyName(args.Name).Name;

        if (assembly_name != rhino_common_assembly_name)
          return null;

        AppDomain.CurrentDomain.AssemblyResolve -= OnRhinoCommonResolve;

        var rhino_system_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "System");
        return Assembly.LoadFrom(Path.Combine(rhino_system_dir, rhino_common_assembly_name + ".dll"));
      };

      ResolveEventHandler OnGHResolve = null;
      AppDomain.CurrentDomain.AssemblyResolve += OnGHResolve = (sender, args) =>
      {
        const string gh_assembly_name = "Grasshopper";
        var assembly_name = new AssemblyName(args.Name).Name;

        if (assembly_name != gh_assembly_name)
          return null;

        AppDomain.CurrentDomain.AssemblyResolve -= OnGHResolve;

        var gh_plugin_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino WIP", "Plug-ins", "Grasshopper");
        return Assembly.LoadFrom(Path.Combine(gh_plugin_dir, gh_assembly_name + ".dll"));
      };


    }

    public static void LaunchRhino()
    {
      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (Sample1.RhinoMethods:LaunchRhino)");

      // Load Rhino
      if(!m_rhino_init)
        try
        {
          //Run Rhino.Inside no UI
          m_rhino_core = new RhinoCore(new string[] { "/NOSPLASH" }, WindowStyle.Hidden);
          m_rhino_init = true;
          FMessage.Log(ELogVerbosity.Warning, DoSomething());
        }
        catch (Exception ex)
        {
          FMessage.Log(ELogVerbosity.Error, "Cannot load Rhino Inside");
          FMessage.Log(ELogVerbosity.Error, ex.Message);
          FMessage.Log(ELogVerbosity.Error, ex.Source);
        }
    }

    public static string DoSomething()
    {
      //var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 100.00);
      //mesh = Rhino.Geometry.Mesh.CreateFromBrep(sphere.ToBrep(), Rhino.Geometry.MeshingParameters.Default)[0];
      //mesh.Faces.ConvertQuadsToTriangles();
      //mesh.Flip(true, true, true);

      Rhino.RhinoDoc.Open(@"E:\dev\Rhino Logo.3dm", out bool wasOpen);
      mesh = new Rhino.Geometry.Mesh();
      foreach (var obj in Rhino.RhinoDoc.ActiveDoc.Objects)
      {
        switch (obj.ObjectType)
        {
          case Rhino.DocObjects.ObjectType.Brep:
            var m = Rhino.Geometry.Mesh.CreateFromBrep(obj.Geometry as Rhino.Geometry.Brep, Rhino.Geometry.MeshingParameters.Default);
            mesh.Append(m);
            break;
        }
      }

      // create mesh

      mesh.Faces.ConvertQuadsToTriangles();
      mesh.Flip(true, true, true);

      return "The mesh has " + mesh.Vertices.Count + " vertices and " + mesh.Faces.Count + " faces.";
    }

    public static void LaunchGrasshopper()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      var ghInit = Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false) ? true : false;
      Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
    }

    private static void Instances_CanvasCreated(GH_Canvas canvas)
    {
      Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += Document_ObjectsAdded;
      Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
    }

    private static void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
      FMessage.Log(ELogVerbosity.Warning, "Hello from C# GH Document Changed");
    }

    private static void Document_ObjectsAdded(GH_Document sender, GH_DocObjectEventArgs e)
    {
      FMessage.Log(ELogVerbosity.Warning, "Hello from C# GH Objects Added");
    }
    
    public static void CreateMesh()
    {
      //mesh = Rhino.Geometry.Mesh.CreateQuadSphere(new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10), 6);
    }

    public static IList<int> GetFaceIds()
    {
      var list = new List<int>();
      if (mesh != null)
        foreach (var face in mesh.Faces)
        {
          list.Add(face.A);
          list.Add(face.B);
          list.Add(face.C);

        }
      return list;
    }

    public static IList<FVector> GetVertices()
    {
      var list = new List<FVector>();
      if (mesh != null)
        foreach (var vert in mesh.Vertices)
          list.Add(new FVector(vert.X, vert.Y, vert.Z));
      return list;
    }

  }
}
