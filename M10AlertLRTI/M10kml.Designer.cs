namespace M10AlertLRTI
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
      this.btnDoc = new System.Windows.Forms.Button();
      this.btnVillage = new System.Windows.Forms.Button();
      this.btnCountry = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnDoc
      // 
      this.btnDoc.Location = new System.Drawing.Point(12, 12);
      this.btnDoc.Name = "btnDoc";
      this.btnDoc.Size = new System.Drawing.Size(75, 23);
      this.btnDoc.TabIndex = 0;
      this.btnDoc.Text = "kml_doc";
      this.btnDoc.UseVisualStyleBackColor = true;
      this.btnDoc.Click += new System.EventHandler(this.button1_Click);
      // 
      // btnVillage
      // 
      this.btnVillage.Location = new System.Drawing.Point(12, 41);
      this.btnVillage.Name = "btnVillage";
      this.btnVillage.Size = new System.Drawing.Size(75, 23);
      this.btnVillage.TabIndex = 1;
      this.btnVillage.Text = "kml_village";
      this.btnVillage.UseVisualStyleBackColor = true;
      this.btnVillage.Click += new System.EventHandler(this.button2_Click);
      // 
      // btnCountry
      // 
      this.btnCountry.Location = new System.Drawing.Point(12, 70);
      this.btnCountry.Name = "btnCountry";
      this.btnCountry.Size = new System.Drawing.Size(75, 23);
      this.btnCountry.TabIndex = 2;
      this.btnCountry.Text = "kml_country";
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
      // M10kml
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnCountry);
      this.Controls.Add(this.btnVillage);
      this.Controls.Add(this.btnDoc);
      this.Name = "M10kml";
      this.Text = "M10kml";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnDoc;
    private System.Windows.Forms.Button btnVillage;
    private System.Windows.Forms.Button btnCountry;
    private System.Windows.Forms.Button button1;
  }
}