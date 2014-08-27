using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace None.NLA
{
   public class NLAConfiguration
   {
      /// <summary>
      /// Allow first letter upper case
      /// </summary>
      public static bool AllowUpperCase
      {
         get
         {
            return Properties.Settings.Default.AllowUpperCase;
         }
      }

      /// <summary>
      /// Support completion in string variables
      /// </summary>
      public static bool AllowInStrings
      { 
         get
         {
            return Properties.Settings.Default.AllowInStrings;
         }
      }
      
      /// <summary>
      /// Support camel case
      /// </summary>
      public static bool UpperCaseSeparator
      { 
         get
         {
            return Properties.Settings.Default.UpperCaseSeparator;
         }
      }

      /// <summary>
      /// Support snake case
      /// </summary>
      public static bool UnderscoreSeparator
      { 
         get
         {
            return Properties.Settings.Default.UnderscoreSeparator;
         }
      }

      /// <summary>
      /// Absolute path to the file with the words to use for the autocompletion
      /// </summary>
      public static string DataLocation
      { 
         get
         {
            return Properties.Settings.Default.DataLocation;
         }
      }
   }
}
