namespace PrintUtils
{
    partial class PrinterSetFrm
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.exportPrinterSetting = new System.Windows.Forms.Button();
            this.printerList = new System.Windows.Forms.ComboBox();
            this.documentPropButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(173, 90);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(76, 32);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "打印";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // exportPrinterSetting
            // 
            this.exportPrinterSetting.Location = new System.Drawing.Point(52, 90);
            this.exportPrinterSetting.Name = "exportPrinterSetting";
            this.exportPrinterSetting.Size = new System.Drawing.Size(115, 32);
            this.exportPrinterSetting.TabIndex = 37;
            this.exportPrinterSetting.Text = "导出打印机设置";
            this.exportPrinterSetting.UseVisualStyleBackColor = true;
            this.exportPrinterSetting.Click += new System.EventHandler(this.exportPrinterSetting_Click);
            // 
            // printerList
            // 
            this.printerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printerList.FormattingEnabled = true;
            this.printerList.Location = new System.Drawing.Point(20, 12);
            this.printerList.Name = "printerList";
            this.printerList.Size = new System.Drawing.Size(321, 20);
            this.printerList.TabIndex = 39;
            // 
            // documentPropButton
            // 
            this.documentPropButton.Location = new System.Drawing.Point(347, 11);
            this.documentPropButton.Name = "documentPropButton";
            this.documentPropButton.Size = new System.Drawing.Size(65, 21);
            this.documentPropButton.TabIndex = 38;
            this.documentPropButton.Text = "属性...";
            this.documentPropButton.UseVisualStyleBackColor = true;
            this.documentPropButton.Click += new System.EventHandler(this.documentPropButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(336, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 32);
            this.button1.TabIndex = 40;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(277, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 42;
            this.label1.Text = "份数:";
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.Location = new System.Drawing.Point(318, 50);
            this.numericUpDownCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCopies.Name = "numericUpDownCopies";
            this.numericUpDownCopies.Size = new System.Drawing.Size(94, 21);
            this.numericUpDownCopies.TabIndex = 43;
            this.numericUpDownCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(254, 90);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(76, 32);
            this.btnOk.TabIndex = 44;
            this.btnOk.Text = "设置";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // PrinterSetFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(430, 138);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.numericUpDownCopies);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.printerList);
            this.Controls.Add(this.documentPropButton);
            this.Controls.Add(this.exportPrinterSetting);
            this.Controls.Add(this.btnPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PrinterSetFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "打印";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button exportPrinterSetting;
        private System.Windows.Forms.ComboBox printerList;
        private System.Windows.Forms.Button documentPropButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownCopies;
        private System.Windows.Forms.Button btnOk;
    }
}