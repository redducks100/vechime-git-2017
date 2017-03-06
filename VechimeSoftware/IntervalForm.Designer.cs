namespace VechimeSoftware
{
    partial class Interval
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
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.inceputTimePicker = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.sfarsitTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.labelIntervalInfo = new System.Windows.Forms.Label();
            this.buttonContinuati = new System.Windows.Forms.Button();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 1, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(12, 46);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(514, 46);
            this.tableLayoutPanel6.TabIndex = 1;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.20717F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.79283F));
            this.tableLayoutPanel7.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.inceputTimePicker, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(251, 40);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Data inceput:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inceputTimePicker
            // 
            this.inceputTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.inceputTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.inceputTimePicker.Location = new System.Drawing.Point(123, 10);
            this.inceputTimePicker.Name = "inceputTimePicker";
            this.inceputTimePicker.Size = new System.Drawing.Size(125, 20);
            this.inceputTimePicker.TabIndex = 3;
            this.inceputTimePicker.ValueChanged += new System.EventHandler(this.inceputTimePicker_ValueChanged);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Controls.Add(this.sfarsitTimePicker, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(260, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(251, 40);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // sfarsitTimePicker
            // 
            this.sfarsitTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.sfarsitTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.sfarsitTimePicker.Location = new System.Drawing.Point(128, 10);
            this.sfarsitTimePicker.Name = "sfarsitTimePicker";
            this.sfarsitTimePicker.Size = new System.Drawing.Size(120, 20);
            this.sfarsitTimePicker.TabIndex = 4;
            this.sfarsitTimePicker.ValueChanged += new System.EventHandler(this.inceputTimePicker_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data sfarsit:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelIntervalInfo
            // 
            this.labelIntervalInfo.AutoSize = true;
            this.labelIntervalInfo.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIntervalInfo.Location = new System.Drawing.Point(71, 9);
            this.labelIntervalInfo.Name = "labelIntervalInfo";
            this.labelIntervalInfo.Size = new System.Drawing.Size(397, 23);
            this.labelIntervalInfo.TabIndex = 2;
            this.labelIntervalInfo.Text = "Selectati intervalul in care vor fi incluse perioadele.";
            // 
            // buttonContinuati
            // 
            this.buttonContinuati.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonContinuati.Location = new System.Drawing.Point(379, 107);
            this.buttonContinuati.Name = "buttonContinuati";
            this.buttonContinuati.Size = new System.Drawing.Size(115, 28);
            this.buttonContinuati.TabIndex = 5;
            this.buttonContinuati.Text = "Continuati";
            this.buttonContinuati.UseVisualStyleBackColor = true;
            this.buttonContinuati.Click += new System.EventHandler(this.buttonContinuati_Click);
            // 
            // Interval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 142);
            this.Controls.Add(this.buttonContinuati);
            this.Controls.Add(this.labelIntervalInfo);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Name = "Interval";
            this.Text = "IntervalForm";
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker inceputTimePicker;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.DateTimePicker sfarsitTimePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelIntervalInfo;
        private System.Windows.Forms.Button buttonContinuati;
    }
}
