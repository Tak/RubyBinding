//
// RubyTextEditorExtension.cs
//
// Authors:
//  Levi Bard <taktaktaktaktaktaktaktaktaktak@gmail.com> 
//
// Copyright (C) 2009 Levi Bard
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//



using System;
using System.IO;

using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Gui.Completion;

namespace MonoDevelop.RubyBinding
{
	public class RubyTextEditorExtension: CompletionTextEditorExtension
	{
		public override bool ExtendsEditor (Document doc, IEditableTextBuffer editor)
		{
			return (Path.GetExtension (doc.FileName).Equals (RubyLanguageBinding.RubyExtension, StringComparison.OrdinalIgnoreCase));
		}
		
		public override ICompletionDataList HandleCodeCompletion (ICodeCompletionContext completionContext, char completionChar)
		{
			CompletionDataList cdl = new CompletionDataList ();
			if ('.' == completionChar) {
				string contents = Editor.Text,
				       symbol = RubyCompletion.GetSymbol (contents, completionContext.TriggerOffset-1);
				// Console.WriteLine ("RubyBinding: Completing {0}", symbol);
				if (!string.IsNullOrEmpty (symbol)) {
					string basepath = (null == Document.Project)? 
						Document.FileName.FullPath.ParentDirectory: 
						Document.Project.BaseDirectory.FullPath;
					ICompletionData[] completions = RubyCompletion.Complete (basepath, contents, symbol, completionContext.TriggerLine-1);
					if (null != completions) {
						// Console.WriteLine ("RubyBinding: Got {0} completions", completions.Length);
						cdl.AddRange (completions);
					}
				}
			}
			
			// Zero-length list causes segfault
			return (0 < cdl.Count)? cdl: null;
		}// HandleCodeCompletion
		
		public override ICompletionDataList CodeCompletionCommand (ICodeCompletionContext completionContext)
		{
			if (RubyLanguageBinding.IsRubyFile (Document.FileName)) {
				int pos = completionContext.TriggerOffset;
				return HandleCodeCompletion(completionContext, Editor.GetText (pos - 1, pos)[0]);
			}
			return null;
		}// CodeCompletionCommand
		
		public override  IParameterDataProvider HandleParameterCompletion (ICodeCompletionContext completionContext, char completionChar)
		{
			ParameterDataProvider pdp = null;
			if (char.IsWhiteSpace (completionChar) || '(' == completionChar) {
				pdp = new ParameterDataProvider (Document, completionContext);
			}
			return (null != pdp && pdp.Valid)? pdp: null;
		}
	}// RubyTextEditorExtension
}
