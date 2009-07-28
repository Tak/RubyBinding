//
// GeneralOptionsPanel.cs
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
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
using System.IO;
using System.Collections;
using Gtk;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Gui.Dialogs;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Core.Gui.Dialogs;

namespace MonoDevelop.RubyBinding
{
	/// <summary>
	/// Panel for Ruby-specific options
	/// </summary>
	[System.ComponentModel.ToolboxItem(true)]
	public partial class GeneralOptionsPanel : Gtk.Bin
	{
		RubyProjectConfiguration config;
		Gtk.ListStore loadpathStore = new Gtk.ListStore (typeof(string));
		
		public GeneralOptionsPanel()
		{
			this.Build();
			
			Gtk.CellRendererText textRenderer = new Gtk.CellRendererText ();
			
			loadpathTreeView.Model = loadpathStore;
			loadpathTreeView.HeadersVisible = false;
			loadpathTreeView.AppendColumn ("Load Path", textRenderer, "text", 0);
		}
		
		public GeneralOptionsPanel (RubyProject project, RubyProjectConfiguration config): this ()
		{
			Load (project, config);
		}
		
		public bool Load (RubyProject project, RubyProjectConfiguration config)
		{
			int found = 0,
			    count = 0;
			if (null == config || null == project){ return false; }
			this.config = config;
			
			foreach (ProjectFile pf in project.Files) {
				projectFilesCB.AppendText (pf.Name);
				if (pf.Name.Equals (config.MainFile, StringComparison.OrdinalIgnoreCase)) { 
					found = count;
				}
				++count;
			}
			projectFilesCB.Active = found;
			foreach (object path in config.LoadPaths) {
				if (!string.IsNullOrEmpty ((string)path)) {
					loadpathStore.AppendValues (path);
				}
			}
			
			return true;
		}
		
		public bool Store ()
		{
			if (null == config) { return false; }
			
			TreeIter iter;
			string line;
			
			config.MainFile = projectFilesCB.ActiveText;
			config.LoadPaths.Clear ();
			for (loadpathStore.GetIterFirst (out iter);
			     loadpathStore.IterIsValid (iter);
			     loadpathStore.IterNext (ref iter)) {
				line = (string)loadpathStore.GetValue (iter, 0);
				if (!string.IsNullOrEmpty (line)){ config.LoadPaths.Add (line); }
			}
			return true;
		}

		protected virtual void loadpathAddEntryChanged (object sender, System.EventArgs e)
		{
			addLoadpathButton.Sensitive = Directory.Exists (loadpathAddEntry.Text);
		}

		protected virtual void loadpathAddButtonClicked (object sender, System.EventArgs e)
		{
			string path = loadpathAddEntry.Text;
			if (!string.IsNullOrEmpty (path)) {
				loadpathStore.AppendValues (path);
			}
		}

		protected virtual void browseButtonClicked (object sender, System.EventArgs e)
		{
			FileChooserDialog fcd = new FileChooserDialog (GettextCatalog.GetString ("Choose Load Path"), null, FileChooserAction.SelectFolder, 
			                                               Gtk.Stock.Cancel, Gtk.ResponseType.Cancel, Gtk.Stock.Open, Gtk.ResponseType.Ok);
			try {
				if (fcd.Run() == (int)ResponseType.Ok) {
					loadpathAddEntry.Text = fcd.Filename;
				}
			} finally {
				fcd.Destroy ();
			}
		}

		protected virtual void removeLoadpathButtonClicked (object sender, System.EventArgs e)
		{
			TreeIter iter;
			if (loadpathTreeView.Selection.GetSelected (out iter)) {
				loadpathStore.Remove (ref iter);
			}
			loadpathTreeViewCursorChanged (sender, e);
		}

		protected virtual void loadpathTreeViewCursorChanged (object sender, System.EventArgs e)
		{
			removeLoadpathButton.Sensitive = (null != loadpathTreeView.Selection && 
			                                  0 < loadpathTreeView.Selection.CountSelectedRows ());
		}
	}
    
	public class GeneralOptionsPanelBinding : MultiConfigItemOptionsPanel
	{
		private GeneralOptionsPanel panel;
		
		public override Gtk.Widget CreatePanelWidget ()
		{
			return panel = new GeneralOptionsPanel ();
		}
		
		public override void LoadConfigData ()
		{
			panel.Load ((RubyProject)ConfiguredProject, (RubyProjectConfiguration)CurrentConfiguration);
			panel.ShowAll ();
		}
		
		public override void ApplyChanges ()
		{
			panel.Store ();
		}
	}
}
