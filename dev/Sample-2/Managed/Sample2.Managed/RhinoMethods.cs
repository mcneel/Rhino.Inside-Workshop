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

namespace Sample2
{
  class RhinoMethods
  {
    static RhinoCore m_rhino_core;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    static bool m_rhino_init = false;

    public static Rhino.Geometry.Mesh mesh { get; set; }

    static RhinoMethods()
    {

      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (Sample2.RhinoMethods:Static Constructor)");

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
      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (Sample2.RhinoMethods:LaunchRhino)");

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
          FMessage.Log(ELogVerbosity.Error, "Cannot load Rhino Inside");
          FMessage.Log(ELogVerbosity.Error, ex.Message);
          FMessage.Log(ELogVerbosity.Error, ex.Source);
        }
    }

    public static void LaunchGrasshopper()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      PlugIn.LoadPlugIn(GrasshopperGuid);
      Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
      
      var ghInit = Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false) ? true : false;
      
      //Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += Document_ObjectsAdded;
      //Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
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

    

  }
}
