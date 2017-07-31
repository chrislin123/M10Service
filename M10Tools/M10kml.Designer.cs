namespace M10Tools
{
  partial class M10kml
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
      if (disposing && (components != null))
      {
        components.Dispose();
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
      this.btnVillage = new System.Windows.Forms.Button();
      this.btnTownShip = new System.Windows.Forms.Button();
      this.btnCountry = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.RarButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnVillage
      // 
      this.btnVillage.Location = new System.Drawing.Point(12, 12);
      this.btnVillage.Name = "btnVillage";
      this.btnVillage.Size = new System.Drawing.Size(75, 23);
      this.btnVillage.TabIndex = 0;
      this.btnVillage.Text = "kml_村里";
      this.btnVillage.UseVisualStyleBackColor = true;
      this.btnVillage.Click += new System.EventHandler(this.btnVillage_Click);
      // 
      // btnTownShip
      // 
      this.btnTownShip.Location = new System.Drawing.Point(12, 41);
      this.btnTownShip.Name = "btnTownShip";
      this.btnTownShip.Size = new System.Drawing.Size(75, 23);
      this.btnTownShip.TabIndex = 1;
      this.btnTownShip.Text = "kml_鄉鎮";
      this.btnTownShip.UseVisualStyleBackColor = true;
      this.btnTownShip.Click += new System.EventHandler(this.button2_Click);
      // 
      // btnCountry
      // 
      this.btnCountry.Location = new System.Drawing.Point(12, 70);
      this.btnCountry.Name = "btnCountry";
      this.btnCountry.Size = new System.Drawing.Size(75, 23);
      this.btnCountry.TabIndex = 2;
      this.btnCountry.Text = "kml_縣市";
      this.btnCountry.UseVisualStyleBackColor = true;
      this.btnCountry.Click += new System.EventHandler(this.btnCountry_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(12, 99);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "TEST";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click_1);
      // 
      // RarButton
      // 
      this.RarButton.Location = new System.Drawing.Point(12, 128);
      this.RarButton.Name = "RarButton";
      this.RarButton.Size = new System.Drawing.Size(75, 23);
      this.RarButton.TabIndex = 4;
      this.RarButton.Text = "RAR";
      this.RarButton.UseVisualStyleBackColor = true;
      this.RarButton.Click += new System.EventHandler(this.RarButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(168, 23);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(33, 12);
      this.label1.TabIndex = 5;
      this.label1.Text = "label1";
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
      this.statusStrip1.Location = new System.Drawing.Point(0, 240);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(284, 22);
      this.statusStrip1.TabIndex = 6;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(129, 17);
      this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
      // 
      // M10kml
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.RarButton);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnCountry);
      this.Controls.Add(this.btnTownShip);
      this.Controls.Add(this.btnVillage);
      this.Name = "M10kml";
      this.Text = "M10kml";
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnVillage;
    private System.Windows.Forms.Button btnTownShip;
    private System.Windows.Forms.Button btnCountry;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button RarButton;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
  }
}