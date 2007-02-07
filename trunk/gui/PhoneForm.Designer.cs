namespace Gui
{
  partial class PhoneForm
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
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.digitKeyH = new System.Windows.Forms.Button();
      this.digitKey0 = new System.Windows.Forms.Button();
      this.digitKeyS = new System.Windows.Forms.Button();
      this.digitKey7 = new System.Windows.Forms.Button();
      this.digitKey8 = new System.Windows.Forms.Button();
      this.digitKey9 = new System.Windows.Forms.Button();
      this.digitKey4 = new System.Windows.Forms.Button();
      this.digitKey5 = new System.Windows.Forms.Button();
      this.digitKey6 = new System.Windows.Forms.Button();
      this.digitKey3 = new System.Windows.Forms.Button();
      this.digitKey2 = new System.Windows.Forms.Button();
      this.digitKey1 = new System.Windows.Forms.Button();
      this.onhookButton = new System.Windows.Forms.Button();
      this.offhookButton = new System.Windows.Forms.Button();
      this.OKbutton = new System.Windows.Forms.Button();
      this.Cancel = new System.Windows.Forms.Button();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // richTextBox1
      // 
      this.richTextBox1.BackColor = System.Drawing.Color.Navy;
      this.richTextBox1.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.richTextBox1.ForeColor = System.Drawing.Color.White;
      this.richTextBox1.HideSelection = false;
      this.richTextBox1.Location = new System.Drawing.Point(42, 12);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.ReadOnly = true;
      this.richTextBox1.Size = new System.Drawing.Size(284, 235);
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = "";
      this.richTextBox1.WordWrap = false;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.digitKeyH);
      this.panel2.Controls.Add(this.digitKey0);
      this.panel2.Controls.Add(this.digitKeyS);
      this.panel2.Controls.Add(this.digitKey7);
      this.panel2.Controls.Add(this.digitKey8);
      this.panel2.Controls.Add(this.digitKey9);
      this.panel2.Controls.Add(this.digitKey4);
      this.panel2.Controls.Add(this.digitKey5);
      this.panel2.Controls.Add(this.digitKey6);
      this.panel2.Controls.Add(this.digitKey3);
      this.panel2.Controls.Add(this.digitKey2);
      this.panel2.Controls.Add(this.digitKey1);
      this.panel2.Location = new System.Drawing.Point(97, 363);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(178, 167);
      this.panel2.TabIndex = 18;
      // 
      // digitKeyH
      // 
      this.digitKeyH.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.digitKeyH.Location = new System.Drawing.Point(120, 126);
      this.digitKeyH.Name = "digitKeyH";
      this.digitKeyH.Size = new System.Drawing.Size(40, 38);
      this.digitKeyH.TabIndex = 11;
      this.digitKeyH.Text = "#";
      this.digitKeyH.UseVisualStyleBackColor = true;
      this.digitKeyH.Click += new System.EventHandler(this.digitKeyH_Click);
      // 
      // digitKey0
      // 
      this.digitKey0.Location = new System.Drawing.Point(67, 126);
      this.digitKey0.Name = "digitKey0";
      this.digitKey0.Size = new System.Drawing.Size(38, 38);
      this.digitKey0.TabIndex = 10;
      this.digitKey0.Text = "0";
      this.digitKey0.UseVisualStyleBackColor = true;
      this.digitKey0.Click += new System.EventHandler(this.digitKey0_Click);
      // 
      // digitKeyS
      // 
      this.digitKeyS.Location = new System.Drawing.Point(16, 128);
      this.digitKeyS.Name = "digitKeyS";
      this.digitKeyS.Size = new System.Drawing.Size(39, 36);
      this.digitKeyS.TabIndex = 9;
      this.digitKeyS.Text = "*";
      this.digitKeyS.UseVisualStyleBackColor = true;
      this.digitKeyS.Click += new System.EventHandler(this.digitKeyS_Click);
      // 
      // digitKey7
      // 
      this.digitKey7.Location = new System.Drawing.Point(16, 86);
      this.digitKey7.Name = "digitKey7";
      this.digitKey7.Size = new System.Drawing.Size(39, 36);
      this.digitKey7.TabIndex = 8;
      this.digitKey7.Text = "7 pqrs";
      this.digitKey7.UseVisualStyleBackColor = true;
      this.digitKey7.Click += new System.EventHandler(this.digitKey7_Click);
      // 
      // digitKey8
      // 
      this.digitKey8.Location = new System.Drawing.Point(67, 86);
      this.digitKey8.Name = "digitKey8";
      this.digitKey8.Size = new System.Drawing.Size(38, 36);
      this.digitKey8.TabIndex = 7;
      this.digitKey8.Text = "8 tuv";
      this.digitKey8.UseVisualStyleBackColor = true;
      this.digitKey8.Click += new System.EventHandler(this.digitKey8_Click);
      // 
      // digitKey9
      // 
      this.digitKey9.Location = new System.Drawing.Point(120, 86);
      this.digitKey9.Name = "digitKey9";
      this.digitKey9.Size = new System.Drawing.Size(40, 36);
      this.digitKey9.TabIndex = 6;
      this.digitKey9.Text = "9 wxyz";
      this.digitKey9.UseVisualStyleBackColor = true;
      this.digitKey9.Click += new System.EventHandler(this.digitKey9_Click);
      // 
      // digitKey4
      // 
      this.digitKey4.Location = new System.Drawing.Point(16, 45);
      this.digitKey4.Name = "digitKey4";
      this.digitKey4.Size = new System.Drawing.Size(39, 34);
      this.digitKey4.TabIndex = 5;
      this.digitKey4.Text = " 4 ghi";
      this.digitKey4.UseVisualStyleBackColor = true;
      this.digitKey4.Click += new System.EventHandler(this.digitKey4_Click);
      // 
      // digitKey5
      // 
      this.digitKey5.Location = new System.Drawing.Point(67, 45);
      this.digitKey5.Name = "digitKey5";
      this.digitKey5.Size = new System.Drawing.Size(38, 34);
      this.digitKey5.TabIndex = 4;
      this.digitKey5.Text = "  5 jkl";
      this.digitKey5.UseVisualStyleBackColor = true;
      this.digitKey5.Click += new System.EventHandler(this.digitKey5_Click);
      // 
      // digitKey6
      // 
      this.digitKey6.Location = new System.Drawing.Point(120, 45);
      this.digitKey6.Name = "digitKey6";
      this.digitKey6.Size = new System.Drawing.Size(40, 34);
      this.digitKey6.TabIndex = 3;
      this.digitKey6.Text = "6 mno";
      this.digitKey6.UseVisualStyleBackColor = true;
      this.digitKey6.Click += new System.EventHandler(this.digitKey6_Click);
      // 
      // digitKey3
      // 
      this.digitKey3.Location = new System.Drawing.Point(120, 3);
      this.digitKey3.Name = "digitKey3";
      this.digitKey3.Size = new System.Drawing.Size(40, 36);
      this.digitKey3.TabIndex = 2;
      this.digitKey3.Text = " 3 def";
      this.digitKey3.UseVisualStyleBackColor = true;
      this.digitKey3.Click += new System.EventHandler(this.digitKey3_Click);
      // 
      // digitKey2
      // 
      this.digitKey2.Location = new System.Drawing.Point(67, 3);
      this.digitKey2.Name = "digitKey2";
      this.digitKey2.Size = new System.Drawing.Size(38, 36);
      this.digitKey2.TabIndex = 1;
      this.digitKey2.Text = "2 abc";
      this.digitKey2.UseVisualStyleBackColor = true;
      this.digitKey2.Click += new System.EventHandler(this.digitKey2_Click);
      // 
      // digitKey1
      // 
      this.digitKey1.Location = new System.Drawing.Point(16, 3);
      this.digitKey1.Name = "digitKey1";
      this.digitKey1.Size = new System.Drawing.Size(39, 36);
      this.digitKey1.TabIndex = 0;
      this.digitKey1.Text = "1";
      this.digitKey1.UseVisualStyleBackColor = true;
      this.digitKey1.Click += new System.EventHandler(this.digitKey1_Click);
      // 
      // onhookButton
      // 
      this.onhookButton.BackColor = System.Drawing.Color.OrangeRed;
      this.onhookButton.Location = new System.Drawing.Point(263, 288);
      this.onhookButton.Name = "onhookButton";
      this.onhookButton.Size = new System.Drawing.Size(48, 50);
      this.onhookButton.TabIndex = 19;
      this.onhookButton.Text = "DOWN";
      this.onhookButton.UseVisualStyleBackColor = false;
      this.onhookButton.Click += new System.EventHandler(this.onhookButton_Click);
      // 
      // offhookButton
      // 
      this.offhookButton.BackColor = System.Drawing.Color.DarkSeaGreen;
      this.offhookButton.Location = new System.Drawing.Point(52, 288);
      this.offhookButton.Name = "offhookButton";
      this.offhookButton.Size = new System.Drawing.Size(46, 50);
      this.offhookButton.TabIndex = 20;
      this.offhookButton.Text = "UP";
      this.offhookButton.UseVisualStyleBackColor = false;
      this.offhookButton.Click += new System.EventHandler(this.offhookButton_Click);
      // 
      // OKbutton
      // 
      this.OKbutton.Location = new System.Drawing.Point(164, 277);
      this.OKbutton.Name = "OKbutton";
      this.OKbutton.Size = new System.Drawing.Size(41, 32);
      this.OKbutton.TabIndex = 21;
      this.OKbutton.Text = "OK";
      this.OKbutton.UseVisualStyleBackColor = true;
      this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
      // 
      // Cancel
      // 
      this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel.Location = new System.Drawing.Point(153, 324);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new System.Drawing.Size(64, 23);
      this.Cancel.TabIndex = 22;
      this.Cancel.Text = "Esc";
      this.Cancel.UseVisualStyleBackColor = true;
      this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // PhoneForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.WhiteSmoke;
      this.CancelButton = this.Cancel;
      this.ClientSize = new System.Drawing.Size(367, 542);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.OKbutton);
      this.Controls.Add(this.offhookButton);
      this.Controls.Add(this.onhookButton);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.richTextBox1);
      this.Name = "PhoneForm";
      this.Text = "SIPek Phone";
      this.Shown += new System.EventHandler(this.PhoneForm_Shown);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PhoneForm_FormClosing);
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button digitKeyH;
    private System.Windows.Forms.Button digitKey0;
    private System.Windows.Forms.Button digitKeyS;
    private System.Windows.Forms.Button digitKey7;
    private System.Windows.Forms.Button digitKey8;
    private System.Windows.Forms.Button digitKey9;
    private System.Windows.Forms.Button digitKey4;
    private System.Windows.Forms.Button digitKey5;
    private System.Windows.Forms.Button digitKey6;
    private System.Windows.Forms.Button digitKey3;
    private System.Windows.Forms.Button digitKey2;
    private System.Windows.Forms.Button digitKey1;
    private System.Windows.Forms.Button onhookButton;
    private System.Windows.Forms.Button offhookButton;
    private System.Windows.Forms.Button OKbutton;
    private System.Windows.Forms.Button Cancel;
  }
}

