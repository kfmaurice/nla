using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;


namespace None.NLA
{
   [Export(typeof(IVsTextViewCreationListener))]
   [Name("token completion handler")]
   [ContentType("code")]
   [TextViewRole(PredefinedTextViewRoles.Editable)]
   internal class NLACompletionHandlerProvider : IVsTextViewCreationListener
   {
      [Import]
      internal IVsEditorAdaptersFactoryService AdapterService = null;
      [Import]
      internal ICompletionBroker CompletionBroker { get; set; }
      [Import]
      internal SVsServiceProvider ServiceProvider { get; set; }

      #region IVsTextViewCreationListener interface implementation
      public void VsTextViewCreated(IVsTextView textViewAdapter)
      {
         ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
         if (textView == null)
            return;

         Func<TestCompletionCommandHandler> createCommandHandler = delegate() { return new TestCompletionCommandHandler(textViewAdapter, textView, this); };
         textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
      } 
      #endregion
   }

   internal class TestCompletionCommandHandler : IOleCommandTarget
   {
      private IOleCommandTarget nextCommandHandler;
      private ITextView textView;
      private NLACompletionHandlerProvider provider;
      private ICompletionSession session;

      internal TestCompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, NLACompletionHandlerProvider provider)
      {
         this.textView = textView;
         this.provider = provider;

         //add the command to the command chain
         textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
      }

      #region IOleCommandTarget interface implementation
      public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
      {
         return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
      }

      public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
      {
         if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider))
         {
            return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
         }

         //make a copy of this so we can look at it after forwarding some commands
         uint commandID = nCmdID;
         char typedChar = char.MinValue;

         //make sure the input is a char before getting it
         if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
         {
            typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
         }

         //check for a commit character
         if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar) || typedChar == '='))
         {
            //check for a a selection
            if (session != null && !session.IsDismissed)
            {
               //if the selection is fully selected, commit the current session
               if (session.SelectedCompletionSet.SelectionStatus.IsSelected)
               {
                  session.Commit();
                  //also, don't add the character to the buffer
                  if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
                  {
                     return VSConstants.S_OK;
                  }
               }
               else
               {
                  //if there is no selection, dismiss the session
                  session.Dismiss();
               }
            }
         }

         //pass along the command so the char is added to the buffer
         int retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
         bool handled = false;

         if ((!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar)) || commandID == (int)VSConstants.VSStd2KCmdID.COMPLETEWORD)
         {
            if (session == null || session.IsDismissed) // If there is no active session, bring up completion
            {
               this.TriggerCompletion();
               if (session != null)
               {
                  session.Filter();
               }
            }
            else    //the completion session is already active, so just filter
            {
               session.Filter();
            }
            handled = true;
         }
         else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
         {
            if (session != null && !session.IsDismissed)
            {
               session.Filter();
               if (!session.SelectedCompletionSet.SelectionStatus.IsSelected)
               {
                  session.Dismiss();
               }
            }
            handled = true;
         }

         if (handled)
         {
            return VSConstants.S_OK;
         }
         return retVal;
      } 
      #endregion

      private bool TriggerCompletion()
      {
         //the caret must be in a non-projection location 
         SnapshotPoint? caretPoint =
         textView.Caret.Position.Point.GetPoint(
         textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
         if (!caretPoint.HasValue)
         {
            return false;
         }

         if (provider.CompletionBroker.GetSessions(textView).Count == 0)
         {
            session = provider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);

            //subscribe to the Dismissed event on the session 
            session.Dismissed += this.OnSessionDismissed;
            session.Start(); 
         }

         return true;
      }

      private void OnSessionDismissed(object sender, EventArgs e)
      {
         session.Dismissed -= this.OnSessionDismissed;
         session = null;
      }
   }
}