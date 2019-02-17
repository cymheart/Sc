//----------------------------------------------------------------------------
// Simple Control (Sc) - Version 1.0
// A high quality control rendering engine for C#
// Copyright (C) 2016-2017 cymheart
// Contact: 
//
// 
// Sc is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// Sc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Sc; if not, write to the Free Software
//----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RectangleF = System.Drawing.RectangleF;
using Matrix = System.Drawing.Drawing2D.Matrix;


namespace Sc
{
    public partial class ScReDrawTree
    {
        void RunGDIPaint(ScGraphics g)
        {
            ScMgr scmgr = root.layer.ScMgr;
            scmgr.ClearBitmapRect(root.clipRect);
            GDIPaint(g, root);
            DrawChildNodeGDI(root, g);
        }

        void DrawChildNodeGDI(ScDrawNode parent, ScGraphics g)
        {
            GDILayer gdiLayer;

            foreach (ScDrawNode node in parent.nodes)
            {
                if (node.layer.IsComputedStraight)
                {
                    GDIPaint(g, node);
                    DrawChildNodeGDI(node, g);
                }
                else
                {
                    gdiLayer = PushLayer((GDIGraphics)g, node);
                    GDIPaintLayer((GDIGraphics)g, node);
                    DrawChildNodeGDI(node, g);
                    PopLayer((GDIGraphics)g);
                    gdiLayer.Dispose();
                }
            }
        }

        void GDIPaint(ScGraphics g, ScDrawNode node)
        {
            if (node == null)
                return;

            ScLayer layer = node.layer;

            if (node.rootLayer == null)
            {
                g.SetClip(node.clipRect);
                g.Transform = layer.GlobalMatrix;
                g.layer = layer;
                layer.OnGDIPaint(g);
            }
            else
            {
                ScLayer rootLayer = node.rootLayer;

                Matrix m = new Matrix();
                m.Translate(-rootLayer.DrawBox.X, -rootLayer.DrawBox.Y);
                m.Multiply(layer.GlobalMatrix);

                g.SetClip(node.clipRect);
                g.Transform = m;
                g.layer = layer;
                layer.OnGDIPaint(g);
                m.Dispose();
            }

            g.layer = null;
            g.ResetTransform();
            g.ResetClip();
        }

        void GDIPaintLayer(GDIGraphics g, ScDrawNode node)
        {
            if (node == null)
                return;

            ScLayer layer = node.layer;

            g.SetClip(node.clipRect);
            g.Transform = node.m;
            layer.OnGDIPaint(g);
            g.ResetTransform();
            g.ResetClip();

            node.m.Dispose();
        }


        public GDILayer PushLayer(GDIGraphics g, ScDrawNode node)
        {
            ScLayer sclayer = node.layer;
            GDILayer gdiLayer = new GDILayer(g);

            GDILayerParameters layerParameters = new GDILayerParameters();
            layerParameters.ContentBounds = sclayer.DrawBox;
            layerParameters.MaskAntialiasMode = GDIAntialiasMode.PerPrimitive;
            layerParameters.Opacity = sclayer.Opacity;

            layerParameters.ClipRect = new RectangleF(
             (int)(node.clipRect.X - sclayer.DrawBox.X - 1), (int)(node.clipRect.Y - sclayer.DrawBox.Y - 1),
             (int)(node.clipRect.Width + 2), (int)(node.clipRect.Height + 2));


            Matrix m = new Matrix();
            m.Translate(-sclayer.DrawBox.X, -sclayer.DrawBox.Y);
            m.Multiply(sclayer.GlobalMatrix);
            node.m = m;
            node.rootLayer = node.layer;

            if (!sclayer.Parent.IsComputedStraight)
            {
                m = new Matrix();
                m.Translate(-sclayer.Parent.DrawBox.X, -sclayer.Parent.DrawBox.Y);
                m.Multiply(sclayer.GlobalMatrix);

                layerParameters.GeometricMask = sclayer.CreateTransLastHitGeometryForGDI(m);
                m.Dispose();
            }
            else
            {
                layerParameters.GeometricMask = sclayer.TransLastHitGraphicsPath;
                layerParameters.parentClipRect = node.clipRect;
            }

            node.clipRect = layerParameters.ClipRect;

            layerParameters.sclayer = sclayer;

            g.PushLayer(layerParameters, gdiLayer);
            return gdiLayer;
        }

        public void PopLayer(GDIGraphics g)
        {
            g.PopLayer();
        }
    }


}
