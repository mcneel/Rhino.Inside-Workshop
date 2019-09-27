using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace Sample2
{
  [UClass, Blueprintable, BlueprintType]
  public class AMyGHActor: AActor
  {
    public override void Initialize(FObjectInitializer initializer)
    {
      base.Initialize(initializer);
    }

    [UFunction, BlueprintCallable]
    public void LaunchRhino()
    {
      RhinoMethods.LaunchRhino();
      RhinoMethods.LaunchGrasshopper();
    }

    [UFunction, BlueprintCallable]
    public void LaunchGrasshopper()
    {
      
    }


  }
}
