﻿namespace M10_XmlArrange
{
  partial class MXAP010Form
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
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.lblProgress = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblStatus = new System.Windows.Forms.Label();
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
      this.label1.Location = new System.Drawing.Point(12, 42);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(105, 24);
      this.label1.TabIndex = 1;
      this.label1.Text = "執行進度：";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.label2.Location = new System.Drawing.Point(12, 80);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(105, 24);
      this.label2.TabIndex = 2;
      this.label2.Text = "執行程序：";
      // 
      // lblProcCount
      // 
      this.lblProcCount.AutoSize = true;
      this.lblProcCount.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblProcCount.Location = new System.Drawing.Point(123, 42);
      this.lblProcCount.Name = "lblProcCount";
      this.lblProcCount.Size = new System.Drawing.Size(48, 24);
      this.lblProcCount.TabIndex = 3;
      this.lblProcCount.Text = "進度";
      // 
      // lblProc
      // 
      this.lblProc.AutoSize = true;
      this.lblProc.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblProc.Location = new System.Drawing.Point(123, 80);
      this.lblProc.Name = "lblProc";
      this.lblProc.Size = new System.Drawing.Size(48, 24);
      this.lblProc.TabIndex = 4;
      this.lblProc.Text = "程序";
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
      this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
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
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.label3.Location = new System.Drawing.Point(12, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(105, 24);
      this.label3.TabIndex = 9;
      this.label3.Text = "執行狀態：";
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
      this.lblStatus.Location = new System.Drawing.Point(123, 9);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(48, 24);
      this.lblStatus.TabIndex = 10;
      this.lblStatus.Text = "狀態";
      // 
      // MXAP010Form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(818, 319);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.lblProgress);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.lblProc);
      this.Controls.Add(this.lblProcCount);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnStart);
      this.Name = "MXAP010Form";
      this.Text = "M10彙整XML功能";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MXAP010Form_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MXAP010Form_FormClosed);
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
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.Label lblProgress;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lblStatus;
  }
}

