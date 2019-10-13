using Rhino.Runtime.InProcess;
using System;
using System.Windows.Forms;

namespace Sample_2
{
    public partial class Form1 : Form
    {
        Rhino.Runtime.InProcess.RhinoCore rhinoCore;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            rhinoCore = new Rhino.Runtime.InProcess.RhinoCore(new string[] {"/NOSPLASH"}, WindowStyle.Hidden, Handle);
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            rhinoCore.Dispose();
            rhinoCore = null;
            base.OnHandleDestroyed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Rhino.RhinoDoc.ActiveDoc.Objects.AddSphere(new Rhino.Geometry.Sphere(Rhino.Geometry.Point3d.Origin, 10));

            Rhino.RhinoDoc.Open(@"C:\data\Rhino Logo.3dm", out bool alreadyOpen);

            viewportControl1.Viewport.DisplayMode = Rhino.Display.DisplayModeDescription.FindByName("Arctic");
            viewportControl1.Invalidate();
        }
    }
}
