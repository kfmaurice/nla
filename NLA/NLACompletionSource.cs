using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace None.NLA
{
   internal class NLACompletionSource : ICompletionSource
   {
      private NLACompletionSourceProvider sourceProvider;
      private ITextBuffer textBuffer;
      private List<Completion> compList;
      private List<Completion> compListUppercase;
      private bool isDisposed;

      public NLACompletionSource(NLACompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
      {
         this.sourceProvider = sourceProvider;
         this.textBuffer = textBuffer;
      }

      #region ICompletionSource interface implementation
      void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
      {
         GetWords(NLAConfiguration.DataLocation);

         bool isUpperCase;
         var span = FindTokenSpanAtPosition(session.GetTriggerPoint(textBuffer), session, out isUpperCase);
         if (span != null)
         {
            completionSets.Add(new CompletionSet(
              "Tokens",    //the non-localized title of the tab
              "Tokens",    //the display title of the tab
              span,
              isUpperCase ? compListUppercase : compList,
              null));
         }
      } 
      #endregion

      #region IDisposable interface implementation
      public void Dispose()
      {
         if (!isDisposed)
         {
            GC.SuppressFinalize(this);
            isDisposed = true;
         }
      }
      #endregion

      #region Helpers
      private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session, out bool isUpperCase)
      {
         SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
         ITextStructureNavigator navigator = sourceProvider.NavigatorService.GetTextStructureNavigator(textBuffer);
         TextExtent extent = navigator.GetExtentOfWord(currentPoint);

         string text = extent.Span.GetText();
         string strText = navigator.GetExtentOfWord((session.TextView.Caret.Position.BufferPosition) - text.Length - 1).Span.GetText();
         int uppercasePosition = 0;
         int underscorePosition = 0;

         if (NLAConfiguration.UnderscoreSeparator)
         {
            uppercasePosition = text.LastIndexOf("_");
            if (uppercasePosition > 0)
            {
               uppercasePosition = uppercasePosition + 1; // get passed the underscore character
               underscorePosition = uppercasePosition;
            }
         }
         else if (NLAConfiguration.UpperCaseSeparator)
         {
            uppercasePosition = text.LastIndexOf(text.LastOrDefault(c => char.IsUpper(c)));
            underscorePosition = uppercasePosition - 1;
         }

         isUpperCase = char.IsUpper(text[0]);
         if (!NLAConfiguration.AllowInStrings)
         {
            var line = currentPoint.GetContainingLine();
            var doubleQuote = line.GetText().IndexOf("\"");
            var singleQuote = line.GetText().IndexOf("'");

            if ((doubleQuote >= 0 && line.Start.Position + doubleQuote <= currentPoint.Position) || (singleQuote >= 0 && line.Start.Position + singleQuote <= currentPoint.Position))
            {
               return null;
            }
         }

         if (uppercasePosition > 0)
         {
            var span = currentPoint.Snapshot.CreateTrackingSpan(extent.Span.Start.Position + uppercasePosition, text.Length - uppercasePosition, SpanTrackingMode.EdgeInclusive);
            var underscoreSpan = currentPoint.Snapshot.CreateTrackingSpan(extent.Span.Start.Position + underscorePosition, text.Length - underscorePosition, SpanTrackingMode.EdgeInclusive);

            if (!NLAConfiguration.UnderscoreSeparator && underscoreSpan.GetText(currentPoint.Snapshot).StartsWith("_")) // avoid this "word_Composition" when camel case is not active
            {
               return null;
            }

            text = span.GetText(currentPoint.Snapshot);
            if (!String.IsNullOrEmpty(text))
            {
               isUpperCase = char.IsUpper(text[0]);
            }
            return span;
         }

         return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
      }

      private void GetWords(string file)
      {
         if ((compList == null || compList.Count == 0) && File.Exists(file))
         {
            List<string> words = new List<string>();
            compList = new List<Completion>();
            compListUppercase = new List<Completion>();

            using (StreamReader sr = new StreamReader(file))
            {
               string line;
               while ((line = sr.ReadLine()) != null)
               {
                  words.Add(line);
               }
            }

            words.OrderBy(w => w);
            foreach (var word in words)
            {
               var upper = word.ToUpperFirstCharacter();
               compList.Add(new Completion(word, word, word, null, null));
               compListUppercase.Add(new Completion(upper, upper, upper, null, null));
            }
         }
      } 
      #endregion
   }

   [Export(typeof(ICompletionSourceProvider))]
   [ContentType("code")]
   [Name("token completion source")]
   internal class NLACompletionSourceProvider : ICompletionSourceProvider
   {
      [Import]
      internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

      #region ICompletionSourceProvider interface implementation
      public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
      {
         return new NLACompletionSource(this, textBuffer);
      } 
      #endregion
   }
}
