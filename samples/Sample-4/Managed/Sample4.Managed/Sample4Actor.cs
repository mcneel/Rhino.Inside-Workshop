using System;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using Rhino.Runtime.InProcess;
using Grasshopper.GUI.Canvas;
using Rhino.PlugIns;
using Grasshopper.Kernel;

namespace Sample4
{
  [UClass, BlueprintType, Blueprintable]
  class ASample4Actor : AActor
  {

    static RhinoCore rhinoCore;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    static GH_Document definition;

    static ASample4Actor()
    {
      RhinoInside.Resolver.Initialize();
    }

    protected override void BeginPlay()
    {
      base.BeginPlay();

      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (" + this.GetType().ToString() + ":BeginPlay)"); 
    }

    [UFunction, BlueprintCallable]
    public void LaunchRhino()
    {
      if (rhinoCore == null)
        rhinoCore = new RhinoCore(new string[] { "/NOSPLASH" }, WindowStyle.Hidden);
    }

    [UFunction, BlueprintCallable]
    public void LaunchGrasshopper()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
      Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false);
    }

    [UFunction, BlueprintCallable]
    public void ToggleGrasshopper()
    {
      Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false);
    }

    private static void Instances_CanvasCreated(GH_Canvas canvas)
    {
      Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
    }

    private static void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
      definition = e.NewDocument;
      definition.SolutionEnd += Definition_SolutionEnd;
      FMessage.Log(ELogVerbosity.Warning, "Document Changed");
    }

    private static void Definition_SolutionEnd(object sender, GH_SolutionEventArgs e)
    {
      FMessage.Log(ELogVerbosity.Warning, "Solution End");
    }
  }
}
