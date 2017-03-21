﻿//
//  X-RunUO - Ultima Online Server Emulator
//  Copyright (C) 2015 Pedro Pardal
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace Server.Network
{
	public delegate void OnPacketReceive( NetState state, PacketReader pvSrc );
	public delegate bool ThrottlePacketCallback( NetState state );

	public class PacketHandler
	{
		private int m_PacketID;
		private int m_Length;
		private bool m_Ingame;
		private OnPacketReceive m_OnReceive;
		private ThrottlePacketCallback m_ThrottleCallback;

		public PacketHandler( int packetID, int length, bool ingame, OnPacketReceive onReceive )
		{
			m_PacketID = packetID;
			m_Length = length;
			m_Ingame = ingame;
			m_OnReceive = onReceive;
		}

		public int PacketID
		{
			get
			{
				return m_PacketID;
			}
		}

		public int Length
		{
			get
			{
				return m_Length;
			}
		}

		public OnPacketReceive OnReceive
		{
			get
			{
				return m_OnReceive;
			}
		}

		public ThrottlePacketCallback ThrottleCallback
		{
			get { return m_ThrottleCallback; }
			set { m_ThrottleCallback = value; }
		}

		public bool Ingame
		{
			get
			{
				return m_Ingame;
			}
		}
	}
}