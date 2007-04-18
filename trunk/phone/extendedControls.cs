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
using System.Collections.Generic;
using System.Text;
using MenuDesigner;

namespace Sipek
{
  public class CExtendedPage : CPage
  {
    private List<CPage> _embeddedPages;
    private int _currentIndex = -1;
    private CLink _linkNext;
    private bool _internalMove = false;

    public CExtendedPage(int pageId)
      : base(pageId)
    {
      _embeddedPages = new List<CPage>();

      _linkNext = new CLink("Next");
      _linkNext.PosY = 10;
      _linkNext.Align = EAlignment.justify_right;
      add(_linkNext);
    }

    public override void onDraw(CObserver renderer)
    {
      if (_currentIndex < 0) return;
      _linkNext.onDraw(renderer);
      _embeddedPages[_currentIndex].onDraw(renderer);
    }

    public override void onErase(CObserver renderer)
    {
      _linkNext.onErase(renderer);
      foreach (CPage item in _embeddedPages)
      {
        item.onErase(renderer);
      }
    }

    public override void add(CComponent component)
    {
    }

    public override void remove(CComponent component)
    {
    }

    public void add(CPage page)
    {
      _embeddedPages.Add(page);

      _currentIndex = 0;
      base.add(page);
    }

    public void remove(CPage page)
    {
      _embeddedPages.Remove(page);
      base.remove(page);
    }

    public override void onEntry()
    {
      if (!_internalMove) base.onEntry();
    }
    public override void onExit()
    {
      if (!_internalMove) base.onExit();
    }

    public override bool onRightKey()
    {
      bool status = _embeddedPages[_currentIndex].onRightKey();
      if (status) return true;

      forward();
      
      _controller.showPage(this.Id);

      return status;
    }

    public override bool onLeftKey()
    {
      bool status = _embeddedPages[_currentIndex].onLeftKey();
      if (status) return true;

      this.backward();
      
      _controller.showPage(this.Id);

      return status;
    }

    private void forward()
    {
      if (_currentIndex < 0) return;

      if (_currentIndex < _embeddedPages.Count - 1)
      {
        _currentIndex++;
      }
      else
      {
        _currentIndex = 0;
      }
      _internalMove = true;
    }

    private void backward()
    {
      if (_currentIndex < 0) return ;

      if (_currentIndex > 0)
      {
        _currentIndex--;
      }
      else
      {
        _currentIndex = _embeddedPages.Count - 1;
      }
      _internalMove = true;
    }

    public override bool onSoftKey(int id)
    {
      if (id == 10)
      {
        forward();
        _controller.showPage(this.Id);
        return true;
      }
      return _embeddedPages[_currentIndex].onSoftKey(id);
    }

    public override bool onDigitKey(int id)
    {
      return _embeddedPages[_currentIndex].onDigitKey(id);
    }
    
    public override bool onCharKey(int id)
    {
      return _embeddedPages[_currentIndex].onCharKey(id);
    }

    public override bool onClearKey()
    {
      return _embeddedPages[_currentIndex].onClearKey();
    }
    
  }

  /// <summary>
  /// 
  /// </summary>
  public class CDoubleLink : CLink
  {
    private CLink _link1;
	  private CLink _link2;

    public CDoubleLink(string caption_1, string caption_2)
      : this(caption_1, caption_2, EAlignment.justify_left, EAlignment.justify_left)
    {
    }

    public CDoubleLink(string caption_1, string caption_2, EAlignment alignmode1, EAlignment alignmode2)
      : base("")
    {
      _link1 = new CLink(caption_1);
      _link1.Align = alignmode1;
	    _link2 = new CLink(caption_2);
      _link2.Align = alignmode2;
    }

    // overriden
    public override int Size
    {
	    get { return 2; }
    }

    public override int PosY
    {
      get
      {
        return base.PosY;
      }
      set
      {
        base.PosY = value;
	      _link1.PosY = value;
	      _link2.PosY = value + 1;
      }
    }

