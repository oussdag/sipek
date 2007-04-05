namespace Sipek
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoneForm));
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
      this.clearButton = new System.Windows.Forms.Button();
      this.offhookButton = new System.Windows.Forms.Button();
      this.OKbutton = new System.Windows.Forms.Button();
      this.Cancel = new System.Windows.Forms.Button();
      this.buttonSettings = new System.Windows.Forms.Button();
      this.leftNav = new System.Windows.Forms.Button();
      this.rightNav = new System.Windows.Forms.Button();
      this.upNav = new System.Windows.Forms.Button();
      this.downNav = new System.Windows.Forms.Button();
      this.button0 = new System.Windows.Forms.Button();
      this.button5 = new System.Windows.Forms.Button();
      this.button6 = new System.Windows.Forms.Button();
      this.button7 = new System.Windows.Forms.Button();
      this.button8 = new System.Windows.Forms.Button();
      this.button10 = new System.Windows.Forms.Button();
      this.button9 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.button4 = new System.Windows.Forms.Button();
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
      this.richTextBox1.Size = new System.Drawing.Size(284, 248);
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
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
      this.panel2.Location = new System.Drawing.Point(86, 377);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(201, 167);
      this.panel2.TabIndex = 18;
      // 
      // digitKeyH
      // 
      this.digitKeyH.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.digitKeyH.Location = new System.Drawing.Point(135, 126);
      this.digitKeyH.Name = "digitKeyH";
      this.digitKeyH.Size = new System.Drawing.Size(40, 38);
      this.digitKeyH.TabIndex = 11;
      this.digitKeyH.Text = "#";
      this.digitKeyH.UseVisualStyleBackColor = true;
      this.digitKeyH.Click += new System.EventHandler(this.digitKeyH_Click);
      // 
      // digitKey0
      // 
      this.digitKey0.Location = new System.Drawing.Point(80, 126);
      this.digitKey0.Name = "digitKey0";
      this.digitKey0.Size = new System.Drawing.Size(38, 38);
      this.digitKey0.TabIndex = 10;
      this.digitKey0.Text = "0";
      this.digitKey0.UseVisualStyleBackColor = true;
      this.digitKey0.Click += new System.EventHandler(this.digitKey0_Click);
      // 
      // digitKeyS
      // 
      this.digitKeyS.Location = new System.Drawing.Point(26, 128);
      this.digitKeyS.Name = "digitKeyS";
      this.digitKeyS.Size = new System.Drawing.Size(39, 36);
      this.digitKeyS.TabIndex = 9;
      this.digitKeyS.Text = "*";
      this.digitKeyS.UseVisualStyleBackColor = true;
      this.digitKeyS.Click += new System.EventHandler(this.digitKeyS_Click);
      // 
      // digitKey7
      // 
      this.digitKey7.Location = new System.Drawing.Point(26, 86);
      this.digitKey7.Name = "digitKey7";
      this.digitKey7.Size = new System.Drawing.Size(39, 36);
      this.digitKey7.TabIndex = 8;
      this.digitKey7.Text = "7 pqrs";
      this.digitKey7.UseVisualStyleBackColor = true;
      this.digitKey7.Click += new System.EventHandler(this.digitKey7_Click);
      // 
      // digitKey8
      // 
      this.digitKey8.Location = new System.Drawing.Point(80, 86);
      this.digitKey8.Name = "digitKey8";
      this.digitKey8.Size = new System.Drawing.Size(38, 36);
      this.digitKey8.TabIndex = 7;
      this.digitKey8.Text = "8 tuv";
      this.digitKey8.UseVisualStyleBackColor = true;
      this.digitKey8.Click += new System.EventHandler(this.digitKey8_Click);
      // 
      // digitKey9
      // 
      this.digitKey9.Location = new System.Drawing.Point(135, 86);
      this.digitKey9.Name = "digitKey9";
      this.digitKey9.Size = new System.Drawing.Size(40, 36);
      this.digitKey9.TabIndex = 6;
      this.digitKey9.Text = "9 wxyz";
      this.digitKey9.UseVisualStyleBackColor = true;
      this.digitKey9.Click += new System.EventHandler(this.digitKey9_Click);
      // 
      // digitKey4
      // 
      this.digitKey4.Location = new System.Drawing.Point(26, 45);
      this.digitKey4.Name = "digitKey4";
      this.digitKey4.Size = new System.Drawing.Size(39, 34);
      this.digitKey4.TabIndex = 5;
      this.digitKey4.Text = " 4 ghi";
      this.digitKey4.UseVisualStyleBackColor = true;
      this.digitKey4.Click += new System.EventHandler(this.digitKey4_Click);
      // 
      // digitKey5
      // 
      this.digitKey5.Location = new System.Drawing.Point(80, 45);
      this.digitKey5.Name = "digitKey5";
      this.digitKey5.Size = new System.Drawing.Size(38, 34);
      this.digitKey5.TabIndex = 4;
      this.digitKey5.Text = "  5 jkl";
      this.digitKey5.UseVisualStyleBackColor = true;
      this.digitKey5.Click += new System.EventHandler(this.digitKey5_Click);
      // 
      // digitKey6
      // 
      this.digitKey6.Location = new System.Drawing.Point(135, 45);
      this.digitKey6.Name = "digitKey6";
      this.digitKey6.Size = new System.Drawing.Size(40, 34);
      this.digitKey6.TabIndex = 3;
      this.digitKey6.Text = "6 mno";
      this.digitKey6.UseVisualStyleBackColor = true;
      this.digitKey6.Click += new System.EventHandler(this.digitKey6_Click);
      // 
      // digitKey3
      // 
      this.digitKey3.Location = new System.Drawing.Point(135, 3);
      this.digitKey3.Name = "digitKey3";
      this.digitKey3.Size = new System.Drawing.Size(40, 36);
      this.digitKey3.TabIndex = 2;
      this.digitKey3.Text = " 3 def";
      this.digitKey3.UseVisualStyleBackColor = true;
      this.digitKey3.Click += new System.EventHandler(this.digitKey3_Click);
      // 
      // digitKey2
      // 
      this.digitKey2.Location = new System.Drawing.Point(80, 3);
      this.digitKey2.Name = "digitKey2";
      this.digitKey2.Size = new System.Drawing.Size(38, 36);
      this.digitKey2.TabIndex = 1;
      this.digitKey2.Text = "2 abc";
      this.digitKey2.UseVisualStyleBackColor = true;
      this.digitKey2.Click += new System.EventHandler(this.digitKey2_Click);
      // 
      // digitKey1
      // 
      this.digitKey1.Location = new System.Drawing.Point(26, 3);
      this.digitKey1.Name = "digitKey1";
      this.digitKey1.Size = new System.Drawing.Size(39, 36);
      this.digitKey1.TabIndex = 0;
      this.digitKey1.Text = "1";
      this.digitKey1.UseVisualStyleBackColor = true;
      this.digitKey1.Click += new System.EventHandler(this.digitKey1_Click);
      // 
      // clearButton
      // 
      this.clearButton.BackColor = System.Drawing.Color.OrangeRed;
      this.clearButton.Location = new System.Drawing.Point(256, 331);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(48, 30);
      this.clearButton.TabIndex = 19;
      this.clearButton.Text = "Clear";
      this.clearButton.UseVisualStyleBackColor = false;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // offhookButton
      // 
      this.offhookButton.BackColor = System.Drawing.Color.DarkSeaGreen;
      this.offhookButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.offhookButton.Location = new System.Drawing.Point(42, 266);
      this.offhookButton.Name = "offhookButton";
      this.offhookButton.Size = new System.Drawing.Size(46, 50);
      this.offhookButton.TabIndex = 20;
      this.offhookButton.Text = "GO";
      this.offhookButton.UseVisualStyleBackColor = false;
      this.offhookButton.Click += new System.EventHandler(this.offhookButton_Click);
      // 
      // OKbutton
      // 
      this.OKbutton.Location = new System.Drawing.Point(157, 292);
      this.OKbutton.Name = "OKbutton";
      this.OKbutton.Size = new System.Drawing.Size(55, 47);
      this.OKbutton.TabIndex = 21;
      this.OKbutton.Text = "OK";
      this.OKbutton.UseVisualStyleBackColor = true;
      this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
      // 
      // Cancel
      // 
      this.Cancel.BackColor = System.Drawing.Color.Red;
      this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.Cancel.Location = new System.Drawing.Point(280, 266);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new System.Drawing.Size(46, 50);
      this.Cancel.TabIndex = 22;
      this.Cancel.Text = "Esc";
      this.Cancel.UseVisualStyleBackColor = false;
      this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // buttonSettings
      // 
      this.buttonSettings.BackColor = System.Drawing.Color.DarkSeaGreen;
      this.buttonSettings.Location = new System.Drawing.Point(63, 332);
      this.buttonSettings.Name = "buttonSettings";
      this.buttonSettings.Size = new System.Drawing.Size(43, 29);
      this.buttonSettings.TabIndex = 23;
      this.buttonSettings.Text = "Menu";
      this.buttonSettings.UseVisualStyleBackColor = false;
      this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
      // 
      // leftNav
      // 
      this.leftNav.Location = new System.Drawing.Point(121, 301);
      this.leftNav.Name = "leftNav";
      this.leftNav.Size = new System.Drawing.Size(30, 29);
      this.leftNav.TabIndex = 24;
      this.leftNav.Text = "<";
      this.leftNav.UseVisualStyleBackColor = true;
      this.leftNav.Click += new System.EventHandler(this.leftNav_Click);
      // 
      // rightNav
      // 
      this.rightNav.Location = new System.Drawing.Point(218, 301);
      this.rightNav.Name = "rightNav";
      this.rightNav.Size = new System.Drawing.Size(30, 29);
      this.rightNav.TabIndex = 25;
      this.rightNav.Text = ">";
      this.rightNav.UseVisualStyleBackColor = true;
      this.rightNav.Click += new System.EventHandler(this.rightNav_Click);
      // 
      // upNav
      // 
      this.upNav.Location = new System.Drawing.Point(166, 266);
      this.upNav.Name = "upNav";
      this.upNav.Size = new System.Drawing.Size(38, 23);
      this.upNav.TabIndex = 26;
      this.upNav.Text = "^";
      this.upNav.UseVisualStyleBackColor = true;
      this.upNav.Click += new System.EventHandler(this.upNav_Click);
      // 
      // downNav
      // 
      this.downNav.Location = new System.Drawing.Point(166, 341);
      this.downNav.Name = "downNav";
      this.downNav.Size = new System.Drawing.Size(38, 23);
      this.downNav.TabIndex = 27;
      this.downNav.Text = "v";
      this.downNav.UseVisualStyleBackColor = true;
      this.downNav.Click += new System.EventHandler(this.downNav_Click);
      // 
      // button0
      // 
      this.button0.Location = new System.Drawing.Point(325, 13);
      this.button0.Name = "button0";
      this.button0.Size = new System.Drawing.Size(37, 23);
      this.button0.TabIndex = 28;
      this.button0.UseVisualStyleBackColor = true;
      this.button0.Click += new System.EventHandler(this.button0_Click);
      // 
      // button5
      // 
      this.button5.Location = new System.Drawing.Point(4, 125);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(37, 23);
      this.button5.TabIndex = 29;
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new System.EventHandler(this.button5_Click);
      // 
      // button6
      // 
      this.button6.Location = new System.Drawing.Point(325, 148);
      this.button6.Name = "button6";
      this.button6.Size = new System.Drawing.Size(37, 23);
      this.button6.TabIndex = 30;
      this.button6.UseVisualStyleBackColor = true;
      this.button6.Click += new System.EventHandler(this.button6_Click);
      // 
      // button7
      // 
      this.button7.Location = new System.Drawing.Point(4, 168);
      this.button7.Name = "button7";
      this.button7.Size = new System.Drawing.Size(37, 23);
      this.button7.TabIndex = 31;
      this.button7.UseVisualStyleBackColor = true;
      this.button7.Click += new System.EventHandler(this.button7_Click);
      // 
      // button8
      // 
      this.button8.Location = new System.Drawing.Point(325, 191);
      this.button8.Name = "button8";
      this.button8.Size = new System.Drawing.Size(37, 23);
      this.button8.TabIndex = 32;
      this.button8.UseVisualStyleBackColor = true;
      this.button8.Click += new System.EventHandler(this.button8_Click);
      // 
      // button10
      // 
      this.button10.Location = new System.Drawing.Point(325, 234);
      this.button10.Name = "button10";
      this.button10.Size = new System.Drawing.Size(37, 23);
      this.button10.TabIndex = 33;
      this.button10.UseVisualStyleBackColor = true;
      this.button10.Click += new System.EventHandler(this.button10_Click);
      // 
      // button9
      // 
      this.button9.Location = new System.Drawing.Point(4, 212);
      this.button9.Name = "button9";
      this.button9.Size = new System.Drawing.Size(37, 23);
      this.button9.TabIndex = 34;
      this.button9.UseVisualStyleBackColor = true;
      this.button9.Click += new System.EventHandler(this.button9_Click);
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(4, 82);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(37, 23);
      this.button3.TabIndex = 35;
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(325, 102);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(37, 23);
      this.button4.TabIndex = 36;
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new System.EventHandler(this.button4_Click);
      // 
      // PhoneForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.WhiteSmoke;
      this.CancelButton = this.Cancel;
      this.ClientSize = new System.Drawing.Size(365, 552);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button9);
      this.Controls.Add(this.button10);
      this.Controls.Add(this.button8);
      this.Controls.Add(this.button7);
      this.Controls.Add(this.button6);
      this.Controls.Add(this.button5);
      this.Controls.Add(this.button0);
      this.Controls.Add(this.downNav);
      this.Controls.Add(this.upNav);
      this.Controls.Add(this.rightNav);
      this.Controls.Add(this.leftNav);
      this.Controls.Add(this.buttonSettings);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.OKbutton);
      this.Controls.Add(this.offhookButton);
      this.Controls.Add(this.clearButton);
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
    private System.Windows.Forms.Button clearButton;
    private System.Windows.Forms.Button offhookButton;
    private System.Windows.Forms.Button OKbutton;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.Button buttonSettings;
    private System.Windows.Forms.Button leftNav;
    private System.Windows.Forms.Button rightNav;
    private System.Windows.Forms.Button upNav;
    private System.Windows.Forms.Button downNav;
    private System.Windows.Forms.Button button0;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button6;
    private System.Windows.Forms.Button button7;
    private System.Windows.Forms.Button button8;
    private System.Windows.Forms.Button button10;
    private System.Windows.Forms.Button button9;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button button4;
  }
}

