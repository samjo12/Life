
namespace Class_teach
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.individ_count_label = new System.Windows.Forms.Label();
            this.family_count_label = new System.Windows.Forms.Label();
            this.start_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // individ_count_label
            // 
            this.individ_count_label.AutoSize = true;
            this.individ_count_label.Location = new System.Drawing.Point(7, 9);
            this.individ_count_label.Name = "individ_count_label";
            this.individ_count_label.Size = new System.Drawing.Size(71, 15);
            this.individ_count_label.TabIndex = 2;
            this.individ_count_label.Text = "Индивидов:";
            // 
            // family_count_label
            // 
            this.family_count_label.AutoSize = true;
            this.family_count_label.Location = new System.Drawing.Point(7, 50);
            this.family_count_label.Name = "family_count_label";
            this.family_count_label.Size = new System.Drawing.Size(46, 15);
            this.family_count_label.TabIndex = 3;
            this.family_count_label.Text = "Семей:";
            // 
            // start_btn
            // 
            this.start_btn.Location = new System.Drawing.Point(13, 103);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(75, 23);
            this.start_btn.TabIndex = 4;
            this.start_btn.Text = "Начать";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 961);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.family_count_label);
            this.Controls.Add(this.individ_count_label);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label individ_count_label;
        private System.Windows.Forms.Label family_count_label;
        private System.Windows.Forms.Button start_btn;
    }
}

