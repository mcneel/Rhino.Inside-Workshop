using System;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using Rhino.Runtime.InProcess;
using Grasshopper.GUI.Canvas;
using Rhino.PlugIns;
using Grasshopper.Kernel;
using System.Linq;
using System.Collections.Generic;

namespace Sample4
{
  [UClass, BlueprintType, Blueprintable]
  class ASample4Actor : AActor
  {

    static RhinoCore rhinoCore;
    static readonly Guid GrasshopperGuid = new Guid(0xB45A29B1, 0x4343, 0x4035, 0x98, 0x9E, 0x04, 0x4E, 0x85, 0x80, 0xD9, 0xCF);
    static GH_Document definition;
    static Rhino.Geometry.Mesh mesh;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<FVector> VertList => null;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<int> FaceIDList => null;

    [UProperty, EditAnywhere, BlueprintReadWrite]
    public IList<FColor> VertexColors => null;

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
    public void LoadRhino()
    {
      if (rhinoCore == null)
        rhinoCore = new RhinoCore(new string[] { "/NOSPLASH" }, WindowStyle.Hidden);

    }

    [UFunction, BlueprintCallable]
    public void LoadGH()
    {
      if (!PlugIn.LoadPlugIn(GrasshopperGuid))
        return;

      var script = new Grasshopper.Plugin.GH_RhinoScriptInterface();

      if(!script.IsEditorLoaded())
        script.LoadEditor();

      script.ShowEditor();

      if(definition == null)
        Grasshopper.Instances.DocumentServer.DocumentAdded += DocumentServer_DocumentAdded;
      //Rhino.RhinoApp.RunScript("!_-Grasshopper _W _T ENTER", false);
    }

    [UFunction, BlueprintCallable]
    public void Unload()
    {
      definition.SolutionEnd -= Definition_SolutionEnd;
    }

    private void DocumentServer_DocumentAdded(GH_DocumentServer sender, GH_Document doc)
    {
      doc.SolutionEnd += Definition_SolutionEnd;
      definition = doc;
    }

    private void Definition_SolutionEnd(object sender, GH_SolutionEventArgs e)
    {
      FMessage.Log(ELogVerbosity.Warning, "Solution End");
      if (definition != e.Document)
        return;

      mesh = GetDocumentPreview(e.Document);

      if (mesh == null)
        return;

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

      VertexColors.Clear();
      foreach(var color in mesh.VertexColors)
      {
        VertexColors.Add(new FColor(color.R, color.G, color.B));
      }

    }

    [UFunction, BlueprintCallable]
    public IList<FVector> GetVertices()
    {
        return VertList;
    }

    [UFunction, BlueprintCallable]
    public IList<int> GetFaceIds()
    {
        return FaceIDList;
    }

    [UFunction, BlueprintCallable]
    public IList<FColor> GetVertexColors()
    {
      return VertexColors;
    }

    Rhino.Geometry.Mesh GetDocumentPreview(GH_Document document)
    {
      var meshPreview = new Rhino.Geometry.Mesh();

      var meshes = new List<Rhino.Geometry.Mesh>();

      foreach (var obj in document.Objects.OfType<IGH_ActiveObject>())
      {
        if (obj.Locked)
          continue;

        if (obj is IGH_PreviewObject previewObject)
        {
          if (previewObject.IsPreviewCapable)
          {
            if (obj is IGH_Component component)
            {
              if (!component.Hidden)
                foreach (var param in component.Params.Output)
                  meshes.AddRange(GetParamMeshes(param));
            }
            else if (obj is IGH_Param param)
            {
              meshes.AddRange(GetParamMeshes(param));
            }
          }
        }
      }

      if (meshes.Count > 0)
      {
        meshPreview.Append(meshes);
        return meshPreview;
      }
      else return null;
    }

    public List<Rhino.Geometry.Mesh> GetParamMeshes(IGH_Param param)
    {
      var meshes = new List<Rhino.Geometry.Mesh>();
      foreach (var value in param.VolatileData.AllData(true))
      {
        if (value is IGH_PreviewData)
        {
          switch (value.ScriptVariable())
          {
            case Rhino.Geometry.Mesh mesh:
              meshes.Add(mesh);
              break;
            case Rhino.Geometry.Brep brep:
              var previewMesh = new Rhino.Geometry.Mesh();
              previewMesh.Append(Rhino.Geometry.Mesh.CreateFromBrep(brep, Rhino.Geometry.MeshingParameters.Default));
              meshes.Add(previewMesh);
              break;
          }
        }
      }
      return meshes;
    }

  }
}
