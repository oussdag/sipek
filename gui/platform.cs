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
using System.Windows.Forms;
using System;

namespace Gui
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

  #region TimerImplementation
  ///////////////////////////////////////////////////////////////////////////////////////
  /// <summary>
  /// 
  /// </summary>
  public class CTimerFactoryImpl : CTimerFactory
  {
    public CTimerFactoryImpl()
      : base()
    { }

    protected override CComponentTimer createTimer(NoParamDelegate deleg)
    {
      return new CTimerImpl(deleg);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CTimerImpl : CComponentTimer
  {
    NoParamDelegate mdeleg;
    Timer mtimer;

    public CTimerImpl(NoParamDelegate deleg)
      : base()
    {
      mdeleg = deleg;
      mtimer = new Timer();
      mtimer.Tick += new EventHandler(timeout);
    }

    public override void startMillis(int timeout)
    {
      mtimer.Interval = timeout;
      mtimer.Start();
    }

    public override void abort()
    {
      mtimer.Stop();
    }

    private void timeout(Object obj, EventArgs args)
    {
      this.abort();
      if (mdeleg != null) mdeleg();
    }
  }

  #endregion TimerImplementation

  #region Rendering

  /// <summary>
  /// 
  /// </summary>
  public class Renderer : CObserver
  {

    private int maxLines = 10;
    private int maxColumns = 24;

    protected PhoneForm mform;

    public Renderer(PhoneForm form)
      : base()
    {
      mform = form;
    }

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
      this.mform.writeText(xaxis, positionId, caption);
    }

    public void drawText(int posX, int posY, string caption)
    {
      if (posY > maxLines) return;

      this.mform.writeText(posX, posY, caption);
    }

    public void drawEdit(int positionId, string prompt, string caption, int cursor_position, bool selected, EEditMode mode)
    {
      drawText(positionId, prompt + caption, EAlignment.justify_left);

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
      mform.setCursor(prompt.Length + cursor_position, positionId);
    }

    public void eraseText(int positionId, string caption, EAlignment justify)
    {
      int xaxis = 0;
      if (justify == EAlignment.justify_right) xaxis = maxColumns - caption.Length;
      //this.mform.eraseText(xaxis, positionId, caption);
      this.mform.writeText(xaxis, positionId, new string(' ', caption.Length));
    }

    public void eraseText(int posX, int posY, string caption)
    {
      this.mform.writeText(posX, posY, new string(' ', caption.Length));
    }

    public void eraseEdit(int positionId, string prompt, string caption, int cursor_position)
    {
      eraseText(positionId, prompt + caption, EAlignment.justify_left);
      eraseText(19, 0, "   ");
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

    public void drawSelection(int position, int type)
    {
      mform.setSelection(0, position, maxColumns);
    }

    public void eraseSelection(int position, int type)
    {
      eraseText(0, position, " ");
    }

    public void clearScreen()
    {
      this.mform.clearScreen();
    }

    public void drawLink(int posX, int posY)
    {
    }
    public void eraseLink(int posX, int posY)
    {
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

  }
  #endregion Rendering


} // namespace Gui
