namespace PrintUtils
{
    partial class ReportView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelScroll = new System.Windows.Forms.Panel();
            this.panelView = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelScroll.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelScroll
            // 
            this.panelScroll.AutoScroll = true;
            this.panelScroll.BackColor = System.Drawing.Color.Gainsboro;
            this.panelScroll.Controls.Add(this.panelView);
            this.panelScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScroll.Location = new System.Drawing.Point(0, 0);
            this.panelScroll.Margin = new System.Windows.Forms.Padding(0);
            this.panelScroll.Name = "panelScroll";
            this.panelScroll.Size = new System.Drawing.Size(811, 586);
            this.panelScroll.TabIndex = 0;
            this.panelScroll.SizeChanged += new System.EventHandler(this.panelScroll_SizeChanged);
            // 
            // panelView
            // 
            this.panelView.BackColor = System.Drawing.Color.Transparent;
            this.panelView.Location = new System.Drawing.Point(0, 0);
            this.panelView.Margin = new System.Windows.Forms.Padding(0);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(549, 100);
            this.panelView.TabIndex = 0;
            this.panelView.Paint += new System.Windows.Forms.PaintEventHandler(this.panelView_Paint);
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.panelScroll, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Size = new System.Drawing.Size(811, 586);
            this.tlpMain.TabIndex = 1;
            // 
            // ReportView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "ReportView";
            this.Size = new System.Drawing.Size(811, 586);
            this.panelScroll.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelScroll;
        private System.Windows.Forms.Panel panelView;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
    }
}
