namespace PrintUtils
{
    partial class CreatePrinterExSettingFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatePrinterExSettingFrm));
            this.printerList = new System.Windows.Forms.ComboBox();
            this.btn_Create = new System.Windows.Forms.Button();
            this.textBoxPreSetName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxExportName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PopupBtn_Close = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // printerList
            // 
            this.printerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printerList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printerList.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.printerList.FormattingEnabled = true;
            this.printerList.Location = new System.Drawing.Point(1, 1);
            this.printerList.Name = "printerList";
            this.printerList.Size = new System.Drawing.Size(428, 29);
            this.printerList.TabIndex = 2;
            // 
            // btn_Create
            // 
            this.btn_Create.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Create.BackgroundImage")));
            this.btn_Create.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Create.FlatAppearance.BorderSize = 0;
            this.btn_Create.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Create.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Create.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Create.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.btn_Create.ForeColor = System.Drawing.Color.White;
            this.btn_Create.Location = new System.Drawing.Point(183, 196);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(150, 32);
            this.btn_Create.TabIndex = 5;
            this.btn_Create.Text = "导出配置文件";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // textBoxPreSetName
            // 
            this.textBoxPreSetName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPreSetName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxPreSetName.Location = new System.Drawing.Point(1, 1);
            this.textBoxPreSetName.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxPreSetName.Name = "textBoxPreSetName";
            this.textBoxPreSetName.Size = new System.Drawing.Size(221, 22);
            this.textBoxPreSetName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(10, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 21);
            this.label1.TabIndex = 7;
            this.label1.Text = "打印机中预设的配置文件名:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(122, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 21);
            this.label2.TabIndex = 11;
            this.label2.Text = "导出文件名:";
            // 
            // textBoxExportName
            // 
            this.textBoxExportName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxExportName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxExportName.Location = new System.Drawing.Point(1, 1);
            this.textBoxExportName.Name = "textBoxExportName";
            this.textBoxExportName.Size = new System.Drawing.Size(221, 22);
            this.textBoxExportName.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.printerList);
            this.panel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(13, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 31);
            this.panel1.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.PopupBtn_Close);
            this.panel2.Controls.Add(this.btn_Create);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(517, 243);
            this.panel2.TabIndex = 13;
            // 
            // PopupBtn_Close
            // 
            this.PopupBtn_Close.BackColor = System.Drawing.Color.Transparent;
            this.PopupBtn_Close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PopupBtn_Close.BackgroundImage")));
            this.PopupBtn_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PopupBtn_Close.FlatAppearance.BorderSize = 0;
            this.PopupBtn_Close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PopupBtn_Close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PopupBtn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PopupBtn_Close.Location = new System.Drawing.Point(487, 3);
            this.PopupBtn_Close.Name = "PopupBtn_Close";
            this.PopupBtn_Close.Size = new System.Drawing.Size(27, 27);
            this.PopupBtn_Close.TabIndex = 15;
            this.PopupBtn_Close.UseVisualStyleBackColor = false;
            this.PopupBtn_Close.Click += new System.EventHandler(this.PopupBtn_Close_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel4.Controls.Add(this.textBoxPreSetName);
            this.panel4.Location = new System.Drawing.Point(220, 104);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(223, 24);
            this.panel4.TabIndex = 14;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel3.Controls.Add(this.textBoxExportName);
            this.panel3.Location = new System.Drawing.Point(221, 150);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(223, 24);
            this.panel3.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label3.Location = new System.Drawing.Point(449, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 21);
            this.label3.TabIndex = 12;
            this.label3.Text = "属性...";
            this.label3.Click += new System.EventHandler(this.documentPropButton_Click);
            // 
            // CreatePrinterExSettingFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(212)))), ((int)(((byte)(253)))));
            this.ClientSize = new System.Drawing.Size(542, 268);
            this.Controls.Add(this.panel2);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "CreatePrinterExSettingFrm";
            this.Text = "新建打印机配置文件";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox printerList;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.TextBox textBoxPreSetName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxExportName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button PopupBtn_Close;
    }
}