/* 
 * Copyright (C) 2007 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 */

using System;
using System.Windows.Forms;

using MenuDesigner;
using Sipek;
using Telephony;
using System.Collections.Generic;

namespace Sipek
{
  public partial class PhoneForm : Form
  {
    private static PhoneForm _form = null;

    public static PhoneForm PhoneFormInstance
    {
      get 
      {
        return _form; 
      }
    }

    protected CComponentController control;
    private int _caretPos = -1;
    private int _selection = -1;

    // languages
    public static CLanguage _langEN = new CEnLanguage();
    public static CDeLanguage _langDE = new CDeLanguage();

    /// <summary>
    /// 
    /// </summary>
    public PhoneForm()
    {
      _form = this;
      InitializeComponent();

      ///////////////////////////////////////////////////////////////
      // sasacoh coding
      ///////////////////////////////////////////////////////////////
      string str = new string(' ', 255);
      richTextBox1.Lines = new string[] { str, str, str, str, str, str, str, str, str, str, str };
      //_buttons = new System.Collections.Generic.List<Button>();

      control = CComponentController.getInstance();

      Renderer renderer = new Renderer(this);
      CFactoryImpl factory = new CFactoryImpl();

      control.attach(renderer);
      control.Factory = factory;
      control.Language = _langEN;

      // Create menu pages...
      new CInitPage();
      new CIdlePage();
      new CPhonebookPage();
      new CPhonebookEditPage();
      new CMenuPage();
      new CSIPSettings();
      new CSIPProxySettings();
      new CSIPProxySettings2nd();
      new CSIPProxySettings3rd();
      new CRingModePage();
      new CCalllogPage();
      new CAccountsPage();
      new CServicesPage();
      new CRedirectPage();
      new CMessageBoxPage();
      new CMessageReceivedPage();
      
      control.initialize();

      // register callback
      Telephony.CCallManager.getInstance().CallStateChanged += onTelephonyRefresh;

      // set active page...
      control.showPage((int)EPages.P_INIT);
    }

    delegate void RefreshDelegate();

    public void onTelephonyRefresh()
    {
      if (this.Created)
        this.Invoke(new RefreshDelegate(this.RefreshForm));
    }

    public void RefreshForm()
    {
      int currentSession = CCallManager.getInstance().getCurrentCallIndex();

      // get current session
      if (currentSession < 0)
      {
        // todo::: showHomePage
        CComponentController.getInstance().showPage(CComponentController.getInstance().HomePageId);
        return;
      }

      Dictionary<int, CStateMachine> calls = CCallManager.getInstance().getCallList();
      CStateMachine call = CCallManager.getInstance().getCurrentCall();
      if (call != null)
      {
        int stateId = (int)call.getStateId();
        if (stateId == (int)EStateId.IDLE)
        {
          if (CCallManager.getInstance().Count == 0)
          {
            CComponentController.getInstance().showPage(stateId);
          }
          else
          {
            calls.GetEnumerator().MoveNext();
            currentSession = calls.GetEnumerator().Current.Key;

            CStateMachine nextcall = CCallManager.getInstance().getCall(currentSession);
            if (nextcall != null) CComponentController.getInstance().showPage((int)nextcall.getStateId());
          }
        }
        else
        {
          CComponentController.getInstance().showPage(stateId);
        }
      }    
    }


    public void clearScreen()
    {
      _caretPos = -1;
      _selection = -1;
    }

    public void writeText(int xaxis, int yaxis, String text)
    {
      string[] tempArray = richTextBox1.Lines;
      string line = tempArray[yaxis];
      line = line.Remove(xaxis, text.Length);
      tempArray[yaxis] = line.Insert(xaxis, text);
      richTextBox1.Lines = tempArray;
      // use stored value to prevent selection overridance
      if (_caretPos >= 0) richTextBox1.Select(_caretPos, 1);
      if (_selection >= 0) richTextBox1.Select(_selection, 23);

      //this.richTextBox1.Focus();
    }

