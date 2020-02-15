
using System.Collections.Generic;
using System.Drawing;
using Sc;

namespace Demo2
{
    public class App
    {
        ScMgr scMgr;
        ScLayer root;

        public App(ScMgr scMgr)
        {   
            this.scMgr = scMgr;
            root = scMgr.GetRootLayer();

            TestLayer testLayer = new TestLayer(scMgr);
            testLayer.Name = "layer1";
            testLayer.Location = new PointF(100, 100);
              testLayer.Width = 300;
              testLayer.Height = 300;
            //testLayer.Dock = ScDockStyle.Fill;
             testLayer.BackgroundColor = Color.FromArgb(255, 255,0,255);
             root.Add(testLayer);


            //  testLayer = new TestLayer(scMgr);
            //  testLayer.Name = "layer2";
            //  testLayer.Location = new PointF(500, 400);
            //  testLayer.Width = 300;
            //  testLayer.Height = 800;
            //  testLayer.BackgroundColor = Color.FromArgb(100, 255, 0, 255);
            //  root.Add(testLayer);


            ScTextBox text = new ScTextBox(scMgr);
            text.Location = new PointF(150, 0);
            text.Width = 200;
            text.Height = 30;
            root.Add(text);
        }


    }
}
