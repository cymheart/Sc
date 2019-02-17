namespace PrintUtils
{
    partial class CreatePrinterPreSettingFrm
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
            this.documentPropButton = new System.Windows.Forms.Button();
            this.printerList = new System.Windows.Forms.ComboBox();
            this.txtRegPath = new System.Windows.Forms.TextBox();
            this.btnGetRegPath = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // documentPropButton
            // 
            this.documentPropButton.Location = new System.Drawing.Point(348, 11);
            this.documentPropButton.Name = "documentPropButton";
            this.documentPropButton.Size = new System.Drawing.Size(65, 20);
            this.documentPropButton.TabIndex = 0;
            this.documentPropButton.Text = "属性...";
            this.documentPropButton.UseVisualStyleBackColor = true;
            this.documentPropButton.Click += new System.EventHandler(this.documentPropButton_Click);
            // 
            // printerList
            // 
            this.printerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printerList.FormattingEnabled = true;
            this.printerList.Location = new System.Drawing.Point(21, 12);
            this.printerList.Name = "printerList";
            this.printerList.Size = new System.Drawing.Size(321, 20);
            this.printerList.TabIndex = 2;
            // 
            // txtRegPath
            // 
            this.txtRegPath.Location = new System.Drawing.Point(21, 38);
            this.txtRegPath.Multiline = true;
            this.txtRegPath.Name = "txtRegPath";
            this.txtRegPath.Size = new System.Drawing.Size(392, 108);
            this.txtRegPath.TabIndex = 3;
            // 
            // btnGetRegPath
            // 
            this.btnGetRegPath.Location = new System.Drawing.Point(54, 152);
            this.btnGetRegPath.Name = "btnGetRegPath";
            this.btnGetRegPath.Size = new System.Drawing.Size(234, 23);
            this.btnGetRegPath.TabIndex = 4;
            this.btnGetRegPath.Text = "获取打印机配置文件注册表路径";
            this.btnGetRegPath.UseVisualStyleBackColor = true;
            this.btnGetRegPath.Click += new System.EventHandler(this.btnGetRegPath_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(294, 152);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(119, 23);
            this.btnCreate.TabIndex = 5;
            this.btnCreate.Text = "生成配置文件";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // CreatePrinterPreSettingFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(430, 191);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnGetRegPath);
            this.Controls.Add(this.txtRegPath);
            this.Controls.Add(this.printerList);
            this.Controls.Add(this.documentPropButton);
            this.Name = "CreatePrinterPreSettingFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "生成打印机配置文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button documentPropButton;
        private System.Windows.Forms.ComboBox printerList;
        private System.Windows.Forms.TextBox txtRegPath;
        private System.Windows.Forms.Button btnGetRegPath;
        private System.Windows.Forms.Button btnCreate;
    }
}