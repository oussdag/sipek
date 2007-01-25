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

namespace Telephony
{
  public enum ECallPages : int
  {
    P_DIALING = 100,
    P_CALLING,
    P_CONNECTING,
    P_RINGING,
    P_ACTIVE,
    P_RELEASED,
    P_INCOMING
  }

  public abstract class CTelephonyPage : CPage
  {

    protected CText _info;
	  protected CText _digits;
	  protected CText _sessions;
  	

    public CTelephonyPage(ECallPages pageId, string pageName) 
      : base((int)pageId, false,true)
	  {
      setText(pageName);

	    CText title = new CText(pageName);
	    title.PosY = 0;
	    add(title);

	    CLink end_call = new CLink("End Call");
      end_call.Align = EAlignment.justify_right;
      end_call.Softkey += new UintDelegate(endCallHandler);
	    end_call.PosY = 9;
      end_call.LinkKey = end_call.PosY;
	    this.add(end_call);

	    _info = new CText("");
      _info.Align = EAlignment.justify_center;
	    _info.PosY = 7;
	    this.add(_info);

	    _digits = new CText("");
      _digits.Align = EAlignment.justify_center;
	    _digits.PosY = 5;
	    this.add(_digits);

	    // sessions link
	    CLink sesLink = new CLink("0/0");
      sesLink.Align = EAlignment.justify_right;
      sesLink.Softkey += new UintDelegate(sessionHandler);
	    sesLink.PosY = 0;
	    this.add(sesLink);

	    // assign page handlers....
      this.Onhook += new NoParamDelegate(onhookHandler);
      this.Speaker +=new NoParamDelegate(onspeakerHandler);
      this.Menu += new NoParamDelegate(menuHandler);

	  }

    public override void onEntry() 
    { 
      base.onEntry();
    	// get current call instance
	    //_currentCall = muiHandler->getCurrentStateMachine();
      CStateMachine currentCall = CCallManager.getInstance().getCurrentCall();

	    // info...
	    string sinfo = ""/*currentCall->GetCallInfo()*/;
	    _info.Caption = sinfo;

	    // digits...
	    string digits = "";
	    if (currentCall.Incoming)
	    {
		    digits = currentCall.CallingNo;
	    }
	    else
	    {
        digits = currentCall.DialedNo;
	    }
 
	    _digits.Caption = digits;

	    // call sessions...
	    //char buf[6] = {0};
	    //sprintf(buf, "%d/%d", muiHandler->getCurrentSID(), muiHandler->getUISessionsNum());
	    //_sessions->setCaption(buf);
    }


    bool endCallHandler(int id) 
    {
      return onhookHandler(); 
    }

    bool menuHandler() 
    { 
      return true; 
    }

    bool onhookHandler() 
    {
      CCallManager.getInstance().destroySession();
      return true; 
    }

    public virtual bool onspeakerHandler() 
    { 
      return true; 
    }

    bool sessionHandler(int id) 
    { 
      return true; 
    }

	  // services
    bool holdHandler(int id) 
    { 
      return true; 
    }

    bool transferHandler(int id) 
    { 
      return true; 
    }

    bool threeptyHandler(int id) 
    { 
      return true; 
    }

    bool completionHandler(int id) 
    { 
      return true; 
    }

    bool newcallHandler(int id) 
    { 
      return true; 
    }

    bool muteHandler(int id) 
    { 
      return true; 
    }

    bool threeptysplitHandler(int id) 
    { 
      return true; 
    }

    bool swapHandler(uint id) 
    { 
      return true; 
    }


	  //string mPageName;
	  // current call
  //	UIStateMachine* currentCall;
  //	UICanvasHandler* muiHandler;
  }



  /////////////////////////////////////////////////////////////////////////////////////////


  public class CDialPage : CTelephonyPage
  {
    protected CEditField mEditField;
    //private CPhonebookPage _phbpage;

    public CDialPage() 
      : base(ECallPages.P_DIALING, "Dialing...")
	  {
		  
	  }

	  public void setDigits(string digits) 
    {
		  mEditField.Caption = digits;
	  }

	  public void makeCall(string number)
    {
    }

    ////////////////////////////////////////////////////////////////////

	  private bool okHandler()
    {
      return true;
    }

	  private bool phbHandler(int id)
    {
      return true;
    }

    private bool digitHandler(int id)
    {
      return true;
    }

  }

  /////////////////////////////////////////////////////////////////////////////////////////
  /// <summary>
  /// 
  /// </summary>
  public class CConnectingPage : CTelephonyPage
  {
    public CConnectingPage()
      : base(ECallPages.P_CONNECTING, "Connecting...")
    {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CRingingPage : CTelephonyPage
  {
    public CRingingPage() 
      : base(ECallPages.P_RINGING, "Calling...")
	  {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CReleasedPage : CTelephonyPage
  {
    public CReleasedPage()
      : base(ECallPages.P_RELEASED, "Released...")
    {
    }

  }

  /// <summary>
  /// 
  /// </summary>
  public class CActivePage : CTelephonyPage
  {
    public CActivePage()
      : base(ECallPages.P_ACTIVE, "Connected...")
    {
    }

  }

    


} // namespace Telephony
