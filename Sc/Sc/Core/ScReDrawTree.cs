//----------------------------------------------------------------------------
// Simple Control (Sc) - Version 1.1
// A high quality control rendering engine for C#
// Copyright (C) 2016-2020 cymheart
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

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Drawing;

namespace Sc
{
    public partial class ScReDrawTree
    {
        ScMgr scmgr;
        public ScDrawNode root;

        static RawMatrix3x2 identityMatrix = new RawMatrix3x2(
            1.0f, 0.0f,
            0.0f, 1.0f,
            0.0f, 0.0f
            );

        public void Draw(ScGraphics g)
        {
            if (root == null)
                return;

            g.SetClip(root.clipRect);

            switch (g.GetGraphicsType())
            {
                case GraphicsType.D2D:

                    if (root.isRender)
                        D2DPaint(g, root);

                    DrawChildNodeD2D(root, g);
                    break;


            }

            g.ResetClip();
        }


        void DrawChildNodeD2D(ScDrawNode parent, ScGraphics g)
        {
            Layer d2dLayer;
 
            foreach (ScDrawNode node in parent.nodes)
            {
                if (node.layer.IsComputedStraight)
                {
                    if(node.isRender && node.layer.IsRender)
                        D2DPaint(g, node);

                    DrawChildNodeD2D(node, g);
                }
                else
                {
                    d2dLayer = PushLayer((D2DGraphics)g, node.layer);
                    D2DPaint(g, node);
                    DrawChildNodeD2D(node, g);

                    PopLayer((D2DGraphics)g);
                    d2dLayer.Dispose();
                }
            }
        }

       
        void D2DPaint(ScGraphics g, ScDrawNode node)
        {
            ScLayer layer = node.layer;

            g.SetClip(node.clipRect);
            g.Transform = layer.GlobalMatrix;
            g.layer = layer;
            layer.OnD2DPaint(g);
            g.layer = null;
            g.ResetTransform();
            g.ResetClip();
        }


        public Layer PushLayer(D2DGraphics g, ScLayer sclayer)
        {
            Layer d2dLayer = new Layer(g.RenderTarget);

            LayerParameters layerParameters = new LayerParameters();
            layerParameters.ContentBounds = GDIDataD2DUtils.TransToRawRectF(sclayer.DrawBox);
            layerParameters.LayerOptions = LayerOptions.InitializeForCleartype;
            layerParameters.MaskAntialiasMode = AntialiasMode.PerPrimitive;

            //应用到GeometricMask上的变换，这个变换可能已经在计算布局的时候已经计算到了sclayer.TransLastHitPathGeometry上
            //所以不需要应用变换
            layerParameters.MaskTransform = identityMatrix;   

            layerParameters.Opacity = sclayer.Opacity;
            layerParameters.GeometricMask = sclayer.TransLastHitPathGeometry;

            g.RenderTarget.PushLayer(ref layerParameters, d2dLayer);
            return d2dLayer;
        }

        public void PopLayer(D2DGraphics g)
        {
            g.RenderTarget.PopLayer();
        }


        public ScDrawNode ReCreateReDrawTree(ScLayer rootLayer, Rectangle refreshArea)
        {
            if (rootLayer.Visible == false)
                return null;

            scmgr = rootLayer.ScMgr;

            RectangleF clipRect = new RectangleF(refreshArea.X, refreshArea.Y, refreshArea.Width, refreshArea.Height);        
            root = _AddChildReDrawScLayer(null, clipRect, new List<ScLayer> { rootLayer }); 

            return root;

        }

     
        void AddChildReDrawScLayer(ScDrawNode parentDrawNode, RectangleF parentClipRect)
        {
            ScLayer parentScLayer = parentDrawNode.layer;
            _AddChildReDrawScLayer(parentDrawNode, parentClipRect, parentScLayer.controls);
            _AddChildReDrawScLayer(parentDrawNode, parentClipRect, parentScLayer.DirectClipChildLayerList);
        }


        ScDrawNode _AddChildReDrawScLayer(ScDrawNode parentDrawNode, RectangleF parentClipRect, List<ScLayer> childLayerList)
        {
            if (childLayerList == null)
                return null;

            RectangleF rect;
            ScDrawNode drawNode = null;

            foreach (ScLayer childLayer in childLayerList)
            {
                if (childLayer.Visible == false ||
                    childLayer.IsNotAtRootDrawBoxBound)
                    continue;

                rect = childLayer.DrawBox;
                ScDrawNode clipDrawNode;
                RectangleF clipRect;

                clipDrawNode = parentDrawNode;
                clipRect = parentClipRect;

                if (clipRect.IntersectsWith(rect))
                {
                    rect.Intersect(clipRect);

                    drawNode = new ScDrawNode();
                    drawNode.layer = childLayer;
                    drawNode.clipRect = rect;
                    drawNode.parent = clipDrawNode;

                    if(clipDrawNode != null)
                        clipDrawNode.nodes.Add(drawNode);

                    //子层完全覆盖了父层，父层将不再绘制
                    if (childLayer.BackgroundColor != null &&
                        childLayer.BackgroundColor.Value.A == 255 &&
                        childLayer.IsComputedStraight && clipDrawNode.clipRect.Equals(rect))
                    {
                        clipDrawNode.isRender = false;

                        for (ScDrawNode parentNode = clipDrawNode.parent; parentNode != null; parentNode = parentNode.parent)
                        {
                            if (!parentNode.clipRect.Equals(rect) || parentNode.layer.IsComputedStraight)
                                break;

                            parentNode.isRender = false;
                        }
                    }

                    AddChildReDrawScLayer(drawNode, drawNode.clipRect);
                }
            }

            return drawNode;
        }   
    }
}
