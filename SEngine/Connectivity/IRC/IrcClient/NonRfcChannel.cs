/*
 * $Id: NonRfcChannel.cs 274 2008-06-02 19:10:11Z meebey $
 * $URL: svn+ssh://svn.qnetp.net/svn/smartirc/SmartIrc4net/trunk/src/IrcClient/NonRfcChannel.cs $
 * $Rev: 274 $
 * $Author: meebey $
 * $Date: 2008-06-02 21:10:11 +0200 (Mon, 02 Jun 2008) $
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2005 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 * 
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections;

namespace Meebey.SmartIrc4net
{
	/// <summary>
	/// 
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public class NonRfcChannel : Channel
	{
		//private Hashtable _Halfops = Hashtable.Synchronized(new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer()));
		private Hashtable _Halfops = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"> </param>
		internal NonRfcChannel(string name)
			: base(name)
		{
		}

#if DEBUG
		~NonRfcChannel()
		{
			Console.WriteLine("NonRfcChannel (" + Name + ") destroyed");
		}
#endif

		/// <summary>
		/// 
		/// </summary>
		/// <value> </value>
		public Hashtable Halfops
		{
			get
			{
				return (Hashtable)_Halfops.Clone();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value> </value>
		internal Hashtable UnsafeHalfops
		{
			get
			{
				return _Halfops;
			}
		}
	}
}
