using System;
using System.Linq;

namespace None.NLA
{
   public static class StringExtensions
   {
      public static string ToUpperFirstCharacter(this string item)
      {
         if (String.IsNullOrWhiteSpace(item))
         {
            return item;
         }

         var chars = item.ToArray();
         chars[0] = char.ToUpper(chars[0]);

         return new string(chars);
      }
   }
}
