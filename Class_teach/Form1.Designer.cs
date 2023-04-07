
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
            individ_count_label = new System.Windows.Forms.Label();
            family_count_label = new System.Windows.Forms.Label();
            start_btn = new System.Windows.Forms.Button();
            tbPeople = new System.Windows.Forms.TextBox();
            tbStatFam = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            nudCells = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            nudPeople = new System.Windows.Forms.NumericUpDown();
            tbStatInfants = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            tbStatTeens = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tbStatAdults = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tbStatOldMans = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            tbStatBattles = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            tbStatKills = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            tbStatDeath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)nudCells).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPeople).BeginInit();
            SuspendLayout();
            // 
            // individ_count_label
            // 
            individ_count_label.AutoSize = true;
            individ_count_label.Location = new System.Drawing.Point(7, 266);
            individ_count_label.Name = "individ_count_label";
            individ_count_label.Size = new System.Drawing.Size(71, 15);
            individ_count_label.TabIndex = 2;
            individ_count_label.Text = "Индивидов:";
            // 
            // family_count_label
            // 
            family_count_label.AutoSize = true;
            family_count_label.Location = new System.Drawing.Point(7, 516);
            family_count_label.Name = "family_count_label";
            family_count_label.Size = new System.Drawing.Size(46, 15);
            family_count_label.TabIndex = 3;
            family_count_label.Text = "Семей:";
            // 
            // start_btn
            // 
            start_btn.Location = new System.Drawing.Point(7, 3);
            start_btn.Name = "start_btn";
            start_btn.Size = new System.Drawing.Size(71, 23);
            start_btn.TabIndex = 4;
            start_btn.Text = "Начать";
            start_btn.UseVisualStyleBackColor = true;
            start_btn.Click += start_btn_Click;
            // 
            // tbPeople
            // 
            tbPeople.Location = new System.Drawing.Point(7, 283);
            tbPeople.Name = "tbPeople";
            tbPeople.Size = new System.Drawing.Size(88, 23);
            tbPeople.TabIndex = 5;
            // 
            // tbStatFam
            // 
            tbStatFam.Location = new System.Drawing.Point(7, 534);
            tbStatFam.Name = "tbStatFam";
            tbStatFam.Size = new System.Drawing.Size(88, 23);
            tbStatFam.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 312);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 6;
            label1.Text = "Детей:";
            // 
            // nudCells
            // 
            nudCells.Location = new System.Drawing.Point(7, 51);
            nudCells.Name = "nudCells";
            nudCells.Size = new System.Drawing.Size(88, 23);
            nudCells.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 33);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(79, 15);
            label2.TabIndex = 8;
            label2.Text = "Число ячеек:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(4, 77);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(86, 15);
            label3.TabIndex = 9;
            label3.Text = "Первые люди:";
            // 
            // nudPeople
            // 
            nudPeople.Location = new System.Drawing.Point(7, 95);
            nudPeople.Name = "nudPeople";
            nudPeople.Size = new System.Drawing.Size(88, 23);
            nudPeople.TabIndex = 10;
            nudPeople.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tbStatInfants
            // 
            tbStatInfants.Location = new System.Drawing.Point(7, 331);
            tbStatInfants.Name = "tbStatInfants";
            tbStatInfants.Size = new System.Drawing.Size(88, 23);
            tbStatInfants.TabIndex = 11;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 357);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(76, 15);
            label4.TabIndex = 2;
            label4.Text = "Подростков:";
            // 
            // tbStatTeens
            // 
            tbStatTeens.Location = new System.Drawing.Point(7, 375);
            tbStatTeens.Name = "tbStatTeens";
            tbStatTeens.Size = new System.Drawing.Size(88, 23);
            tbStatTeens.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(7, 401);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(64, 15);
            label5.TabIndex = 6;
            label5.Text = "Взрослых:";
            // 
            // tbStatAdults
            // 
            tbStatAdults.Location = new System.Drawing.Point(7, 419);
            tbStatAdults.Name = "tbStatAdults";
            tbStatAdults.Size = new System.Drawing.Size(88, 23);
            tbStatAdults.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(7, 445);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(62, 15);
            label6.TabIndex = 6;
            label6.Text = "Стариков:";
            // 
            // tbStatOldMans
            // 
            tbStatOldMans.Location = new System.Drawing.Point(7, 463);
            tbStatOldMans.Name = "tbStatOldMans";
            tbStatOldMans.Size = new System.Drawing.Size(88, 23);
            tbStatOldMans.TabIndex = 11;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(7, 588);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(44, 15);
            label7.TabIndex = 3;
            label7.Text = "Драки:";
            // 
            // tbStatBattles
            // 
            tbStatBattles.Location = new System.Drawing.Point(7, 606);
            tbStatBattles.Name = "tbStatBattles";
            tbStatBattles.Size = new System.Drawing.Size(88, 23);
            tbStatBattles.TabIndex = 5;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(7, 632);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(61, 15);
            label8.TabIndex = 3;
            label8.Text = "Убийства:";
            // 
            // tbStatKills
            // 
            tbStatKills.Location = new System.Drawing.Point(7, 650);
            tbStatKills.Name = "tbStatKills";
            tbStatKills.Size = new System.Drawing.Size(88, 23);
            tbStatKills.TabIndex = 5;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(7, 674);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(52, 15);
            label9.TabIndex = 3;
            label9.Text = "Смерти:";
            // 
            // tbStatDeath
            // 
            tbStatDeath.Location = new System.Drawing.Point(7, 692);
            tbStatDeath.Name = "tbStatDeath";
            tbStatDeath.Size = new System.Drawing.Size(88, 23);
            tbStatDeath.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1784, 961);
            Controls.Add(tbStatOldMans);
            Controls.Add(tbStatAdults);
            Controls.Add(tbStatInfants);
            Controls.Add(nudPeople);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(label2);
            Controls.Add(label5);
            Controls.Add(nudCells);
            Controls.Add(label1);
            Controls.Add(tbStatTeens);
            Controls.Add(tbStatDeath);
            Controls.Add(tbStatKills);
            Controls.Add(tbStatBattles);
            Controls.Add(tbStatFam);
            Controls.Add(label9);
            Controls.Add(tbPeople);
            Controls.Add(label8);
            Controls.Add(start_btn);
            Controls.Add(label7);
            Controls.Add(label4);
            Controls.Add(family_count_label);
            Controls.Add(individ_count_label);
            Name = "Form1";
            Text = "Form1";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            MouseDown += Form1_MouseDown;
            ((System.ComponentModel.ISupportInitialize)nudCells).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPeople).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label individ_count_label;
        private System.Windows.Forms.Label family_count_label;
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.TextBox tbPeople;
        private System.Windows.Forms.TextBox tbStatFam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudCells;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudPeople;
        private System.Windows.Forms.TextBox tbStatInfants;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbStatTeens;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbStatAdults;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbStatOldMans;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbStatBattles;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbStatKills;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbStatDeath;
    }
}

