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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    /// <summary>
    /// Item布局模式
    /// </summary>
    public enum ScLayerLayoutViewerLayoutMode
    {
        /// <summary>
        /// 多行单列布局
        /// </summary>
        MORE_ROW_SINGLE_COL_LAYOUT,

        /// <summary>
        /// 多列单行布局
        /// </summary>
        MORE_COL_SINGLE_ROW_LAYOUT,

        /// <summary>
        /// 多行多列，行优先布局
        /// </summary>
        MORE_ROW_MORE_COL_ROW_FIRSTLAYOUT,

        /// <summary>
        /// 多列多行，列优先布局
        /// </summary>
        MORE_COL_MORE_ROW_COL_FIRSTLAYOUT,

        /// <summary>
        /// 自由布局 
        /// </summary>
        FREEDOM_LAYOUT,

    }


    /// <summary>
    /// Item尺寸模式
    /// </summary>
    public enum ScLayerLayoutViewerSizeMode
    {
        /// <summary>
        /// 尺寸固定
        /// </summary>
        SIZE_FIXED,

        /// <summary>
        /// 尺寸不固定
        /// </summary>
        SIZE_UNFIXED, 
    }


    /// <summary>
    /// Item数量模式
    /// </summary>
    public enum ScLayerLayoutViewerCountMode
    {
        /// <summary>
        /// 数量固定
        /// </summary>
        COUNT_FIXED,

        /// <summary>
        /// 数量不固定
        /// </summary>
        COUNT_UNFIXED,
    }



    /// <summary>
    /// Item数据模式
    /// </summary>
    public enum ScLayerLayoutViewerDataMode
    {
        /// <summary>
        /// 数据固定
        /// </summary>
        DATA_FIXED,

        /// <summary>
        /// 数据不固定
        /// </summary>
        DATA_UNFIXED,
    }


    /// <summary>
    /// Header方向
    /// </summary>
    public enum ScLayerLayoutViewerHeaderOrientation
    {
        /// <summary>
        /// 水平方向
        /// </summary>
        HORIZONTAL,

        /// <summary>
        /// 垂直方向
        /// </summary>
        VERTICAL

    }


    /// <summary>
    /// Header尺寸模式
    /// </summary>
    public enum ScLayerLayoutViewerHeaderSizeMode
    {
        /// <summary>
        /// 固定
        /// </summary>
        NONE,

        /// <summary>
        /// 自适应
        /// </summary>
        ADAPTIVE
    }

}
