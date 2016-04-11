namespace M10CalRtiDetail
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ddlver = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnstart = new System.Windows.Forms.Button();
            this.ddlTimeDelay = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRTI10 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.lblrti10 = new System.Windows.Forms.Label();
            this.lblrti30 = new System.Windows.Forms.Label();
            this.lblrti50 = new System.Windows.Forms.Label();
            this.lblrti70 = new System.Windows.Forms.Label();
            this.lblrti90 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // ddlver
            // 
            this.ddlver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlver.FormattingEnabled = true;
            this.ddlver.Location = new System.Drawing.Point(216, 20);
            this.ddlver.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddlver.Name = "ddlver";
            this.ddlver.Size = new System.Drawing.Size(199, 28);
            this.ddlver.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(14, 101);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1417, 666);
            this.dataGridView1.TabIndex = 1;
            // 
            // btnstart
            // 
            this.btnstart.Location = new System.Drawing.Point(489, 53);
            this.btnstart.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(125, 38);
            this.btnstart.TabIndex = 2;
            this.btnstart.Text = "開始轉檔";
            this.btnstart.UseVisualStyleBackColor = true;
            this.btnstart.Click += new System.EventHandler(this.btnstart_Click);
            // 
            // ddlTimeDelay
            // 
            this.ddlTimeDelay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlTimeDelay.FormattingEnabled = true;
            this.ddlTimeDelay.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.ddlTimeDelay.Location = new System.Drawing.Point(216, 63);
            this.ddlTimeDelay.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddlTimeDelay.Name = "ddlTimeDelay";
            this.ddlTimeDelay.Size = new System.Drawing.Size(199, 28);
            this.ddlTimeDelay.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "工作延時：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // btnRTI10
            // 
            this.btnRTI10.Location = new System.Drawing.Point(642, 53);
            this.btnRTI10.Margin = new System.Windows.Forms.Padding(5);
            this.btnRTI10.Name = "btnRTI10";
            this.btnRTI10.Size = new System.Drawing.Size(125, 38);
            this.btnRTI10.TabIndex = 6;
            this.btnRTI10.Text = "轉檔RTI10";
            this.btnRTI10.UseVisualStyleBackColor = true;
            this.btnRTI10.Click += new System.EventHandler(this.btnRTI10_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(42, 123);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(5);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 24;
            this.dataGridView2.Size = new System.Drawing.Size(572, 534);
            this.dataGridView2.TabIndex = 7;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(701, 123);
            this.dataGridView3.Margin = new System.Windows.Forms.Padding(5);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 24;
            this.dataGridView3.Size = new System.Drawing.Size(572, 534);
            this.dataGridView3.TabIndex = 8;
            // 
            // lblrti10
            // 
            this.lblrti10.AutoSize = true;
            this.lblrti10.Location = new System.Drawing.Point(786, 66);
            this.lblrti10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblrti10.Name = "lblrti10";
            this.lblrti10.Size = new System.Drawing.Size(89, 20);
            this.lblrti10.TabIndex = 9;
            this.lblrti10.Text = "工作延時：";
            // 
            // lblrti30
            // 
            this.lblrti30.AutoSize = true;
            this.lblrti30.Location = new System.Drawing.Point(904, 66);
            this.lblrti30.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblrti30.Name = "lblrti30";
            this.lblrti30.Size = new System.Drawing.Size(89, 20);
            this.lblrti30.TabIndex = 10;
            this.lblrti30.Text = "工作延時：";
            // 
            // lblrti50
            // 
            this.lblrti50.AutoSize = true;
            this.lblrti50.Location = new System.Drawing.Point(1022, 66);
            this.lblrti50.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblrti50.Name = "lblrti50";
            this.lblrti50.Size = new System.Drawing.Size(89, 20);
            this.lblrti50.TabIndex = 11;
            this.lblrti50.Text = "工作延時：";
            // 
            // lblrti70
            // 
            this.lblrti70.AutoSize = true;
            this.lblrti70.Location = new System.Drawing.Point(1140, 66);
            this.lblrti70.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblrti70.Name = "lblrti70";
            this.lblrti70.Size = new System.Drawing.Size(89, 20);
            this.lblrti70.TabIndex = 12;
            this.lblrti70.Text = "工作延時：";
            // 
            // lblrti90
            // 
            this.lblrti90.AutoSize = true;
            this.lblrti90.Location = new System.Drawing.Point(1258, 66);
            this.lblrti90.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblrti90.Name = "lblrti90";
            this.lblrti90.Size = new System.Drawing.Size(89, 20);
            this.lblrti90.TabIndex = 13;
            this.lblrti90.Text = "工作延時：";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1445, 781);
            this.Controls.Add(this.lblrti90);
            this.Controls.Add(this.lblrti70);
            this.Controls.Add(this.lblrti50);
            this.Controls.Add(this.lblrti30);
            this.Controls.Add(this.lblrti10);
            this.Controls.Add(this.dataGridView3);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.btnRTI10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlTimeDelay);
            this.Controls.Add(this.btnstart);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.ddlver);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "MainForm";
            this.Text = "M10_Cal_RtiDetail";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlver;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnstart;
        private System.Windows.Forms.ComboBox ddlTimeDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRTI10;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Label lblrti10;
        private System.Windows.Forms.Label lblrti30;
        private System.Windows.Forms.Label lblrti50;
        private System.Windows.Forms.Label lblrti70;
        private System.Windows.Forms.Label lblrti90;
    }
}

