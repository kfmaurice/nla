using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace None.NLA
{
   public class NLAOptions : DialogPage
   {
      [Category("Autocompletion")]
      [Description("Allow uppercase characters")]
      public bool AllowUpperCase { get; set; }

      [Category("Autocompletion")]
      [Description("Allow completion in string variables")]
      public bool AllowInStrings { get; set; }

      [Category("Word Composition")]
      [Description("Allow uppercase character as word separator")]
      public bool UpperCaseSeparator { get; set; }

      [Category("Word Composition")]
      [Description("Allow underscore character as word separator")]
      public bool UnderscoreSeparator { get; set; }

      [Category("Source File")]
      [Description("Words to be used for autocompletion")]
      public string DataLocation { get; set; }

      #region Custom storage
      public override void LoadSettingsFromStorage()
      {
         AllowUpperCase = Properties.Settings.Default.AllowUpperCase;
         AllowInStrings = Properties.Settings.Default.AllowInStrings;
         UpperCaseSeparator = Properties.Settings.Default.UpperCaseSeparator;
         UnderscoreSeparator = Properties.Settings.Default.UnderscoreSeparator;
         DataLocation = Properties.Settings.Default.DataLocation;
      }

      public override void SaveSettingsToStorage()
      {
         Properties.Settings.Default.AllowUpperCase = AllowUpperCase;
         Properties.Settings.Default.AllowInStrings = AllowInStrings;
         Properties.Settings.Default.UpperCaseSeparator = UpperCaseSeparator;
         Properties.Settings.Default.UnderscoreSeparator = UnderscoreSeparator;
         Properties.Settings.Default.DataLocation = DataLocation;
         Properties.Settings.Default.Save();
      }

      protected override System.Windows.Forms.IWin32Window Window
      {
         get
         {
            return new NLAOptionsView(this);
         }
      }
      #endregion
   }
}
