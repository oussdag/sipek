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

}
