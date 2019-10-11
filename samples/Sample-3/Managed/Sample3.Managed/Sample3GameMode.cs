using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using Rhino.Runtime.InProcess;

namespace Sample3
{
  [UClass, BlueprintType, Blueprintable]
  class ASample3GameMode : AGameMode
  {
    static RhinoCore rhinoCore;

    static ASample3GameMode()
    {
      RhinoInside.Resolver.Initialize();
    }

    public override void Initialize(FObjectInitializer initializer)
    {
      base.Initialize(initializer);
      if(rhinoCore==null)
        rhinoCore = new Rhino.Runtime.InProcess.RhinoCore();
    }

    protected override void BeginPlay()
    {
      base.BeginPlay();

      FMessage.Log(ELogVerbosity.Warning, "Hello from C# (" + this.GetType().ToString() + ":BeginPlay)");

      DoSomething();

    }

    public void DoSomething()
    {
      try
      {
        var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10);
        var mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);

        FMessage.Log(ELogVerbosity.Warning, "Created a mesh with " + mesh.Vertices.Count.ToString() + " vertices and " + mesh.Vertices.Count.ToString() + " Faces.");
      }
      catch (System.Exception ex)
      {
        FMessage.Log(ELogVerbosity.Error, "Something went wrong with Rhino.Inside.");
        FMessage.Log(ELogVerbosity.Error, ex.Message);
      }
    }
  }
}
