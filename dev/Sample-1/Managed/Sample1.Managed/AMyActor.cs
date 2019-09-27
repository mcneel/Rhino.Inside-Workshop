using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace Sample1
{
  [UClass, Blueprintable, BlueprintType]
  public class AMyActor : AActor
  {
    // "return null;" is re-written to return a list which is capable of
    // reading / writing the native memory of UE4. All other properties require get;set;
    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<FVector> VertList => null;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<int> FaceIDList => null;


    public override void Initialize(FObjectInitializer initializer)
    {
      base.Initialize(initializer);
    }
    
    [UFunction, BlueprintCallable]
    public void LaunchRhino()
    {
      RhinoMethods.LaunchRhino();
    }

    [UFunction, BlueprintCallable]
    public void DoSomething()
    {
      RhinoMethods.DoSomething();
    }

    [UFunction, BlueprintCallable]
    public void GetMesh()
    {
      var vertList = RhinoMethods.GetVertices();
      foreach (var v in vertList) VertList.Add(v);
      var faceList = RhinoMethods.GetFaceIds();
      foreach (var i in faceList) FaceIDList.Add(i);
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

    protected override void BeginPlay()
    {
      base.BeginPlay();
    }

  }
}
