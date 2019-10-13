using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using Rhino.PlugIns;
using System;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace Sample4
{
  [UClass, BlueprintType, Blueprintable]
  class ASample4Actor : AActor
  {
    Rhino.Runtime.InProcess.RhinoCore rhinoCore;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    GH_Document definition;


    static ASample4Actor()
    {
      RhinoInside.Resolver.Initialize();
    }

    public override void Initialize(FObjectInitializer initializer)
    {
      base.Initialize(initializer);
      if (rhinoCore == null)
        rhinoCore = new Rhino.Runtime.InProcess.RhinoCore(new string[] { "/NOSPLASH" }, Rhino.Runtime.InProcess.WindowStyle.Hidden);
    }

    protected override void BeginPlay()
    {
      base.BeginPlay();

      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (" + this.GetType().ToString() + ":BeginPlay)");
    }

    [UFunction, BlueprintCallable]
    public void LaunchGrasshopper()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;

      var ghInit = Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false) ? true : false;
    }

    private void Instances_CanvasCreated(GH_Canvas canvas)
    {
      Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
    }

    private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
    {
      definition = e.NewDocument;
      definition.SolutionEnd += Definition_SolutionEnd;
    }

    private void Definition_SolutionEnd(object sender, GH_SolutionEventArgs e)
    {
      // TODO: Add code to harvest display meshes when the Grasshopper Definition solution completes solving.
    }
  }
}
