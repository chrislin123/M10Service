﻿namespace M10Tools
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
    /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
    /// 這個方法的內容。
    /// </summary>
    private void InitializeComponent()
    {
      this.btnImpErrLRTI = new System.Windows.Forms.Button();
      this.btnKML = new System.Windows.Forms.Button();
      this.btnStockTrans = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnImpErrLRTI
      // 
      this.btnImpErrLRTI.Location = new System.Drawing.Point(12, 12);
      this.btnImpErrLRTI.Name = "btnImpErrLRTI";
      this.btnImpErrLRTI.Size = new System.Drawing.Size(126, 23);
      this.btnImpErrLRTI.TabIndex = 0;
      this.btnImpErrLRTI.Text = "匯入警戒值(ErrLRTI)";
      this.btnImpErrLRTI.UseVisualStyleBackColor = true;
      this.btnImpErrLRTI.Click += new System.EventHandler(this.btnImpErrLRTI_Click);
      // 
      // btnKML
      // 
      this.btnKML.Location = new System.Drawing.Point(12, 41);
      this.btnKML.Name = "btnKML";
      this.btnKML.Size = new System.Drawing.Size(126, 23);
      this.btnKML.TabIndex = 1;
      this.btnKML.Text = "KML轉檔";
      this.btnKML.UseVisualStyleBackColor = true;
      this.btnKML.Click += new System.EventHandler(this.btnKML_Click);
      // 
      // btnStockTrans
      // 
      this.btnStockTrans.Location = new System.Drawing.Point(12, 70);
      this.btnStockTrans.Name = "btnStockTrans";
      this.btnStockTrans.Size = new System.Drawing.Size(126, 23);
      this.btnStockTrans.TabIndex = 2;
      this.btnStockTrans.Text = "Stock轉檔";
      this.btnStockTrans.UseVisualStyleBackColor = true;
      this.btnStockTrans.Click += new System.EventHandler(this.btnStockTrans_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.btnStockTrans);
      this.Controls.Add(this.btnKML);
      this.Controls.Add(this.btnImpErrLRTI);
      this.Name = "MainForm";
      this.Text = "MainForm";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnImpErrLRTI;
    private System.Windows.Forms.Button btnKML;
    private System.Windows.Forms.Button btnStockTrans;
  }
}

