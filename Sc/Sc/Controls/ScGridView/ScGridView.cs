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

using MouseKeyboardLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sc
{
    public class ColumnSetting
    {
        public delegate ScLayer CreateControlHandler(ScMgr scmgr, ColumnSetting columnSetting);
        public CreateControlHandler CreateHeaderControl;
        public CreateControlHandler CreateItemControl;

        public delegate void DisplayItemHandler(ScLayer columnItem, int dataRowIdx);
        public DisplayItemHandler DisplayItemValue;

        public ScLayerLayoutViewerColumnInfo columnBaseInfo;

        public ColumnSetting(string name, string text, bool isHideText, bool isHideColoum, float width)
        {
            columnBaseInfo = new ScLayerLayoutViewerColumnInfo(name, text, isHideText, isHideColoum, width);
        }
    }


    public class ScGridView : ScGridViewCoreContainer
    {
        List<ColumnSetting> columnSettingList = new List<ColumnSetting>();
        List<ScLayerLayoutViewerColumnInfo> columnBaseInfoList = new List<ScLayerLayoutViewerColumnInfo>();

        public List<bool> dataSelectedList = new List<bool>();
        public List<int> selectedDataIdxList = new List<int>();
        public ScLayerLayoutViewerItem selectedItem = null;

      //  MouseHook mouseHook = new MouseHook();
      //  KeyboardHook keyboardHook = new KeyboardHook();
        bool isKeyDownCtrl = false;

        public ScGridView(ScMgr scmgr)
            : base(scmgr)
        {       
            CreateHeaderItemDataLayerEvent += GridView_CreateHeaderItemDataLayerEvent;
            CreateDataItemDataLayerEvent += GridView_CreateDataItemDataLayerEvent;
            ItemDataSetValueEvent += GridView_ItemDataSetValueEvent;
            ViewerItemMouseDownEvent += GridView_ViewerItemMouseDownEvent;

            //
           // keyboardHook.KeyDown += KeyboardHook_KeyDown;
           // keyboardHook.KeyUp += KeyboardHook_KeyUp;

          //  mouseHook.Start();
           // keyboardHook.Start();
        }

        private void KeyboardHook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey ||
              e.KeyCode == Keys.RControlKey)
            {
                isKeyDownCtrl = false;
            }
        }

        private void KeyboardHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey ||
               e.KeyCode == Keys.RControlKey)
            {
                isKeyDownCtrl = true;
            }
        }

        public void AppendColumnSetting(ColumnSetting columnSetting)
        {
            if (string.IsNullOrEmpty(columnSetting.columnBaseInfo.dataName))
                columnSetting.columnBaseInfo.dataName = "_COL_" + columnSettingList.Count;

            columnSettingList.Add(columnSetting);
        }

        public void AppendColumnSettingEnd()
        {
            foreach (ColumnSetting columnSetting in columnSettingList)
            {
                columnBaseInfoList.Add(columnSetting.columnBaseInfo);
            }

            CreateHeaderItemFormDataInfo(columnBaseInfoList.ToArray());
        }

        public void ClearColumnSetting()
        {
            columnSettingList.Clear();
        }


        public void ResetDataRowCount(int dataRowCount)
        {
            SuspendLayout();

            dataSelectedList.Clear();
            for (int i=0; i< dataRowCount; i++)
                dataSelectedList.Add(false);

            selectedDataIdxList.Clear();
            ClearSelectedItems();
            SetRowCount(dataRowCount);
            SetContentShowPos(GetContentShowPos());
            ResumeLayout(true);
        }

        private void GridView_CreateHeaderItemDataLayerEvent(ScLayer contentLayer, string name)
        {
            ScLayer headerControl;

            foreach (ColumnSetting columnSetting in columnSettingList)
            {
                if (columnSetting.columnBaseInfo.dataName == name)
                {
                    headerControl = columnSetting.CreateHeaderControl(contentLayer.ScMgr, columnSetting);
                    contentLayer.Add(headerControl);
                    return;
                }
            }
        }


        private void GridView_CreateDataItemDataLayerEvent(ScLayerLayoutViewerItem dataItem, ScLayer contentLayer, string name)
        {
            ScLayer itemControl;

            foreach (ColumnSetting columnSetting in columnSettingList)
            {
                if (columnSetting.columnBaseInfo.dataName == name)
                {
                    itemControl = columnSetting.CreateItemControl(contentLayer.ScMgr, columnSetting);
                    contentLayer.Add(itemControl);
                    return;
                }
            }
        }


        private void GridView_ItemDataSetValueEvent(ScLayerLayoutViewerItem[] viewItems, List<Dictionary<string, ScLayer>> userItemLayerList, int dataStartIdx, int dataEndIdx)
        {
            ScLayer item;
            int idx = dataStartIdx;
            Dictionary<string, ScLayer> userLayerDict;


            for (int i = 0; i < userItemLayerList.Count(); i++)
            {
                userLayerDict = userItemLayerList[i];

                foreach (ColumnSetting columnSetting in columnSettingList)
                {
                    item = userLayerDict[columnSetting.columnBaseInfo.dataName];
                    columnSetting.DisplayItemValue(item, idx);
                }

                idx++;
            }

            ClearSelectedItems();

            foreach (int n in selectedDataIdxList)
            {
                if (n >= dataStartIdx && n <= dataEndIdx)
                {
                    viewItems[n - dataStartIdx].IsSelected = true;
                }
            }
        }

        private void GridView_ViewerItemMouseDownEvent(ScLayerLayoutViewerItem viewerItem, Dictionary<string, ScLayer> userLayerDict, ScMouseEventArgs e)
        {
            if(isKeyDownCtrl == true)
            {
                int a;
                a = 3;
            }


            ClearSelectedItems();

            foreach (int i in selectedDataIdxList)
            {
                dataSelectedList[i] = false;
            }

            selectedItem = viewerItem;
            viewerItem.IsSelected = true;
            dataSelectedList[viewerItem.DataIdx] = true;

            selectedDataIdxList.Clear();
            selectedDataIdxList.Add(viewerItem.DataIdx);
        }


        void SetSelectedRow(int idx)
        {
            ClearSelectedItems();

            foreach (int j in selectedDataIdxList)
            {
                dataSelectedList[j] = false;
            }

            selectedDataIdxList.Clear();
            if (idx >= 0)
            {
                dataSelectedList[idx] = true;
                selectedDataIdxList.Add(idx);
            }
        }

        public void SetShowRow(int rowIdx)
        {
            float contentPos = rowIdx * (RowHeight + RowSpacing);
            SetContentShowPos(contentPos);
            Refresh();
        }

        public void RemoveSelectedRows()
        {
            int removeCount = selectedDataIdxList.Count;
            if (removeCount <= 0)
                return;

            foreach (int i in selectedDataIdxList)
            {
                dataSelectedList.RemoveAt(i);
            }

            selectedDataIdxList.Clear();
            RemoveRowCount(removeCount);
            SetSelectedRow(0);
            SetShowRow(0);
        }


        public void RemoveRow(int idx)
        {
            for(int i=0; i< selectedDataIdxList.Count; i++)
            {
                if(selectedDataIdxList[i] == idx)
                {
                    dataSelectedList.RemoveAt(idx);
                    selectedDataIdxList.RemoveAt(i);
                    RemoveRowCount(1);
                    return;
                }
            }
        }

        public void ClearAllData()
        {
            dataSelectedList.Clear();
            selectedDataIdxList.Clear();
            ClearSelectedItems();
            RemoveAllRowCount();
            SetContentShowPos(0);
        }
    }
}
