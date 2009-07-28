//
// RubyProjectConfiguration.cs: configuration for a RubyProject
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
//
//
// Copyright (C) 2009 Levi Bard
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 


using System;
using System.Collections;

using MonoDevelop.Core.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.RubyBinding
{
	public class RubyProjectConfiguration: ProjectConfiguration
	{
		[ItemProperty("MainFile")]
		public string MainFile{ get; set; }
		
		[ItemProperty ("LoadPaths")]
		[ItemProperty ("LoadPath", Scope = "*", ValueType = typeof(string))]
		public ArrayList LoadPaths{ get; set; }
		
		public RubyProjectConfiguration (): base()
		{
			LoadPaths = new ArrayList ();
		}
		
		public override void CopyFrom (ItemConfiguration configuration)
		{
			try {
				base.CopyFrom (configuration);
				RubyProjectConfiguration conf = (RubyProjectConfiguration)configuration;
				MainFile = conf.MainFile;
				LoadPaths = new ArrayList ();
				if (null != conf.LoadPaths) {
					foreach (object path in conf.LoadPaths) {
						if (!string.IsNullOrEmpty ((string)path)) {
							LoadPaths.Add (path);
						}
					}
				}
			} catch (Exception e) {
				MessageService.ShowException (e);
			}
		}// CopyFrom
	}// RubyProjectConfiguration
}
