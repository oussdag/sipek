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

using MenuDesigner;
using System.Timers;
using System;

namespace Sipek
{

  #region Internationalization

  public class CEnLanguage : CLanguage
  {
    public CEnLanguage()
    {
      this.setDefaultKeyMapping();
      // no translation needed
    }
  }

  public class CDeLanguage : CLanguage
  {
    public CDeLanguage()
    {
      this.setDefaultKeyMapping();

      // some translations...
      _langTable.Add("languages", "Sprache");
      _langTable.Add("editing", "Editieren");
      _langTable.Add("back", "Zurueck");
      _langTable.Add("link", "Nachste");
      _langTable.Add("list", "Liste");
      _langTable.Add("hello world!", "Hallo Welt");

    }
  }

  #endregion Internationalization

  #region FactoryImplementation
  ///////////////////////////////////////////////////////////////////////////////////////
  /// <summary>
  /// 
  /// </summary>
  public class CFactoryImpl : CControllerFactory
  {
    public CFactoryImpl()
      : base()
    { }

    protected override CComponentTimer createTimer(VoidDelegate deleg)
    {
      return new CTimerImpl(deleg);
    }
/*    protected override CSynchronizer createSynchronizerImpl()
    {
      return new CSynchronizerImpl();
    }*/
  }

  /// <summary>
  /// 
  /// </summary>
  public class CTimerImpl : CComponentTimer
  {
    VoidDelegate mdeleg;
    Timer mtimer;
    int _pageId;

    public CTimerImpl(VoidDelegate deleg)
      : base()
    {
      mdeleg = deleg;
      mtimer = new Timer();
      mtimer.Elapsed += new ElapsedEventHandler(mtimer_Elapsed);
    }

    void mtimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      mtimer.Stop();      
      // check if page is not actual anymore
      if (_pageId != CComponentController.getInstance().Page.Id)
      {
        return;
      }

      if (mdeleg != null) mdeleg();
    }

    public override void startMillis(int timeout)
    {
      _pageId = CComponentController.getInstance().Page.Id;
      mtimer.Interval = timeout;
      mtimer.Start();
    }

