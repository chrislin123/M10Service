namespace M10Winform
{
  partial class MainFormNew
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
      this.components = new System.ComponentModel.Container();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.chktimer = new System.Windows.Forms.CheckBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnStart = new System.Windows.Forms.Button();
      this.chkdownload = new System.Windows.Forms.CheckBox();
      this.button1 = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // timer1
      // 
      this.timer1.Interval = 5000;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // richTextBox1
      // 
      this.richTextBox1.Location = new System.Drawing.Point(12, 12);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.Size = new System.Drawing.Size(490, 386);
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = "";
      // 
      // chktimer
      // 
      this.chktimer.AutoSize = true;
      this.chktimer.Checked = true;
      this.chktimer.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chktimer.Location = new System.Drawing.Point(526, 45);
      this.chktimer.Name = "chktimer";
      this.chktimer.Size = new System.Drawing.Size(72, 16);
      this.chktimer.TabIndex = 1;
      this.chktimer.Text = "是否啟動";
      this.chktimer.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.Cornsilk;
      this.panel1.Controls.Add(this.btnStart);
      this.panel1.Controls.Add(this.chkdownload);
      this.panel1.Location = new System.Drawing.Point(508, 143);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(132, 172);
      this.panel1.TabIndex = 3;
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(30, 79);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(75, 23);
      this.btnStart.TabIndex = 4;
      this.btnStart.Text = "開始";
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // chkdownload
      // 
      this.chkdownload.AutoSize = true;
      this.chkdownload.Checked = true;
      this.chkdownload.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkdownload.Location = new System.Drawing.Point(18, 23);
      this.chkdownload.Name = "chkdownload";
      this.chkdownload.Size = new System.Drawing.Size(72, 16);
      this.chkdownload.TabIndex = 3;
      this.chkdownload.Text = "是否下載";
      this.chkdownload.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(538, 353);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 5;
      this.button1.Text = "測試";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // MainFormNew
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(644, 409);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.chktimer);
      this.Controls.Add(this.richTextBox1);
      this.Name = "MainFormNew";
      this.Text = "MainFormNew";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.RichTextBox richTextBox1;
    private System.Windows.Forms.CheckBox chktimer;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.CheckBox chkdownload;
    private System.Windows.Forms.Button button1;
  }
}

