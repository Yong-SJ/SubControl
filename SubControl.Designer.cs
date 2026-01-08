namespace SubControl
{
    partial class SubControl
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.P1Read_bt = new System.Windows.Forms.Button();
            this.P1Write_bt = new System.Windows.Forms.Button();
            this.P1Connect_bt = new System.Windows.Forms.Button();
            this.P1Disconnect_bt = new System.Windows.Forms.Button();
            this.Status_label = new System.Windows.Forms.Label();
            this.P1Read_label = new System.Windows.Forms.Label();
            this.P1ReadBit_bt = new System.Windows.Forms.Button();
            this.P1WriteBit_bt = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.P1ReadBit_label = new System.Windows.Forms.Label();
            this.P1PulseMode_bt = new System.Windows.Forms.Button();
            this.P1Write_label = new System.Windows.Forms.Label();
            this.P1WriteBit_label = new System.Windows.Forms.Label();
            this.Edit_bt = new System.Windows.Forms.Button();
            this.SingleWrite_bt = new System.Windows.Forms.Button();
            this.MultiWrite_bt = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Read_bt = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.P1ReadRegister_txt = new CenteredTextBox();
            this.P1Value_txt = new CenteredTextBox();
            this.P1WriteRegister_txt = new CenteredTextBox();
            this.P1BitTureFalse_txt = new CenteredTextBox();
            this.P1Port_txt = new CenteredTextBox();
            this.P1IpAddress_txt = new CenteredTextBox();
            this.ReadID_txt = new CenteredTextBox();
            this.ReadRegister_txt = new CenteredTextBox();
            this.ReadInput_txt = new CenteredTextBox();
            this.MultiID_txt = new CenteredTextBox();
            this.SingleID_txt = new CenteredTextBox();
            this.MultiRegister_txt = new CenteredTextBox();
            this.SingleRegister_txt = new CenteredTextBox();
            this.MultiInput_txt = new CenteredTextBox();
            this.SingleInput_txt = new CenteredTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // P1Read_bt
            // 
            this.P1Read_bt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1Read_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Read_bt.Location = new System.Drawing.Point(785, 46);
            this.P1Read_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Read_bt.Name = "P1Read_bt";
            this.P1Read_bt.Size = new System.Drawing.Size(194, 43);
            this.P1Read_bt.TabIndex = 23;
            this.P1Read_bt.Text = "Read";
            this.P1Read_bt.UseVisualStyleBackColor = true;
            this.P1Read_bt.Visible = false;
            this.P1Read_bt.Click += new System.EventHandler(this.P1Read_bt_Click);
            // 
            // P1Write_bt
            // 
            this.P1Write_bt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1Write_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Write_bt.Location = new System.Drawing.Point(785, 136);
            this.P1Write_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Write_bt.Name = "P1Write_bt";
            this.P1Write_bt.Size = new System.Drawing.Size(194, 43);
            this.P1Write_bt.TabIndex = 15;
            this.P1Write_bt.Text = "Write";
            this.P1Write_bt.UseVisualStyleBackColor = true;
            this.P1Write_bt.Visible = false;
            this.P1Write_bt.Click += new System.EventHandler(this.P1Write_bt_Click);
            // 
            // P1Connect_bt
            // 
            this.P1Connect_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Connect_bt.Location = new System.Drawing.Point(221, 617);
            this.P1Connect_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Connect_bt.Name = "P1Connect_bt";
            this.P1Connect_bt.Size = new System.Drawing.Size(148, 71);
            this.P1Connect_bt.TabIndex = 23;
            this.P1Connect_bt.Text = "Connect";
            this.P1Connect_bt.UseVisualStyleBackColor = true;
            this.P1Connect_bt.Visible = false;
            this.P1Connect_bt.Click += new System.EventHandler(this.P1Connect_bt_Click);
            // 
            // P1Disconnect_bt
            // 
            this.P1Disconnect_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Disconnect_bt.Location = new System.Drawing.Point(221, 705);
            this.P1Disconnect_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Disconnect_bt.Name = "P1Disconnect_bt";
            this.P1Disconnect_bt.Size = new System.Drawing.Size(148, 71);
            this.P1Disconnect_bt.TabIndex = 23;
            this.P1Disconnect_bt.Text = "Disconnect";
            this.P1Disconnect_bt.UseVisualStyleBackColor = true;
            this.P1Disconnect_bt.Visible = false;
            this.P1Disconnect_bt.Click += new System.EventHandler(this.P1Disconnect_bt_Click);
            // 
            // Status_label
            // 
            this.Status_label.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.Status_label.Location = new System.Drawing.Point(391, 617);
            this.Status_label.Name = "Status_label";
            this.Status_label.Size = new System.Drawing.Size(150, 59);
            this.Status_label.TabIndex = 27;
            this.Status_label.Text = "-";
            this.Status_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Status_label.Visible = false;
            // 
            // P1Read_label
            // 
            this.P1Read_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1Read_label.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Read_label.Location = new System.Drawing.Point(981, 46);
            this.P1Read_label.Margin = new System.Windows.Forms.Padding(1);
            this.P1Read_label.Name = "P1Read_label";
            this.P1Read_label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.P1Read_label.Size = new System.Drawing.Size(790, 43);
            this.P1Read_label.TabIndex = 26;
            this.P1Read_label.Text = "-";
            this.P1Read_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.P1Read_label.Visible = false;
            // 
            // P1ReadBit_bt
            // 
            this.P1ReadBit_bt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1ReadBit_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1ReadBit_bt.Location = new System.Drawing.Point(785, 91);
            this.P1ReadBit_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1ReadBit_bt.Name = "P1ReadBit_bt";
            this.P1ReadBit_bt.Size = new System.Drawing.Size(194, 43);
            this.P1ReadBit_bt.TabIndex = 28;
            this.P1ReadBit_bt.Text = "Read Bit";
            this.P1ReadBit_bt.UseVisualStyleBackColor = true;
            this.P1ReadBit_bt.Visible = false;
            this.P1ReadBit_bt.Click += new System.EventHandler(this.P1ReadBit_bt_Click);
            // 
            // P1WriteBit_bt
            // 
            this.P1WriteBit_bt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1WriteBit_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1WriteBit_bt.Location = new System.Drawing.Point(785, 181);
            this.P1WriteBit_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1WriteBit_bt.Name = "P1WriteBit_bt";
            this.P1WriteBit_bt.Size = new System.Drawing.Size(194, 44);
            this.P1WriteBit_bt.TabIndex = 15;
            this.P1WriteBit_bt.Text = "Write Bit";
            this.P1WriteBit_bt.UseVisualStyleBackColor = true;
            this.P1WriteBit_bt.Visible = false;
            this.P1WriteBit_bt.Click += new System.EventHandler(this.P1WriteBit_bt_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.44444F));
            this.tableLayoutPanel2.Controls.Add(this.label10, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.P1ReadBit_bt, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.P1Read_bt, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.P1ReadRegister_txt, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.P1Value_txt, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label16, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.P1WriteRegister_txt, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label12, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label17, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label13, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.P1Write_bt, 4, 3);
            this.tableLayoutPanel2.Controls.Add(this.P1WriteBit_bt, 4, 4);
            this.tableLayoutPanel2.Controls.Add(this.P1Read_label, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.P1ReadBit_label, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.P1PulseMode_bt, 3, 4);
            this.tableLayoutPanel2.Controls.Add(this.P1Write_label, 5, 3);
            this.tableLayoutPanel2.Controls.Add(this.P1WriteBit_label, 5, 4);
            this.tableLayoutPanel2.Controls.Add(this.P1BitTureFalse_txt, 2, 4);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(23, 794);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1772, 226);
            this.tableLayoutPanel2.TabIndex = 29;
            this.tableLayoutPanel2.Visible = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label10.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label10.Location = new System.Drawing.Point(785, 1);
            this.label10.Margin = new System.Windows.Forms.Padding(1);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(194, 43);
            this.label10.TabIndex = 10;
            this.label10.Text = "Button";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label10.Visible = false;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label8.Location = new System.Drawing.Point(1, 46);
            this.label8.Margin = new System.Windows.Forms.Padding(1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(194, 43);
            this.label8.TabIndex = 10;
            this.label8.Text = "Read";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label9.Location = new System.Drawing.Point(197, 1);
            this.label9.Margin = new System.Windows.Forms.Padding(1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(194, 43);
            this.label9.TabIndex = 10;
            this.label9.Text = "Register";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label9.Visible = false;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label14.Location = new System.Drawing.Point(1, 91);
            this.label14.Margin = new System.Windows.Forms.Padding(1);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(194, 43);
            this.label14.TabIndex = 10;
            this.label14.Text = "Read Bit";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label14.Visible = false;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label15.Location = new System.Drawing.Point(1, 136);
            this.label15.Margin = new System.Windows.Forms.Padding(1);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(194, 43);
            this.label15.TabIndex = 10;
            this.label15.Text = "Write";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label15.Visible = false;
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label16.Location = new System.Drawing.Point(1, 181);
            this.label16.Margin = new System.Windows.Forms.Padding(1);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(194, 44);
            this.label16.TabIndex = 10;
            this.label16.Text = "Write Bit";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label16.Visible = false;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label12.Location = new System.Drawing.Point(589, 1);
            this.label12.Margin = new System.Windows.Forms.Padding(1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(194, 43);
            this.label12.TabIndex = 10;
            this.label12.Text = "Mode";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label12.Visible = false;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label17.Location = new System.Drawing.Point(393, 1);
            this.label17.Margin = new System.Windows.Forms.Padding(1);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(194, 43);
            this.label17.TabIndex = 10;
            this.label17.Text = "Value";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label17.Visible = false;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label13.Location = new System.Drawing.Point(981, 1);
            this.label13.Margin = new System.Windows.Forms.Padding(1);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(790, 43);
            this.label13.TabIndex = 10;
            this.label13.Text = "Data";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label13.Visible = false;
            // 
            // P1ReadBit_label
            // 
            this.P1ReadBit_label.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1ReadBit_label.Location = new System.Drawing.Point(981, 91);
            this.P1ReadBit_label.Margin = new System.Windows.Forms.Padding(1);
            this.P1ReadBit_label.Name = "P1ReadBit_label";
            this.P1ReadBit_label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.P1ReadBit_label.Size = new System.Drawing.Size(790, 43);
            this.P1ReadBit_label.TabIndex = 26;
            this.P1ReadBit_label.Text = "-";
            this.P1ReadBit_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.P1ReadBit_label.Visible = false;
            // 
            // P1PulseMode_bt
            // 
            this.P1PulseMode_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1PulseMode_bt.Location = new System.Drawing.Point(589, 181);
            this.P1PulseMode_bt.Margin = new System.Windows.Forms.Padding(1);
            this.P1PulseMode_bt.Name = "P1PulseMode_bt";
            this.P1PulseMode_bt.Size = new System.Drawing.Size(194, 44);
            this.P1PulseMode_bt.TabIndex = 15;
            this.P1PulseMode_bt.Text = "Pulse";
            this.P1PulseMode_bt.UseVisualStyleBackColor = true;
            this.P1PulseMode_bt.Visible = false;
            this.P1PulseMode_bt.Click += new System.EventHandler(this.P1PulseMode_bt_Click);
            // 
            // P1Write_label
            // 
            this.P1Write_label.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Write_label.Location = new System.Drawing.Point(981, 136);
            this.P1Write_label.Margin = new System.Windows.Forms.Padding(1);
            this.P1Write_label.Name = "P1Write_label";
            this.P1Write_label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.P1Write_label.Size = new System.Drawing.Size(790, 43);
            this.P1Write_label.TabIndex = 26;
            this.P1Write_label.Text = "-";
            this.P1Write_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.P1Write_label.Visible = false;
            // 
            // P1WriteBit_label
            // 
            this.P1WriteBit_label.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1WriteBit_label.Location = new System.Drawing.Point(981, 181);
            this.P1WriteBit_label.Margin = new System.Windows.Forms.Padding(1);
            this.P1WriteBit_label.Name = "P1WriteBit_label";
            this.P1WriteBit_label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.P1WriteBit_label.Size = new System.Drawing.Size(790, 43);
            this.P1WriteBit_label.TabIndex = 26;
            this.P1WriteBit_label.Text = "-";
            this.P1WriteBit_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.P1WriteBit_label.Visible = false;
            // 
            // Edit_bt
            // 
            this.Edit_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.Edit_bt.Location = new System.Drawing.Point(12, 12);
            this.Edit_bt.Name = "Edit_bt";
            this.Edit_bt.Size = new System.Drawing.Size(132, 50);
            this.Edit_bt.TabIndex = 30;
            this.Edit_bt.Text = "Edit";
            this.Edit_bt.UseVisualStyleBackColor = true;
            this.Edit_bt.Click += new System.EventHandler(this.Edit_bt_Click);
            // 
            // SingleWrite_bt
            // 
            this.SingleWrite_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.SingleWrite_bt.Location = new System.Drawing.Point(906, 113);
            this.SingleWrite_bt.Margin = new System.Windows.Forms.Padding(1);
            this.SingleWrite_bt.Name = "SingleWrite_bt";
            this.tableLayoutPanel1.SetRowSpan(this.SingleWrite_bt, 2);
            this.SingleWrite_bt.Size = new System.Drawing.Size(148, 71);
            this.SingleWrite_bt.TabIndex = 15;
            this.SingleWrite_bt.Text = "Single Write";
            this.SingleWrite_bt.UseVisualStyleBackColor = true;
            this.SingleWrite_bt.Visible = false;
            // 
            // MultiWrite_bt
            // 
            this.MultiWrite_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.MultiWrite_bt.Location = new System.Drawing.Point(906, 187);
            this.MultiWrite_bt.Margin = new System.Windows.Forms.Padding(1);
            this.MultiWrite_bt.Name = "MultiWrite_bt";
            this.MultiWrite_bt.Size = new System.Drawing.Size(148, 34);
            this.MultiWrite_bt.TabIndex = 15;
            this.MultiWrite_bt.Text = "Multi Write";
            this.MultiWrite_bt.UseVisualStyleBackColor = true;
            this.MultiWrite_bt.Visible = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label6.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label6.Location = new System.Drawing.Point(2, 113);
            this.label6.Margin = new System.Windows.Forms.Padding(1);
            this.label6.Name = "label6";
            this.tableLayoutPanel1.SetRowSpan(this.label6, 2);
            this.label6.Size = new System.Drawing.Size(148, 71);
            this.label6.TabIndex = 10;
            this.label6.Text = "Write(S)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label7.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label7.Location = new System.Drawing.Point(2, 187);
            this.label7.Margin = new System.Windows.Forms.Padding(1);
            this.label7.Name = "label7";
            this.tableLayoutPanel1.SetRowSpan(this.label7, 2);
            this.label7.Size = new System.Drawing.Size(148, 75);
            this.label7.TabIndex = 10;
            this.label7.Text = "Write(M)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.Visible = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label5.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label5.Location = new System.Drawing.Point(2, 39);
            this.label5.Margin = new System.Windows.Forms.Padding(1);
            this.label5.Name = "label5";
            this.tableLayoutPanel1.SetRowSpan(this.label5, 2);
            this.label5.Size = new System.Drawing.Size(148, 71);
            this.label5.TabIndex = 10;
            this.label5.Text = "Read";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Visible = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label1.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label1.Location = new System.Drawing.Point(153, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 34);
            this.label1.TabIndex = 10;
            this.label1.Text = "Furnace ID";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // Read_bt
            // 
            this.Read_bt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.Read_bt.Location = new System.Drawing.Point(906, 39);
            this.Read_bt.Margin = new System.Windows.Forms.Padding(1);
            this.Read_bt.Name = "Read_bt";
            this.tableLayoutPanel1.SetRowSpan(this.Read_bt, 2);
            this.Read_bt.Size = new System.Drawing.Size(148, 71);
            this.Read_bt.TabIndex = 15;
            this.Read_bt.Text = "Read";
            this.Read_bt.UseVisualStyleBackColor = true;
            this.Read_bt.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.Read_bt, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ReadID_txt, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ReadRegister_txt, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ReadInput_txt, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.MultiID_txt, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.SingleID_txt, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.MultiRegister_txt, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.SingleRegister_txt, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.MultiInput_txt, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.SingleInput_txt, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.MultiWrite_bt, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.SingleWrite_bt, 4, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(23, 309);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1869, 264);
            this.tableLayoutPanel1.TabIndex = 19;
            this.tableLayoutPanel1.Visible = false;
            // 
            // P1ReadRegister_txt
            // 
            this.P1ReadRegister_txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1ReadRegister_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1ReadRegister_txt.Location = new System.Drawing.Point(197, 46);
            this.P1ReadRegister_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1ReadRegister_txt.Multiline = true;
            this.P1ReadRegister_txt.Name = "P1ReadRegister_txt";
            this.tableLayoutPanel2.SetRowSpan(this.P1ReadRegister_txt, 2);
            this.P1ReadRegister_txt.Size = new System.Drawing.Size(194, 88);
            this.P1ReadRegister_txt.TabIndex = 22;
            this.P1ReadRegister_txt.Text = "1";
            this.P1ReadRegister_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1ReadRegister_txt.Visible = false;
            // 
            // P1Value_txt
            // 
            this.P1Value_txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1Value_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Value_txt.Location = new System.Drawing.Point(393, 136);
            this.P1Value_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Value_txt.Multiline = true;
            this.P1Value_txt.Name = "P1Value_txt";
            this.P1Value_txt.Size = new System.Drawing.Size(194, 43);
            this.P1Value_txt.TabIndex = 22;
            this.P1Value_txt.Text = "1";
            this.P1Value_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1Value_txt.Visible = false;
            // 
            // P1WriteRegister_txt
            // 
            this.P1WriteRegister_txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P1WriteRegister_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1WriteRegister_txt.Location = new System.Drawing.Point(197, 136);
            this.P1WriteRegister_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1WriteRegister_txt.Multiline = true;
            this.P1WriteRegister_txt.Name = "P1WriteRegister_txt";
            this.tableLayoutPanel2.SetRowSpan(this.P1WriteRegister_txt, 2);
            this.P1WriteRegister_txt.Size = new System.Drawing.Size(194, 89);
            this.P1WriteRegister_txt.TabIndex = 22;
            this.P1WriteRegister_txt.Text = "1";
            this.P1WriteRegister_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1WriteRegister_txt.Visible = false;
            // 
            // P1BitTureFalse_txt
            // 
            this.P1BitTureFalse_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1BitTureFalse_txt.Location = new System.Drawing.Point(393, 181);
            this.P1BitTureFalse_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1BitTureFalse_txt.Multiline = true;
            this.P1BitTureFalse_txt.Name = "P1BitTureFalse_txt";
            this.P1BitTureFalse_txt.Size = new System.Drawing.Size(194, 44);
            this.P1BitTureFalse_txt.TabIndex = 22;
            this.P1BitTureFalse_txt.Text = "0";
            this.P1BitTureFalse_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1BitTureFalse_txt.Visible = false;
            // 
            // P1Port_txt
            // 
            this.P1Port_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1Port_txt.Location = new System.Drawing.Point(39, 705);
            this.P1Port_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1Port_txt.Multiline = true;
            this.P1Port_txt.Name = "P1Port_txt";
            this.P1Port_txt.Size = new System.Drawing.Size(148, 71);
            this.P1Port_txt.TabIndex = 21;
            this.P1Port_txt.Text = "502";
            this.P1Port_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1Port_txt.Visible = false;
            // 
            // P1IpAddress_txt
            // 
            this.P1IpAddress_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.P1IpAddress_txt.Location = new System.Drawing.Point(39, 617);
            this.P1IpAddress_txt.Margin = new System.Windows.Forms.Padding(1);
            this.P1IpAddress_txt.Multiline = true;
            this.P1IpAddress_txt.Name = "P1IpAddress_txt";
            this.P1IpAddress_txt.Size = new System.Drawing.Size(148, 71);
            this.P1IpAddress_txt.TabIndex = 20;
            this.P1IpAddress_txt.Text = "192.168.1.32";
            this.P1IpAddress_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.P1IpAddress_txt.Visible = false;
            // 
            // ReadID_txt
            // 
            this.ReadID_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.ReadID_txt.Location = new System.Drawing.Point(153, 39);
            this.ReadID_txt.Margin = new System.Windows.Forms.Padding(1);
            this.ReadID_txt.Multiline = true;
            this.ReadID_txt.Name = "ReadID_txt";
            this.tableLayoutPanel1.SetRowSpan(this.ReadID_txt, 2);
            this.ReadID_txt.Size = new System.Drawing.Size(148, 71);
            this.ReadID_txt.TabIndex = 19;
            this.ReadID_txt.Text = "1";
            this.ReadID_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReadID_txt.Visible = false;
            // 
            // ReadRegister_txt
            // 
            this.ReadRegister_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.ReadRegister_txt.Location = new System.Drawing.Point(304, 39);
            this.ReadRegister_txt.Margin = new System.Windows.Forms.Padding(1);
            this.ReadRegister_txt.Multiline = true;
            this.ReadRegister_txt.Name = "ReadRegister_txt";
            this.tableLayoutPanel1.SetRowSpan(this.ReadRegister_txt, 2);
            this.ReadRegister_txt.Size = new System.Drawing.Size(148, 71);
            this.ReadRegister_txt.TabIndex = 19;
            this.ReadRegister_txt.Text = "0";
            this.ReadRegister_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReadRegister_txt.Visible = false;
            // 
            // ReadInput_txt
            // 
            this.ReadInput_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.ReadInput_txt.Location = new System.Drawing.Point(455, 39);
            this.ReadInput_txt.Margin = new System.Windows.Forms.Padding(1);
            this.ReadInput_txt.Multiline = true;
            this.ReadInput_txt.Name = "ReadInput_txt";
            this.tableLayoutPanel1.SetRowSpan(this.ReadInput_txt, 2);
            this.ReadInput_txt.Size = new System.Drawing.Size(448, 71);
            this.ReadInput_txt.TabIndex = 19;
            this.ReadInput_txt.Text = "0";
            this.ReadInput_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ReadInput_txt.Visible = false;
            // 
            // MultiID_txt
            // 
            this.MultiID_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.MultiID_txt.Location = new System.Drawing.Point(153, 187);
            this.MultiID_txt.Margin = new System.Windows.Forms.Padding(1);
            this.MultiID_txt.Multiline = true;
            this.MultiID_txt.Name = "MultiID_txt";
            this.tableLayoutPanel1.SetRowSpan(this.MultiID_txt, 2);
            this.MultiID_txt.Size = new System.Drawing.Size(148, 75);
            this.MultiID_txt.TabIndex = 23;
            this.MultiID_txt.Text = "1";
            this.MultiID_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MultiID_txt.Visible = false;
            // 
            // SingleID_txt
            // 
            this.SingleID_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.SingleID_txt.Location = new System.Drawing.Point(153, 113);
            this.SingleID_txt.Margin = new System.Windows.Forms.Padding(1);
            this.SingleID_txt.Multiline = true;
            this.SingleID_txt.Name = "SingleID_txt";
            this.tableLayoutPanel1.SetRowSpan(this.SingleID_txt, 2);
            this.SingleID_txt.Size = new System.Drawing.Size(148, 71);
            this.SingleID_txt.TabIndex = 20;
            this.SingleID_txt.Text = "1";
            this.SingleID_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SingleID_txt.Visible = false;
            // 
            // MultiRegister_txt
            // 
            this.MultiRegister_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.MultiRegister_txt.Location = new System.Drawing.Point(304, 187);
            this.MultiRegister_txt.Margin = new System.Windows.Forms.Padding(1);
            this.MultiRegister_txt.Multiline = true;
            this.MultiRegister_txt.Name = "MultiRegister_txt";
            this.tableLayoutPanel1.SetRowSpan(this.MultiRegister_txt, 2);
            this.MultiRegister_txt.Size = new System.Drawing.Size(148, 75);
            this.MultiRegister_txt.TabIndex = 24;
            this.MultiRegister_txt.Text = "0";
            this.MultiRegister_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MultiRegister_txt.Visible = false;
            // 
            // SingleRegister_txt
            // 
            this.SingleRegister_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.SingleRegister_txt.Location = new System.Drawing.Point(304, 113);
            this.SingleRegister_txt.Margin = new System.Windows.Forms.Padding(1);
            this.SingleRegister_txt.Multiline = true;
            this.SingleRegister_txt.Name = "SingleRegister_txt";
            this.tableLayoutPanel1.SetRowSpan(this.SingleRegister_txt, 2);
            this.SingleRegister_txt.Size = new System.Drawing.Size(148, 71);
            this.SingleRegister_txt.TabIndex = 21;
            this.SingleRegister_txt.Text = "0";
            this.SingleRegister_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SingleRegister_txt.Visible = false;
            // 
            // MultiInput_txt
            // 
            this.MultiInput_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.MultiInput_txt.Location = new System.Drawing.Point(455, 187);
            this.MultiInput_txt.Margin = new System.Windows.Forms.Padding(1);
            this.MultiInput_txt.Multiline = true;
            this.MultiInput_txt.Name = "MultiInput_txt";
            this.tableLayoutPanel1.SetRowSpan(this.MultiInput_txt, 2);
            this.MultiInput_txt.Size = new System.Drawing.Size(448, 75);
            this.MultiInput_txt.TabIndex = 25;
            this.MultiInput_txt.Text = "0";
            this.MultiInput_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MultiInput_txt.Visible = false;
            // 
            // SingleInput_txt
            // 
            this.SingleInput_txt.Font = new System.Drawing.Font("Y이드스트릿체 L", 11F);
            this.SingleInput_txt.Location = new System.Drawing.Point(455, 113);
            this.SingleInput_txt.Margin = new System.Windows.Forms.Padding(1);
            this.SingleInput_txt.Multiline = true;
            this.SingleInput_txt.Name = "SingleInput_txt";
            this.tableLayoutPanel1.SetRowSpan(this.SingleInput_txt, 2);
            this.SingleInput_txt.Size = new System.Drawing.Size(448, 71);
            this.SingleInput_txt.TabIndex = 22;
            this.SingleInput_txt.Text = "0";
            this.SingleInput_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SingleInput_txt.Visible = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label2.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label2.Location = new System.Drawing.Point(304, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 34);
            this.label2.TabIndex = 10;
            this.label2.Text = "Register";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label3.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label3.Location = new System.Drawing.Point(455, 2);
            this.label3.Margin = new System.Windows.Forms.Padding(1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(448, 34);
            this.label3.TabIndex = 10;
            this.label3.Text = "Send Data";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label4.Font = new System.Drawing.Font("Y이드스트릿체 L", 12F);
            this.label4.Location = new System.Drawing.Point(906, 2);
            this.label4.Margin = new System.Windows.Forms.Padding(1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 34);
            this.label4.TabIndex = 10;
            this.label4.Text = "Button";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Visible = false;
            // 
            // SubControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.Edit_bt);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.P1Disconnect_bt);
            this.Controls.Add(this.P1Connect_bt);
            this.Controls.Add(this.P1Port_txt);
            this.Controls.Add(this.P1IpAddress_txt);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Status_label);
            this.DoubleBuffered = true;
            this.Name = "SubControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SubControl";
            this.TopMost = true;
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private CenteredTextBox P1IpAddress_txt;
        private CenteredTextBox P1Port_txt;
        private CenteredTextBox P1ReadRegister_txt;
        private System.Windows.Forms.Button P1Read_bt;
        private System.Windows.Forms.Button P1Write_bt;
        private System.Windows.Forms.Button P1Connect_bt;
        private System.Windows.Forms.Button P1Disconnect_bt;
        private System.Windows.Forms.Label Status_label;
        private System.Windows.Forms.Label P1Read_label;
        private System.Windows.Forms.Button P1ReadBit_bt;
        private System.Windows.Forms.Button P1WriteBit_bt;
        private CenteredTextBox P1BitTureFalse_txt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label P1ReadBit_label;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private CenteredTextBox P1WriteRegister_txt;
        private CenteredTextBox P1Value_txt;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label P1Write_label;
        private System.Windows.Forms.Label P1WriteBit_label;
        private System.Windows.Forms.Button P1PulseMode_bt;
        private System.Windows.Forms.Button Edit_bt;
        private System.Windows.Forms.Button SingleWrite_bt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Read_bt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private CenteredTextBox ReadID_txt;
        private CenteredTextBox ReadRegister_txt;
        private CenteredTextBox ReadInput_txt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private CenteredTextBox MultiID_txt;
        private CenteredTextBox SingleID_txt;
        private CenteredTextBox MultiRegister_txt;
        private CenteredTextBox SingleRegister_txt;
        private CenteredTextBox MultiInput_txt;
        private CenteredTextBox SingleInput_txt;
        private System.Windows.Forms.Button MultiWrite_bt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}

