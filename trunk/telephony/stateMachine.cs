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
  /// <summary>
  /// CStateMachine class is a telephony data container for one call. It maintains call state, 
  /// communicates with signaling via proxy and informs GUI about signaling events.
  /// A Finite State Machine is implemented upon State design pattern!
  /// </summary>
  public class CStateMachine
  {
    #region Variables

    private CAbstractState _state;

    private CSipProxy _sigproxy;

    #endregion Variables

    #region Properties
    public CSipProxy SigProxy
    {
      get { return _sigproxy; } 
    }

    #endregion

    #region Constructor

    public CStateMachine(CSipProxy proxy)
    {
      _sigproxy = proxy;
    }

    #endregion Constructor


    #region Methods

    public CAbstractState getState()
    {
      return _state;
    }

    public void changeState(CAbstractState state)
    {
      _state = state;
    }


    public void changeState(CAbstractState.EStateId stateId)
    {
      //_state = state;
    }


    public void updateGui()
    {

    }

    public void destroy()
    { 
    }

    #endregion Methods
  }

} // namespace Telephony
