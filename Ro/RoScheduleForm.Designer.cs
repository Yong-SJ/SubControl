namespace SubControl.Ro
{
    partial class RoScheduleForm
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
            if (disposing)
            {
                // 타이머 정리
                if (PageRefreshTimer != null)
                {
                    PageRefreshTimer.Stop();
                    PageRefreshTimer.Dispose();
                    PageRefreshTimer = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoScheduleForm));
            this.Copy_bt = new System.Windows.Forms.Button();
            this.Change_bt = new System.Windows.Forms.Button();
            this.Schedule1_label = new System.Windows.Forms.Label();
            this.BeforePage_bt = new System.Windows.Forms.Button();
            this.AfterPage_bt = new System.Windows.Forms.Button();
            this.Schedule2_label = new System.Windows.Forms.Label();
            this.Schedule4_label = new System.Windows.Forms.Label();
            this.Schedule3_label = new System.Windows.Forms.Label();
            this.Schedule6_label = new System.Windows.Forms.Label();
            this.Schedule5_label = new System.Windows.Forms.Label();
            this.Schedule8_label = new System.Windows.Forms.Label();
            this.Schedule7_label = new System.Windows.Forms.Label();
            this.Schedule10_label = new System.Windows.Forms.Label();
            this.Schedule9_label = new System.Windows.Forms.Label();
            this.SetPage_label = new System.Windows.Forms.Label();
            this.Page_label = new System.Windows.Forms.Label();
            this.Move_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Copy_bt
            // 
            this.Copy_bt.FlatAppearance.BorderSize = 0;
            this.Copy_bt.Font = new System.Drawing.Font("Y이드스트릿체 B", 10F, System.Drawing.FontStyle.Bold);
            this.Copy_bt.Location = new System.Drawing.Point(967, 640);
            this.Copy_bt.Name = "Copy_bt";
            this.Copy_bt.Size = new System.Drawing.Size(65, 65);
            this.Copy_bt.TabIndex = 0;
            this.Copy_bt.TabStop = false;
            this.Copy_bt.Text = "Copy";
            this.Copy_bt.UseVisualStyleBackColor = true;
            this.Copy_bt.Click += new System.EventHandler(this.Copy_bt_Click);
            // 
            // Change_bt
            // 
            this.Change_bt.FlatAppearance.BorderSize = 0;
            this.Change_bt.Font = new System.Drawing.Font("Y이드스트릿체 B", 10F, System.Drawing.FontStyle.Bold);
            this.Change_bt.Location = new System.Drawing.Point(1069, 640);
            this.Change_bt.Name = "Change_bt";
            this.Change_bt.Size = new System.Drawing.Size(65, 65);
            this.Change_bt.TabIndex = 1;
            this.Change_bt.TabStop = false;
            this.Change_bt.Text = "Change";
            this.Change_bt.UseVisualStyleBackColor = true;
            this.Change_bt.Click += new System.EventHandler(this.Change_bt_Click);
            // 
            // Schedule1_label
            // 
            this.Schedule1_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule1_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule1_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule1_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule1_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule1_label.Location = new System.Drawing.Point(47, 134);
            this.Schedule1_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule1_label.Name = "Schedule1_label";
            this.Schedule1_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule1_label.TabIndex = 147;
            this.Schedule1_label.Tag = "1D_16";
            this.Schedule1_label.Text = "0";
            this.Schedule1_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BeforePage_bt
            // 
            this.BeforePage_bt.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.BeforePage_bt.Location = new System.Drawing.Point(443, 640);
            this.BeforePage_bt.Name = "BeforePage_bt";
            this.BeforePage_bt.Size = new System.Drawing.Size(65, 65);
            this.BeforePage_bt.TabIndex = 148;
            this.BeforePage_bt.TabStop = false;
            this.BeforePage_bt.Text = "<";
            this.BeforePage_bt.UseVisualStyleBackColor = true;
            this.BeforePage_bt.Click += new System.EventHandler(this.BeforePage_Click);
            // 
            // AfterPage_bt
            // 
            this.AfterPage_bt.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.AfterPage_bt.Location = new System.Drawing.Point(675, 640);
            this.AfterPage_bt.Name = "AfterPage_bt";
            this.AfterPage_bt.Size = new System.Drawing.Size(65, 65);
            this.AfterPage_bt.TabIndex = 149;
            this.AfterPage_bt.TabStop = false;
            this.AfterPage_bt.Text = ">";
            this.AfterPage_bt.UseVisualStyleBackColor = true;
            this.AfterPage_bt.Click += new System.EventHandler(this.AfterPage_Click);
            // 
            // Schedule2_label
            // 
            this.Schedule2_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule2_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule2_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule2_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule2_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule2_label.Location = new System.Drawing.Point(627, 134);
            this.Schedule2_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule2_label.Name = "Schedule2_label";
            this.Schedule2_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule2_label.TabIndex = 150;
            this.Schedule2_label.Tag = "0_180_320_60_0";
            this.Schedule2_label.Text = "0";
            this.Schedule2_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule4_label
            // 
            this.Schedule4_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule4_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule4_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule4_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule4_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule4_label.Location = new System.Drawing.Point(627, 230);
            this.Schedule4_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule4_label.Name = "Schedule4_label";
            this.Schedule4_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule4_label.TabIndex = 152;
            this.Schedule4_label.Tag = "0_180_320_60_0";
            this.Schedule4_label.Text = "0";
            this.Schedule4_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule3_label
            // 
            this.Schedule3_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule3_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule3_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule3_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule3_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule3_label.Location = new System.Drawing.Point(47, 230);
            this.Schedule3_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule3_label.Name = "Schedule3_label";
            this.Schedule3_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule3_label.TabIndex = 151;
            this.Schedule3_label.Tag = "0_180_320_60_0";
            this.Schedule3_label.Text = "0";
            this.Schedule3_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule6_label
            // 
            this.Schedule6_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule6_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule6_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule6_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule6_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule6_label.Location = new System.Drawing.Point(629, 326);
            this.Schedule6_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule6_label.Name = "Schedule6_label";
            this.Schedule6_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule6_label.TabIndex = 154;
            this.Schedule6_label.Tag = "0_180_320_60_0";
            this.Schedule6_label.Text = "0";
            this.Schedule6_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule5_label
            // 
            this.Schedule5_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule5_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule5_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule5_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule5_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule5_label.Location = new System.Drawing.Point(49, 326);
            this.Schedule5_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule5_label.Name = "Schedule5_label";
            this.Schedule5_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule5_label.TabIndex = 153;
            this.Schedule5_label.Tag = "0_180_320_60_0";
            this.Schedule5_label.Text = "0";
            this.Schedule5_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule8_label
            // 
            this.Schedule8_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule8_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule8_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule8_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule8_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule8_label.Location = new System.Drawing.Point(627, 422);
            this.Schedule8_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule8_label.Name = "Schedule8_label";
            this.Schedule8_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule8_label.TabIndex = 156;
            this.Schedule8_label.Tag = "0_180_320_60_0";
            this.Schedule8_label.Text = "0";
            this.Schedule8_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule7_label
            // 
            this.Schedule7_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule7_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule7_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule7_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule7_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule7_label.Location = new System.Drawing.Point(47, 422);
            this.Schedule7_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule7_label.Name = "Schedule7_label";
            this.Schedule7_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule7_label.TabIndex = 155;
            this.Schedule7_label.Tag = "0_180_320_60_0";
            this.Schedule7_label.Text = "0";
            this.Schedule7_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule10_label
            // 
            this.Schedule10_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule10_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule10_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule10_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule10_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule10_label.Location = new System.Drawing.Point(629, 518);
            this.Schedule10_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule10_label.Name = "Schedule10_label";
            this.Schedule10_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule10_label.TabIndex = 158;
            this.Schedule10_label.Tag = "0_180_320_60_0";
            this.Schedule10_label.Text = "0";
            this.Schedule10_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Schedule9_label
            // 
            this.Schedule9_label.BackColor = System.Drawing.Color.Transparent;
            this.Schedule9_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Schedule9_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Schedule9_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 12F, System.Drawing.FontStyle.Bold);
            this.Schedule9_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Schedule9_label.Location = new System.Drawing.Point(49, 518);
            this.Schedule9_label.Margin = new System.Windows.Forms.Padding(1);
            this.Schedule9_label.Name = "Schedule9_label";
            this.Schedule9_label.Size = new System.Drawing.Size(507, 70);
            this.Schedule9_label.TabIndex = 157;
            this.Schedule9_label.Tag = "0_180_320_60_0";
            this.Schedule9_label.Text = "0";
            this.Schedule9_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SetPage_label
            // 
            this.SetPage_label.BackColor = System.Drawing.Color.Transparent;
            this.SetPage_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SetPage_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 20F, System.Drawing.FontStyle.Bold);
            this.SetPage_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.SetPage_label.Location = new System.Drawing.Point(529, 637);
            this.SetPage_label.Margin = new System.Windows.Forms.Padding(1);
            this.SetPage_label.Name = "SetPage_label";
            this.SetPage_label.Size = new System.Drawing.Size(50, 71);
            this.SetPage_label.TabIndex = 159;
            this.SetPage_label.Tag = "";
            this.SetPage_label.Text = "1";
            this.SetPage_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Page_label
            // 
            this.Page_label.BackColor = System.Drawing.Color.Transparent;
            this.Page_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Page_label.Font = new System.Drawing.Font("Y이드스트릿체 B", 20F, System.Drawing.FontStyle.Bold);
            this.Page_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.Page_label.Location = new System.Drawing.Point(605, 637);
            this.Page_label.Margin = new System.Windows.Forms.Padding(1);
            this.Page_label.Name = "Page_label";
            this.Page_label.Size = new System.Drawing.Size(50, 71);
            this.Page_label.TabIndex = 159;
            this.Page_label.Tag = "";
            this.Page_label.Text = "5";
            this.Page_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Move_bt
            // 
            this.Move_bt.FlatAppearance.BorderSize = 0;
            this.Move_bt.Font = new System.Drawing.Font("Y이드스트릿체 B", 10F, System.Drawing.FontStyle.Bold);
            this.Move_bt.Location = new System.Drawing.Point(869, 640);
            this.Move_bt.Name = "Move_bt";
            this.Move_bt.Size = new System.Drawing.Size(65, 65);
            this.Move_bt.TabIndex = 0;
            this.Move_bt.TabStop = false;
            this.Move_bt.Text = "Move";
            this.Move_bt.UseVisualStyleBackColor = true;
            this.Move_bt.Click += new System.EventHandler(this.Move_bt_Click);
            // 
            // RoScheduleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1184, 721);
            this.Controls.Add(this.Page_label);
            this.Controls.Add(this.SetPage_label);
            this.Controls.Add(this.Schedule10_label);
            this.Controls.Add(this.Schedule9_label);
            this.Controls.Add(this.Schedule8_label);
            this.Controls.Add(this.Schedule7_label);
            this.Controls.Add(this.Schedule6_label);
            this.Controls.Add(this.Schedule5_label);
            this.Controls.Add(this.Schedule4_label);
            this.Controls.Add(this.Schedule3_label);
            this.Controls.Add(this.Schedule2_label);
            this.Controls.Add(this.AfterPage_bt);
            this.Controls.Add(this.BeforePage_bt);
            this.Controls.Add(this.Schedule1_label);
            this.Controls.Add(this.Change_bt);
            this.Controls.Add(this.Move_bt);
            this.Controls.Add(this.Copy_bt);
            this.DoubleBuffered = true;
            this.Name = "RoScheduleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RoScheduleForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Copy_bt;
        private System.Windows.Forms.Button Change_bt;
        public System.Windows.Forms.Label Schedule1_label;
        private System.Windows.Forms.Button BeforePage_bt;
        private System.Windows.Forms.Button AfterPage_bt;
        public System.Windows.Forms.Label Schedule2_label;
        public System.Windows.Forms.Label Schedule4_label;
        public System.Windows.Forms.Label Schedule3_label;
        public System.Windows.Forms.Label Schedule6_label;
        public System.Windows.Forms.Label Schedule5_label;
        public System.Windows.Forms.Label Schedule8_label;
        public System.Windows.Forms.Label Schedule7_label;
        public System.Windows.Forms.Label Schedule10_label;
        public System.Windows.Forms.Label Schedule9_label;
        public System.Windows.Forms.Label SetPage_label;
        public System.Windows.Forms.Label Page_label;
        private System.Windows.Forms.Button Move_bt;
    }
}