    public override void abort()
    {
      mtimer.Stop();
    }
  }
  /*
  public class CSynchronizerImpl : CSynchronizer
  {
    public override void invoke(VoidIntDelegate del, int id)
    {
      //PhoneForm.PhoneFormInstance.Invoke(del, new object[] { id });
      del(id);
    }
  }
  */
  #endregion FactoryImplementation

  #region Rendering

  /// <summary>
  /// 
  /// </summary>
  public class Renderer : CObserver
  {

    private int maxLines = 10;
    private int maxColumns = 25;

    protected PhoneForm mform;

    delegate void writerDelegate(int xaxis, int posY, string text);
    delegate void setCursorDelegate(int xaxis, int yaxis);
    delegate void setSelectionDelegate(int xaxis, int yaxis, int length);

    public Renderer(PhoneForm form)
      : base()
    {
      mform = form;
    }

    private void formWriter(int xaxis, int yaxis, string text)
    {
      if ((yaxis <= maxLines) && (xaxis <= maxColumns) )
      {
        try
        {
          mform.Invoke(new writerDelegate(mform.writeText), new object[3] { xaxis, yaxis, text });
        }
        catch (Exception e)
        { 
        }
      }
    }

    private void formSetCursor(int xaxis, int yaxis)
    {
      mform.Invoke(new setCursorDelegate(mform.setCursor), new object[2] { xaxis, yaxis});
    }

    private void formSetSelection(int xaxis, int yaxis, int length)
    {
      mform.Invoke(new setSelectionDelegate(mform.setSelection), new object[3] { xaxis, yaxis, length});
    }

    /*
    private void formDrawButton(int xaxis, int yaxis)
    {
      mform.Invoke(new drawButtonDelegate(mform.drawButton), new object[2] { xaxis, yaxis});
    }

    private void formErasebutton()
    {
      mform.Invoke(new eraseButtonDelegate(mform.eraseButton));
    }
    */ 
    ///////////////////////////////////////////////////////////////////////////////////////

    public void drawText(int positionId, string caption, EAlignment justify)
    {
      int xaxis = 0;
      // clipping 
      if (positionId > maxLines) return;

      switch (justify)
      {
        case EAlignment.justify_right: xaxis = maxColumns  - caption.Length; break;
        case EAlignment.justify_center: xaxis = (maxColumns - caption.Length) / 2; break;
      }
      if (xaxis < 0) xaxis = 0;
      formWriter(xaxis, positionId, caption);
    }

    public void drawText(int posX, int posY, string caption)
    {
      if (posY > maxLines) return;

      formWriter(posX, posY, caption);
    }

    public void eraseText(int positionId, string caption, EAlignment justify)
    {
      int xaxis = 0;
      if (justify == EAlignment.justify_right) xaxis = maxColumns - caption.Length;
      if (justify == EAlignment.justify_center) xaxis = (maxColumns - caption.Length) / 2;
      if (xaxis < 0) xaxis = 0;
      formWriter(xaxis, positionId, new string(' ', caption.Length));
    }

    public void eraseText(int posX, int posY, string caption)
    {
      formWriter(posX, posY, new string(' ', caption.Length));
    }

    public void drawCheckMark(int posX, int posY, bool checkedFlag)
    {
      if (checkedFlag)
        drawText(posX, posY, "o");
      else
        drawText(posX, posY, " ");
    }
    public void eraseCheckMark(int posX, int posY)
    {
      drawText(posX, posY, " ");
    }

    public void drawSelection(int position, int wide)
    {
      for (int i = 0; i < wide; i++)
      {
        formSetSelection(0, position + i, maxColumns);
      }
    }

    public void eraseSelection(int position, int wide)
    {
      eraseText(0, position, " ");
    }

    public void clearScreen()
    {
      this.mform.clearScreen();
    }

    public void drawLink(int positionId, string caption, EAlignment justify)
    {
      this.drawText(positionId, caption, justify);
      int x = 0;
      if (justify == EAlignment.justify_right)
        x = maxColumns ;
      else if (justify == EAlignment.justify_center)
        x = (maxColumns) / 2;

      //formDrawButton(x, positionId);
    }

    public void eraseLink(int positionId, string caption, EAlignment justify)
    {
      this.eraseText(positionId, caption, justify);
      //formErasebutton();
    }

    public void drawEdit(int positionId, string prompt, string caption, int cursor_position, bool selected, EEditMode mode)
    {
      drawLink(positionId, prompt + caption, EAlignment.justify_left);

      if (selected)
      {
        switch (mode)
        {
          case EEditMode.alphanum_high:
            drawText(maxColumns - 3, 0, "ABC");
            break;
          case EEditMode.alphanum_low:
            drawText(maxColumns - 3, 0, "abc");
            break;
          case EEditMode.numeric:
            drawText(maxColumns - 3, 0, "123");
            break;
        }
      }
      if (selected) formSetCursor(prompt.Length + cursor_position, positionId);
    }

    public void eraseEdit(int positionId, string prompt, string caption, int cursor_position)
    {
      eraseText(positionId, prompt + caption, EAlignment.justify_left);
      eraseText(maxColumns - 3, 0, "   ");
    }


    public void drawScroller(int positionId, int scrollposition, int scrollSize)
    {
    }

    public void eraseScroller(int positionId, int scrollposition, int scrollSize)
    {
    }

    // advanced drawing interface...
    public void drawChar(int posX, int posY, char index)
    {
    }
    public void eraseChar(int posX, int posY)
    {
    }

    public void drawChar(int posX, int posY, ESymbol symbol)
    {
    }

    public void drawArrow(int posY, int side)
    {
    }
    public void eraseArrow(int posY, int side)
    {
    }

    public void drawCheckBox(int positionId, string caption, EAlignment justify, bool ischecked)
    {
      int alignPos = 0;
      if (EAlignment.justify_right == justify)
      {
        alignPos = caption.Length;
      }

      if (ischecked)
        drawText(positionId, caption.Insert(alignPos, "*"), justify);
      else
        drawText(positionId, caption.Insert(alignPos, " "), justify);
    }

    public void eraseCheckBox(int positionId, string caption, EAlignment justify)
    {
      int alignPos = 0;
      if (EAlignment.justify_right == justify)
      {
        alignPos = caption.Length;
      }
      eraseText(positionId, caption.Insert(alignPos, " "), justify);
    }


    public void drawTextBox(int posY, int lines, string caption, EAlignment alignmode)
    {
      // max lines
      int linesCnt = caption.Length / maxColumns;
      for (int i = 0; i < linesCnt; i++)
      {
        drawText(i + posY, caption.Substring(i*maxColumns, maxColumns), alignmode);
      }
      drawText(posY + linesCnt, caption.Substring(linesCnt * maxColumns, caption.Length % maxColumns), alignmode);
    }

    public void eraseTextBox(int posY, int lines, string caption, EAlignment alignmode)
    {
      int linesCnt = caption.Length / maxColumns;
      for (int i = 0; i < linesCnt; i++)
      {
        string subniz = caption.Substring(i * maxColumns, maxColumns);
        eraseText(i + posY, subniz, alignmode);
      }
      eraseText(posY + linesCnt, caption.Substring(linesCnt * maxColumns, caption.Length % maxColumns), alignmode);
    }

    public void drawEditBox(int posY, int lines, string prompt, string captionOrg, int cursor_position, bool selected, EEditMode mode)
    {
      string caption = prompt + captionOrg;
      drawTextBox(posY, lines, caption, EAlignment.justify_left);
      if (selected)
      {
        switch (mode)
        {
          case EEditMode.alphanum_high:
            drawText(maxColumns - 3, 0, "ABC");
            break;
          case EEditMode.alphanum_low:
            drawText(maxColumns - 3, 0, "abc");
            break;
          case EEditMode.numeric:
            drawText(maxColumns - 3, 0, "123");
            break;
        }
      }
      if (selected) formSetCursor(prompt.Length + cursor_position % maxColumns, posY + (cursor_position/maxColumns));
    }

    public void eraseEditBox(int posY, int lines, string prompt, string captionOrg, int cursor_position)
    {
      string caption = prompt + captionOrg;
      eraseTextBox(posY, lines, caption, EAlignment.justify_left);
      eraseText(maxColumns - 3, 0, "   ");
    }

  }
  #endregion Rendering


} // namespace Sipek
