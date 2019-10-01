using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace Sample2
{
  [UClass, Blueprintable, BlueprintType]
  public class AMyGHActor: AActor
  {

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<FVector> VertList => null;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<int> FaceIDList => null;


    public override void Initialize(FObjectInitializer initializer)
    {
      base.Initialize(initializer);

    }

    protected override void BeginPlay()
    {
      base.BeginPlay();
    }


    [UFunction, BlueprintCallable]
    public bool CheckMeshes()
    {
      Debug.WriteLine("Checking Meshes");

      return RhinoMethods.needsCheck;
      
    }

    [UFunction, BlueprintCallable]
    public void LaunchRhino()
    {
      RhinoMethods.LaunchRhino();
    }

    [UFunction, BlueprintCallable]
    public void CloseRhino()
    {
      RhinoMethods.CloseRhino();
    }

    [UFunction, BlueprintCallable]
    public void LaunchGrasshopper()
    {
      RhinoMethods.LaunchGrasshopper();
    }

    [UFunction, BlueprintCallable]
    public IList<FVector> GetVertices()
    {
      var vertList = RhinoMethods.GetVertices();
      VertList.Clear();
      foreach (var v in vertList) VertList.Add(v);
      return VertList;
    }

    [UFunction, BlueprintCallable]
    public IList<int> GetFaceIds()
    {
      var faceList = RhinoMethods.GetFaceIds();
      FaceIDList.Clear();
      foreach (var i in faceList) FaceIDList.Add(i);
      return FaceIDList;

    }









  }

}
