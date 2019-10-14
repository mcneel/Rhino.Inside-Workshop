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

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<FVector> VertList => null;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<int> FaceIDList => null;

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
      var mesh = Rhino.Geometry.Mesh.CreateFromSphere(sphere, 10, 10);
      mesh.Faces.ConvertQuadsToTriangles();
      mesh.Flip(true, true, true);

      VertList.Clear();
      foreach (var vert in mesh.Vertices)
        VertList.Add(new FVector(vert.X, vert.Y, vert.Z));

      FaceIDList.Clear();
      foreach (var face in mesh.Faces)
      {
        FaceIDList.Add(face.A);
        FaceIDList.Add(face.B);
        FaceIDList.Add(face.C);
      }

      FMessage.Log(ELogVerbosity.Warning, "Created a mesh with " + mesh.Vertices.Count.ToString() + " vertices and " + mesh.Vertices.Count.ToString() + " Faces.");
    }

    [UFunction, BlueprintCallable]
    public IList<FVector> GetVertices()
    {
      if (VertList != null)
        return VertList;
      return null;
    }

    [UFunction, BlueprintCallable]
    public IList<int> GetFaceIds()
    {
      if (FaceIDList != null)
        return FaceIDList;
      return null;
    }


  }
}