    public override void  onDraw(CObserver renderer)
    {
      renderer.drawText(PosY, _controller.translate(_link1.Caption), EAlignment.justify_left);
      renderer.drawText(PosY + 1, _controller.translate(_link2.Caption), EAlignment.justify_left);
      if ((_link1.Caption.Substring(0,3) != "---")||(_link2.Caption.Substring(0,3) != "---"))
      {
        renderer.drawLink(PosY, "", EAlignment.justify_left);
        renderer.drawLink(PosY + 1, "", EAlignment.justify_right);
      }
    }

    public override void onErase(CObserver renderer)
    {
      renderer.eraseText(PosY, _controller.translate(_link1.Caption), EAlignment.justify_left);
      renderer.eraseText(PosY + 1, _controller.translate(_link2.Caption), EAlignment.justify_left);

      if (((_link1.Caption).Substring(0, 3) != "---") || ((_link2.Caption).Substring(0, 3) != "---"))
      {
        renderer.eraseLink(PosY, "", EAlignment.justify_left);
        renderer.eraseLink(PosY + 1, "", EAlignment.justify_right);
      }
    }

    public override bool onSoftKey(int id)
    {
	    if (id == _link1.LinkKey) 
	    {
		    return base.onSoftKey(id);
	    }
	    else if  (id == _link2.LinkKey) 
	    {
		    return base.onSoftKey(id-1);
	    }
	    return false;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// 
  public class CDoubleDecorator : CDecorator
  {
    private CText _text;

    public CDoubleDecorator(CEditField decoratee) 
      : base(null)
    {
      // create list that will hold two controls (text and edit)
	    CComponentList list = new CComponentList();
	    // set list position 
	    list.PosY = decoratee.PosY;

	    // take prompt data from decoratee and put it into text control
	    int pos = decoratee.PosY;
	    _text = new CText(decoratee.Prompt);
	    _text.PosY = pos;

	    // put decoratee (edit) under text control and erase prompt
	    decoratee.PosY = pos + 1;
	    decoratee.Prompt = "";

	    // add both controls
	    list.add(_text);
	    list.add(decoratee);
	    // set list as decoratee
	    _component = list;
    }

	  // overridden 
	  public override bool  onSoftKey(int id)
    {
    	int pos = PosY;
	    if (pos == id)
	    {
		    // handle menu key press as one line lower...
		    base.onSoftKey(pos+1);
		    return false;
	    }
      return base.onSoftKey(id);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  class CStatusBar : CText
  {
    private int _status = 0;

    public CStatusBar() 
      : this(EAlignment.justify_right)
    {
    }

    public CStatusBar(EAlignment alignmode)
      : base("    ",alignmode)
    {
    }

    public void resetStatus()
    {
	    _status = 0;
	    renderStatus();
    }

    public void addStatus(EStatusFlag status, bool value)
    {
	    if (value == true) 
		    _status = _status | (int)status;
	    else
		    _status = _status & ((int)status ^ 0xF);

	    renderStatus();
    }

    public void setStatus(int value)
    {
	    _status = value;
	    renderStatus();
    }

    protected void renderStatus()
    {
	    if (_caption.Length < 4) return;

      if ((_status & (int)EStatusFlag.ERegStatus) > 0)
	    {
		    //_caption[0] = 'R';
        _caption = _caption.Remove(0, 1);
        _caption = _caption.Insert(0, "R");
	    }
	    else
	    {
		    //_caption[0] = 'F';
        _caption = _caption.Remove(0, 1);
        _caption = _caption.Insert(0, "R");
      }
	    // check direct call flag
      if ((_status & (int)EStatusFlag.EDirectCall) > 0)
	    {
		    //_caption[1] = 'D';
        _caption = _caption.Remove(1, 1);
        _caption = _caption.Insert(1, "D");
	    }
	    else
	    {
		    // check keyboard locked flag
        if ((_status & (int)EStatusFlag.ELocked) > 0)
		    {
			    //_caption[1] = 'L';
          _caption = _caption.Remove(1, 1);
          _caption = _caption.Insert(1, "L");
		    }
		    else
		    {
			    //_caption[1] = ' ';
          _caption = _caption.Remove(1, 1);
          _caption = _caption.Insert(1, " ");
		    }
	    }

	    // check direct call flag
      if ((_status & (int)EStatusFlag.EAlarmMissed) > 0)
	    {
		    //_caption[2] = 'A';
        _caption = _caption.Remove(2, 1);
        _caption = _caption.Insert(2, "A");
	    }
	    else
	    {
		    // check keyboard locked flag
        if ((_status & (int)EStatusFlag.ECallMissed) > 0)
		    {
			    //_caption[2] = 'C';
          _caption = _caption.Remove(2, 1);
          _caption = _caption.Insert(2, "C");
		    }
		    else
		    {
			    //_caption[2] = ' ';
          _caption = _caption.Remove(2, 1);
          _caption = _caption.Insert(2, " ");
		    }
	    }

	    // check incoming call status
      if ((_status & (int)EStatusFlag.EIncomingCallDisabled) > 0)
	    {
		    //_caption[3] = 'I';
        _caption = _caption.Remove(3, 1);
        _caption = _caption.Insert(3, "I");
	    }
	    else
	    {
		    // check for silent call status (lower priority than EIncomingCallDisabled)
        if ((_status & (int)EStatusFlag.ESilent) > 0)
		    {
			    //_caption[3] = 'S';
          _caption = _caption.Remove(3, 1);
          _caption = _caption.Insert(3, "S");
		    }
		    else
		    {
			    //_caption[3] = ' ';
          _caption = _caption.Remove(3, 1);
          _caption = _caption.Insert(3, " ");
		    }
	    }
    }
  }

  public class CEditBox : CEditField
  {
    public CEditBox(string prompt, string caption)
      : base(prompt, caption)
    { }
  }

  public class CTextBox : CText
  {
    public CTextBox(string text)
      : base(text)
    { }
  }


  ////////////////////////////////////////////
  // Buddy list
  
  class CBuddyMessage
  {
    private DateTime _datetime;
    private string _text;

    public string Content
    {
      get { return _text; }
    }

    public CBuddyMessage(DateTime datetime, string text)
    {
      _text = text;
      _datetime = datetime;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  class CBuddyItem
  {
    private int _id;
    private string _uri;
    private List<CBuddyMessage> _messageList;

    public int Id
    {
      get { return _id; }
      set {_id = value; }
    }

    public string Uri
    {
      get { return _uri; }
      set { _uri = value; }
    }

    public string LastMessage
    {
      get { return _messageList[_messageList.Count - 1].Content; }
    }

    public CBuddyMessage this [int index]
    {
      get { return _messageList[index]; }
    }


    public CBuddyItem(int id, string uri)
    {
      _id = id;
      _uri = uri;
      _messageList = new List<CBuddyMessage>();
    }

    public void addMessage(DateTime datetime, string text)
    {
      CBuddyMessage msg = new CBuddyMessage(datetime, text);
      _messageList.Add(msg);
    }

    public void clearAllMessages()
    {
      _messageList.Clear();
    }
  }

  /// <summary>
  /// 
  /// </summary>
  class CBuddyList
  {
    private Dictionary<int, CBuddyItem> _buddyList;

    // Singleton 
    private static CBuddyList _instance = null;
    public static CBuddyList getInstance()
    { 
      if (_instance == null)
      {
        _instance = new CBuddyList();
      }
      return _instance;
    }

    protected CBuddyList()
    {
      _buddyList = new Dictionary<int, CBuddyItem>();
    }

    /////////////////////////////////////////////////////////////////////////

    public CBuddyItem this[int index]
    {
      get { return _buddyList[index]; }
    }

    public int Count
    {
      get { return _buddyList.Count; }
    }
    /////////////////////////////////////////////////////////////////////////

    public void addBuddy(int id, string uri)
    { 
      CBuddyItem buddy = new CBuddyItem(id, uri);
      _buddyList.Add(id, buddy);
    }

    public void removeBuddy(int id)
    {
      _buddyList.Remove(id);
    }

    public void removeAll()
    {
      _buddyList.Clear();
    }
  
  }
}