    public void setCursor(int xaxis, int yaxis)
    {
      _caretPos = yaxis * 256 + xaxis;
      richTextBox1.Select(_caretPos, 1); // simulate caret
    }

    public void setSelection(int startX, int startY, int length)
    {
      _selection = startX + startY * 256;
      richTextBox1.Select(_selection, length);
    }

    ////////////////////////////////////////////////////////////////////////
    private void digitKey1_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_1);
      richTextBox1.Focus();
    }

    private void digitKey2_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_2);
      richTextBox1.Focus();
    }

    private void digitKey3_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_3);
      richTextBox1.Focus();
    }

    private void digitKey4_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_4);
      richTextBox1.Focus();
    }

    private void digitKey5_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_5);
      richTextBox1.Focus();
    }

    private void digitKey6_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_6);
      richTextBox1.Focus();
    }

    private void digitKey7_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_7);
      richTextBox1.Focus();
    }

    private void digitKey8_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_8);
      richTextBox1.Focus();
    }

    private void digitKey9_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_9);
      richTextBox1.Focus();
    }

    private void digitKey0_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_0);
      richTextBox1.Focus();
    }

    private void digitKeyS_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_star);
      richTextBox1.Focus();
    }

    private void digitKeyH_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDigitKey((int)ENumKeyTags.NumKey_hash);
      richTextBox1.Focus();
    }

    private void OKbutton_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onOkKey();
      richTextBox1.Focus();
    }

    private void onhookButton_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onOnHookKey();
      richTextBox1.Focus();
    }

    private void offhookButton_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onOffHookKey();
      richTextBox1.Focus();
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onEscKey();
      richTextBox1.Focus();
    }

    private void clearButton_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onClearKey();
      richTextBox1.Focus();
    }

    private void buttonSettings_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onMenuKey();
      richTextBox1.Focus();
    }    
    
    private void PhoneForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      control.shutdown();
      CCallManager.getInstance().shutdown();
    }

    private void PhoneForm_Shown(object sender, EventArgs e)
    {
      CComponentController.getInstance().HomePageId = (int)EPages.P_IDLE;
      // set home page...
      CComponentController.getInstance().showPage((int)EPages.P_IDLE);
    }

    private void rightNav_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onRightKey();
      richTextBox1.Focus();
    }

    private void leftNav_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onLeftKey();
      richTextBox1.Focus();
    }

    private void downNav_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onDownKey();
      richTextBox1.Focus();
    }

    private void upNav_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onUpKey();
      richTextBox1.Focus();
    }

    private void button0_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(0);
      richTextBox1.Focus();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(3);
      richTextBox1.Focus();
    }

    private void button4_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(4);
      richTextBox1.Focus();
    }
    
    private void button5_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(5);
      richTextBox1.Focus();
    }

    private void button6_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(6);
      richTextBox1.Focus();
    }

    private void button7_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(7);
      richTextBox1.Focus();
    }

    private void button8_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(8);
      richTextBox1.Focus();
    }

    private void button9_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(9);
      richTextBox1.Focus();
    }

    private void button10_Click(object sender, EventArgs e)
    {
      control.getAccessIf().onSoftKey(10);
      richTextBox1.Focus();
    }

    private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == 0x08)
      {
        clearButton_Click(sender, e);
      }
      else if (e.KeyChar == 0x0D)
      { 
        OKbutton_Click(sender, e);
      }
      else /*if ((e.KeyChar >= 'A') && (e.KeyChar <= 'z'))*/
      {
        control.getAccessIf().onCharKey(e.KeyChar);
      }
      
    }

/*    private void richTextBox1_Leave(object sender, EventArgs e)
    {
      richTextBox1.Focus();
    }
 */ 

    private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyValue == 37)
      {
        leftNav_Click(sender, e);
      }
      else if (e.KeyValue == 39)
      {
        rightNav_Click(sender, e); 
      }
      else if (e.KeyValue == 38)
      {
        upNav_Click(sender, e);
      }
      else if (e.KeyValue == 40)
      {
        downNav_Click(sender, e);
      }
    }
  }
}