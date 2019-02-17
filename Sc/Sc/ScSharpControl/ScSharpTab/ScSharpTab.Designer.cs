namespace Sc
{
    partial class ScSharpTab
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelTabBody = new System.Windows.Forms.Panel();
            this.panelTabHead = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelTabBody, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelTabHead, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(490, 334);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelTabBody
            // 
            this.panelTabBody.BackColor = System.Drawing.Color.Transparent;
            this.panelTabBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTabBody.Location = new System.Drawing.Point(0, 31);
            this.panelTabBody.Margin = new System.Windows.Forms.Padding(0);
            this.panelTabBody.Name = "panelTabBody";
            this.panelTabBody.Size = new System.Drawing.Size(490, 303);
            this.panelTabBody.TabIndex = 1;
            // 
            // panelTabHead
            // 
            this.panelTabHead.BackColor = System.Drawing.Color.Transparent;
            this.panelTabHead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTabHead.Location = new System.Drawing.Point(0, 0);
            this.panelTabHead.Margin = new System.Windows.Forms.Padding(0);
            this.panelTabHead.Name = "panelTabHead";
            this.panelTabHead.Size = new System.Drawing.Size(490, 31);
            this.panelTabHead.TabIndex = 0;
            // 
            // ScSharpTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ScSharpTab";
            this.Size = new System.Drawing.Size(490, 334);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelTabHead;
        private System.Windows.Forms.Panel panelTabBody;
    }
}
