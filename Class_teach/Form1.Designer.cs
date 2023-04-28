
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
            this.tbStatPeople = new System.Windows.Forms.TextBox();
            this.tbStatFam = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudCells = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudPeople = new System.Windows.Forms.NumericUpDown();
            this.tbStatInfants = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbStatTeens = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbStatAdults = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbStatOldMans = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbStatBattles = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbStatKills = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbStatDeath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbStatMoves = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbStatMales = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbStatFemales = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.nudSpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudCells)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeople)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // individ_count_label
            // 
            this.individ_count_label.AutoSize = true;
            this.individ_count_label.Location = new System.Drawing.Point(7, 401);
            this.individ_count_label.Name = "individ_count_label";
            this.individ_count_label.Size = new System.Drawing.Size(71, 15);
            this.individ_count_label.TabIndex = 2;
            this.individ_count_label.Text = "Индивидов:";
            // 
            // family_count_label
            // 
            this.family_count_label.AutoSize = true;
            this.family_count_label.Location = new System.Drawing.Point(7, 742);
            this.family_count_label.Name = "family_count_label";
            this.family_count_label.Size = new System.Drawing.Size(46, 15);
            this.family_count_label.TabIndex = 3;
            this.family_count_label.Text = "Семей:";
            // 
            // start_btn
            // 
            this.start_btn.Location = new System.Drawing.Point(7, 3);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(88, 23);
            this.start_btn.TabIndex = 4;
            this.start_btn.Text = "Старт/Стоп";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // tbStatPeople
            // 
            this.tbStatPeople.Location = new System.Drawing.Point(7, 418);
            this.tbStatPeople.Name = "tbStatPeople";
            this.tbStatPeople.Size = new System.Drawing.Size(88, 23);
            this.tbStatPeople.TabIndex = 5;
            // 
            // tbStatFam
            // 
            this.tbStatFam.Location = new System.Drawing.Point(7, 760);
            this.tbStatFam.Name = "tbStatFam";
            this.tbStatFam.Size = new System.Drawing.Size(88, 23);
            this.tbStatFam.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 535);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Детей:";
            // 
            // nudCells
            // 
            this.nudCells.Location = new System.Drawing.Point(7, 51);
            this.nudCells.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudCells.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCells.Name = "nudCells";
            this.nudCells.Size = new System.Drawing.Size(88, 23);
            this.nudCells.TabIndex = 7;
            this.nudCells.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCells.ValueChanged += new System.EventHandler(this.nudCells_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Размер ячейки:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Первые люди:";
            // 
            // nudPeople
            // 
            this.nudPeople.Location = new System.Drawing.Point(7, 95);
            this.nudPeople.Name = "nudPeople";
            this.nudPeople.Size = new System.Drawing.Size(88, 23);
            this.nudPeople.TabIndex = 10;
            this.nudPeople.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // tbStatInfants
            // 
            this.tbStatInfants.Location = new System.Drawing.Point(7, 554);
            this.tbStatInfants.Name = "tbStatInfants";
            this.tbStatInfants.Size = new System.Drawing.Size(88, 23);
            this.tbStatInfants.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 580);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Подростков:";
            // 
            // tbStatTeens
            // 
            this.tbStatTeens.Location = new System.Drawing.Point(7, 598);
            this.tbStatTeens.Name = "tbStatTeens";
            this.tbStatTeens.Size = new System.Drawing.Size(88, 23);
            this.tbStatTeens.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 624);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Взрослых:";
            // 
            // tbStatAdults
            // 
            this.tbStatAdults.Location = new System.Drawing.Point(7, 642);
            this.tbStatAdults.Name = "tbStatAdults";
            this.tbStatAdults.Size = new System.Drawing.Size(88, 23);
            this.tbStatAdults.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 668);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Стариков:";
            // 
            // tbStatOldMans
            // 
            this.tbStatOldMans.Location = new System.Drawing.Point(7, 686);
            this.tbStatOldMans.Name = "tbStatOldMans";
            this.tbStatOldMans.Size = new System.Drawing.Size(88, 23);
            this.tbStatOldMans.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 814);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 15);
            this.label7.TabIndex = 3;
            this.label7.Text = "Драки:";
            // 
            // tbStatBattles
            // 
            this.tbStatBattles.Location = new System.Drawing.Point(7, 832);
            this.tbStatBattles.Name = "tbStatBattles";
            this.tbStatBattles.Size = new System.Drawing.Size(88, 23);
            this.tbStatBattles.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 858);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "Убийства:";
            // 
            // tbStatKills
            // 
            this.tbStatKills.Location = new System.Drawing.Point(7, 876);
            this.tbStatKills.Name = "tbStatKills";
            this.tbStatKills.Size = new System.Drawing.Size(88, 23);
            this.tbStatKills.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 900);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Смерти:";
            // 
            // tbStatDeath
            // 
            this.tbStatDeath.Location = new System.Drawing.Point(7, 918);
            this.tbStatDeath.Name = "tbStatDeath";
            this.tbStatDeath.Size = new System.Drawing.Size(88, 23);
            this.tbStatDeath.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 356);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 15);
            this.label10.TabIndex = 13;
            this.label10.Text = "Ход";
            // 
            // tbStatMoves
            // 
            this.tbStatMoves.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.tbStatMoves.Location = new System.Drawing.Point(7, 375);
            this.tbStatMoves.Name = "tbStatMoves";
            this.tbStatMoves.Size = new System.Drawing.Size(88, 23);
            this.tbStatMoves.TabIndex = 14;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 444);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 15);
            this.label11.TabIndex = 15;
            this.label11.Text = "Мужчин";
            // 
            // tbStatMales
            // 
            this.tbStatMales.Location = new System.Drawing.Point(7, 462);
            this.tbStatMales.Name = "tbStatMales";
            this.tbStatMales.Size = new System.Drawing.Size(88, 23);
            this.tbStatMales.TabIndex = 16;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 488);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 15);
            this.label12.TabIndex = 17;
            this.label12.Text = "Женщин";
            // 
            // tbStatFemales
            // 
            this.tbStatFemales.Location = new System.Drawing.Point(7, 507);
            this.tbStatFemales.Name = "tbStatFemales";
            this.tbStatFemales.Size = new System.Drawing.Size(88, 23);
            this.tbStatFemales.TabIndex = 18;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 304);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 15);
            this.label13.TabIndex = 19;
            this.label13.Text = "Скорость";
            // 
            // nudSpeed
            // 
            this.nudSpeed.BackColor = System.Drawing.SystemColors.Info;
            this.nudSpeed.Location = new System.Drawing.Point(7, 322);
            this.nudSpeed.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeed.Name = "nudSpeed";
            this.nudSpeed.Size = new System.Drawing.Size(88, 23);
            this.nudSpeed.TabIndex = 20;
            this.nudSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeed.ValueChanged += new System.EventHandler(this.nudSpeed_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 961);
            this.Controls.Add(this.nudSpeed);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tbStatFemales);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.tbStatMales);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tbStatMoves);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbStatOldMans);
            this.Controls.Add(this.tbStatAdults);
            this.Controls.Add(this.tbStatInfants);
            this.Controls.Add(this.nudPeople);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudCells);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStatTeens);
            this.Controls.Add(this.tbStatDeath);
            this.Controls.Add(this.tbStatKills);
            this.Controls.Add(this.tbStatBattles);
            this.Controls.Add(this.tbStatFam);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbStatPeople);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.start_btn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.family_count_label);
            this.Controls.Add(this.individ_count_label);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.nudCells)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPeople)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label individ_count_label;
        private System.Windows.Forms.Label family_count_label;
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.TextBox tbStatPeople;
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbStatMoves;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbStatMales;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbStatFemales;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudSpeed;
    }
}

