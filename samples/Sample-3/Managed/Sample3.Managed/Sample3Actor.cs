using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using Rhino.Runtime.InProcess;
using System.Collections.Generic;

namespace Sample3
{
  [UClass, BlueprintType, Blueprintable]
  class ASample3Actor : AActor
  {
    static RhinoCore rhinoCore;
    Rhino.Geometry.Mesh mesh;

    static ASample3Actor()
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
    public void DoSomething()
    {
      var sphere = new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10);
      mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);
      mesh.Faces.ConvertQuadsToTriangles();
      mesh.Flip(true, true, true);

      FMessage.Log(ELogVerbosity.Warning, "Created a mesh with " + mesh.Vertices.Count.ToString() + " vertices and " + mesh.Vertices.Count.ToString() + " Faces.");
    }

    [UFunction, BlueprintCallable]
    public List<FVector> GetVertices()
    {
      var list = new List<FVector>();
      foreach (var vert in mesh.Vertices)
        list.Add(new FVector(vert.X, vert.Y, vert.Z));
      return list;
    }

    [UFunction, BlueprintCallable]
    public List<int> GetFaceIds()
    {
      var list = new List<int>();
      foreach (var face in mesh.Faces)
      {
        list.Add(face.A);
        list.Add(face.B);
        list.Add(face.C);
      }
      return list;
    }


  }
}
