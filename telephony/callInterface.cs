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


namespace Telephony
{

  interface CTelephonyInterface
  {
    int makeCall(string dialedNo, int accountId);

    bool endCall();

    bool alerted();

    bool acceptCall();

    bool holdCall();

    bool retrieveCall();

    bool xferCall(string number);

    bool xferCallSession(int session);

    bool threePtyCall(int session);

    //bool serviceRequest(EServiceCodes code, int session);
    bool serviceRequest(int code, string dest);

    bool dialDtmf(int mode, string digits);
  }

  interface CTelephonyCallback
  {
    #region Methods

    void incomingCall( string callingNo);

    void onAlerting();

    void onConnect();

    void onReleased();

    void onHoldConfirm();
    
    #endregion Methods
  }

} // namespace Sipek
