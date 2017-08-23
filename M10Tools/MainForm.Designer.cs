namespace M10Tools
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
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.StatusLabel = new System.Windows.Forms.Label();
      this.btnBuildFolder = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.statusStrip1.SuspendLayout();
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
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
      this.statusStrip1.Location = new System.Drawing.Point(0, 240);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(440, 22);
      this.statusStrip1.TabIndex = 7;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(129, 17);
      this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
      // 
      // StatusLabel
      // 
      this.StatusLabel.AutoSize = true;
      this.StatusLabel.Location = new System.Drawing.Point(12, 219);
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.Size = new System.Drawing.Size(58, 12);
      this.StatusLabel.TabIndex = 8;
      this.StatusLabel.Text = "StatusLabel";
      // 
      // btnBuildFolder
      // 
      this.btnBuildFolder.Location = new System.Drawing.Point(12, 99);
      this.btnBuildFolder.Name = "btnBuildFolder";
      this.btnBuildFolder.Size = new System.Drawing.Size(126, 23);
      this.btnBuildFolder.TabIndex = 9;
      this.btnBuildFolder.Text = "建立救援資料夾";
      this.btnBuildFolder.UseVisualStyleBackColor = true;
      this.btnBuildFolder.Click += new System.EventHandler(this.btnBuildFolder_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 128);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(126, 23);
      this.button1.TabIndex = 10;
      this.button1.Text = "test";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(440, 262);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnBuildFolder);
      this.Controls.Add(this.StatusLabel);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.btnStockTrans);
      this.Controls.Add(this.btnKML);
      this.Controls.Add(this.btnImpErrLRTI);
      this.Name = "MainForm";
      this.Text = "MainForm";
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnImpErrLRTI;
    private System.Windows.Forms.Button btnKML;
    private System.Windows.Forms.Button btnStockTrans;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.Label StatusLabel;
    private System.Windows.Forms.Button btnBuildFolder;
    private System.Windows.Forms.Button button1;
  }
}

