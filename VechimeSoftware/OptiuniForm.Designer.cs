namespace VechimeSoftware
{
    partial class OptiuniForm
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
            this.checkBoxStartUp = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoConnect = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxStartUp
            // 
            this.checkBoxStartUp.AutoSize = true;
            this.checkBoxStartUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxStartUp.Location = new System.Drawing.Point(44, 35);
            this.checkBoxStartUp.Name = "checkBoxStartUp";
            this.checkBoxStartUp.Size = new System.Drawing.Size(253, 20);
            this.checkBoxStartUp.TabIndex = 0;
            this.checkBoxStartUp.Text = "Permiteti aplicatiei sa ruleze la startup.";
            this.checkBoxStartUp.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoConnect
            // 
            this.checkBoxAutoConnect.AutoSize = true;
            this.checkBoxAutoConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxAutoConnect.Location = new System.Drawing.Point(44, 76);
            this.checkBoxAutoConnect.Name = "checkBoxAutoConnect";
            this.checkBoxAutoConnect.Size = new System.Drawing.Size(151, 20);
            this.checkBoxAutoConnect.TabIndex = 1;
            this.checkBoxAutoConnect.Text = "Conectare automata.";
            this.checkBoxAutoConnect.UseVisualStyleBackColor = true;
            this.checkBoxAutoConnect.CheckedChanged += new System.EventHandler(this.checkBoxAutoConnect_CheckedChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(238, 123);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "Salvare";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // OptiuniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 158);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.checkBoxAutoConnect);
            this.Controls.Add(this.checkBoxStartUp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OptiuniForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OptiuniForm";
            this.Load += new System.EventHandler(this.OptiuniForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxStartUp;
        private System.Windows.Forms.CheckBox checkBoxAutoConnect;
        private System.Windows.Forms.Button buttonSave;
    }
}