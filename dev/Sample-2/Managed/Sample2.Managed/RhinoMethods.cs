using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Sample2
{
  public class RhinoMethods
  {
    static RhinoCore m_rhino_core;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    static bool m_rhino_init = false;

    public static Rhino.Geometry.Mesh mesh { get; set; }

    public static bool needsCheck = false;
    static GH_Document definition;

    static RhinoMethods()
    {
      // Resolve RhinoCommon assembly reference
      ResolveEventHandler OnRhinoCommonResolve = null;
      AppDomain.CurrentDomain.AssemblyResolve += OnRhinoCommonResolve = (sender, args) =>
      {
        const string rhino_common_assembly_name = "RhinoCommon";
        var assembly_name = new AssemblyName(args.Name).Name;

        if (assembly_name != rhino_common_assembly_name)
          return null;

        AppDomain.CurrentDomain.AssemblyResolve -= OnRhinoCommonResolve;

        var rhino_system_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino 7 WIP", "System");
        var ass = Assembly.LoadFrom(Path.Combine(rhino_system_dir, rhino_common_assembly_name + ".dll"));
        return ass;
      };

      ResolveEventHandler OnGHResolve = null;
      AppDomain.CurrentDomain.AssemblyResolve += OnGHResolve = (sender, args) =>
      {
        const string gh_assembly_name = "Grasshopper";
        var assembly_name = new AssemblyName(args.Name).Name;

        if (assembly_name != gh_assembly_name)
          return null;

        AppDomain.CurrentDomain.AssemblyResolve -= OnGHResolve;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var gh_plugin_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Rhino 7 WIP", "Plug-ins", "Grasshopper");

        var ass = Assembly.LoadFrom(Path.Combine(gh_plugin_dir, gh_assembly_name + ".dll"));
        return ass;
      };
    }

    public static void LaunchRhino()
    {

      // Load Rhino
      if (!m_rhino_init)
        try
        {
          //Run Rhino.Inside no UI
          m_rhino_core = new RhinoCore(new string[] { "/NOSPLASH" }, WindowStyle.Hidden);
          m_rhino_init = true;
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.Message);
          Debug.WriteLine("Cannot load Rhino Inside");
        }
    }

    public static void CloseRhino()
    {
      m_rhino_core.Dispose();
      m_rhino_core = null;
      m_rhino_init = false;
    }

    public static void LaunchGrasshopper()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      PlugIn.LoadPlugIn(GrasshopperGuid);
      Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;

      var ghInit = Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false) ? true : false;

    }

    private static void Instances_CanvasCreated(GH_Canvas canvas)
    {
      Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += Document_ObjectsAdded;
      Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
    }

    private static void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
      Debug.WriteLine("GH: Document Changed");
      definition = e.NewDocument;
      definition.SolutionEnd += Definition_SolutionEnd;
    }

    private static void Definition_SolutionEnd(object sender, GH_SolutionEventArgs e)
    {
      // Process Meshes
      mesh = GetDocumentPreview(e.Document);
      if(mesh!=null && mesh.Vertices.Count >0)
        needsCheck = true;
      //definition.SolutionEnd -= Definition_SolutionEnd;


    }

    private static void Document_ObjectsAdded(GH_Document sender, GH_DocObjectEventArgs e)
    {
      Debug.WriteLine("GH: Document Objects Added");

    }

    static Rhino.Geometry.Mesh GetDocumentPreview(GH_Document document)
    {
      var meshPreview = new Rhino.Geometry.Mesh();

      foreach (var obj in document.Objects.OfType<IGH_ActiveObject>())
      {
        if (obj.Locked)
          continue;

        if (obj is IGH_PreviewObject previewObject)
        {
          if (previewObject.IsPreviewCapable)
          {
            //primitivesBoundingBox = Rhino.Geometry.BoundingBox.Union(primitivesBoundingBox, previewObject.ClippingBox);

            if (obj is IGH_Component component)
            {
              if (!component.Hidden)
                foreach (var param in component.Params.Output)
                  foreach (var value in param.VolatileData.AllData(true))
                  {
                    if (value is IGH_PreviewData)
                    {
                      switch (value.ScriptVariable())
                      {
                        case Rhino.Geometry.Mesh mesh:
                          meshPreview.Append(mesh);
                          break;
                        case Rhino.Geometry.Brep brep:
                          var previewMesh = new Rhino.Geometry.Mesh();
                          previewMesh.Append(Rhino.Geometry.Mesh.CreateFromBrep(brep, Rhino.Geometry.MeshingParameters.Default));
                          meshPreview.Append(previewMesh);
                          break;
                      }
                    }
                  }
            }
            else if (obj is IGH_Param param)
            {
              foreach (var value in param.VolatileData.AllData(true))
              {
                if (value is IGH_PreviewData)
                {
                  switch (value.ScriptVariable())
                  {
                    case Rhino.Geometry.Mesh mesh:
                      meshPreview.Append(mesh);
                      break;
                    case Rhino.Geometry.Brep brep:
                      var previewMesh = new Rhino.Geometry.Mesh();
                      previewMesh.Append(Rhino.Geometry.Mesh.CreateFromBrep(brep, Rhino.Geometry.MeshingParameters.Default));
                      meshPreview.Append(previewMesh);
                      break;
                  }
                }
              }
            }
          }
        }
      }

      return meshPreview;
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
