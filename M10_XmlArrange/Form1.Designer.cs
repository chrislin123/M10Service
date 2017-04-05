namespace M10_XmlArrange
{
  partial class Form1
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
    /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
    /// 這個方法的內容。
    /// </summary>
    private void InitializeComponent()
    {
      this.btnStart = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lblProcCount = new System.Windows.Forms.Label();
      this.lblProc = new System.Windows.Forms.Label();
      this.btnTest = new System.Windows.Forms.Button();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.btnBack = new System.Windows.Forms.Button();
      this.lblProgress = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(361, 284);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(75, 23);
      this.btnStart.TabIndex = 0;
      this.btnStart.Text = "啟動";
      this.btnStart.UseVisualStyleBackColor = true;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(105, 24);
      this.label1.TabIndex = 1;
      this.label1.Text = "執行進度：";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.label2.Location = new System.Drawing.Point(12, 47);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(105, 24);
      this.label2.TabIndex = 2;
      this.label2.Text = "執行程序：";
      // 
      // lblProcCount
      // 
      this.lblProcCount.AutoSize = true;
      this.lblProcCount.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblProcCount.Location = new System.Drawing.Point(123, 9);
      this.lblProcCount.Name = "lblProcCount";
      this.lblProcCount.Size = new System.Drawing.Size(48, 24);
      this.lblProcCount.TabIndex = 3;
      this.lblProcCount.Text = "進度";
      // 
      // lblProc
      // 
      this.lblProc.AutoSize = true;
      this.lblProc.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblProc.Location = new System.Drawing.Point(123, 47);
      this.lblProc.Name = "lblProc";
      this.lblProc.Size = new System.Drawing.Size(48, 24);
      this.lblProc.TabIndex = 4;
      this.lblProc.Text = "程序";
      // 
      // btnTest
      // 
      this.btnTest.Location = new System.Drawing.Point(442, 284);
      this.btnTest.Name = "btnTest";
      this.btnTest.Size = new System.Drawing.Size(75, 23);
      this.btnTest.TabIndex = 5;
      this.btnTest.Text = "測試";
      this.btnTest.UseVisualStyleBackColor = true;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(12, 205);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(794, 23);
      this.progressBar1.TabIndex = 6;
      // 
      // backgroundWorker1
      // 
      this.backgroundWorker1.WorkerReportsProgress = true;
      this.backgroundWorker1.WorkerSupportsCancellation = true;
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(523, 284);
      this.btnBack.Name = "btnBack";
      this.btnBack.Size = new System.Drawing.Size(75, 23);
      this.btnBack.TabIndex = 7;
      this.btnBack.Text = "背景";
      this.btnBack.UseVisualStyleBackColor = true;
      this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
      // 
      // lblProgress
      // 
      this.lblProgress.AutoSize = true;
      this.lblProgress.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblProgress.Location = new System.Drawing.Point(12, 178);
      this.lblProgress.Name = "lblProgress";
      this.lblProgress.Size = new System.Drawing.Size(48, 24);
      this.lblProgress.TabIndex = 8;
      this.lblProgress.Text = "程序";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(818, 319);
      this.Controls.Add(this.lblProgress);
      this.Controls.Add(this.btnBack);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.btnTest);
      this.Controls.Add(this.lblProc);
      this.Controls.Add(this.lblProcCount);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnStart);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lblProcCount;
    private System.Windows.Forms.Label lblProc;
    private System.Windows.Forms.Button btnTest;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Label lblProgress;
  }
}

