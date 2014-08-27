using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace None.NLA
{
   public partial class NLAOptionsView : UserControl
   {
      public NLAOptions Options { get; set; }

      public NLAOptionsView(NLAOptions options)
      {
         InitializeComponent();

         Options = options;
      }

      #region Window events
      private void NLAOptionsView_Load(object sender, EventArgs e)
      {
         checkBoxAllowUpperCase.Checked = Options.AllowUpperCase;
         checkBoxAllowInStrings.Checked = Options.AllowInStrings;
         radioButtonUpperCaseSeparator.Checked = Options.UpperCaseSeparator;
         radioButtonUnderscoreSeparator.Checked = Options.UnderscoreSeparator;
         radioButtonNoWordComposition.Checked = !Options.UpperCaseSeparator && !Options.UnderscoreSeparator;
         textBoxDataLocation.Text = Options.DataLocation;
      } 
      #endregion

      #region Control events
      private void checkBoxAllowUppercase_CheckedChanged(object sender, EventArgs e)
      {
         Options.AllowUpperCase = checkBoxAllowUpperCase.Checked;
         panelAllowUppercase.Visible = Options.AllowUpperCase && !Options.AllowInStrings;
         panelAllowInStrings.Visible = !Options.AllowUpperCase && Options.AllowInStrings;
         panelUppercaseInStrings.Visible = Options.AllowUpperCase && Options.AllowInStrings;
      }

      private void checkBoxAllowInStrings_CheckedChanged(object sender, EventArgs e)
      {
         Options.AllowInStrings = checkBoxAllowInStrings.Checked;
         panelAllowUppercase.Visible = Options.AllowUpperCase && !Options.AllowInStrings;
         panelAllowInStrings.Visible = !Options.AllowUpperCase && Options.AllowInStrings;
         panelUppercaseInStrings.Visible = Options.AllowUpperCase && Options.AllowInStrings;
      }

      private void buttonBrowse_Click(object sender, EventArgs e)
      {
         openFileDialog.ShowDialog();
         textBoxDataLocation.Text = openFileDialog.FileName;
         Options.DataLocation = textBoxDataLocation.Text;
      }
      #endregion

      private void radioButtonUpperCaseSeparator_CheckedChanged(object sender, EventArgs e)
      {
         Options.UpperCaseSeparator = radioButtonUpperCaseSeparator.Checked;
         Options.UnderscoreSeparator = radioButtonUnderscoreSeparator.Checked;
         panelUpperCaseSeparator.Visible = Options.UpperCaseSeparator && !Options.UnderscoreSeparator;
         panelUnderscoreSeparator.Visible = !Options.UpperCaseSeparator && Options.UnderscoreSeparator;

         checkBoxAllowUpperCase.Enabled = !radioButtonUpperCaseSeparator.Checked;
         checkBoxAllowUpperCase.Checked = radioButtonUpperCaseSeparator.Checked || Options.AllowUpperCase;
      }

      private void radioButtonUnderscoreSeparator_CheckedChanged(object sender, EventArgs e)
      {
         Options.UpperCaseSeparator = radioButtonUpperCaseSeparator.Checked;
         Options.UnderscoreSeparator = radioButtonUnderscoreSeparator.Checked;
         panelUpperCaseSeparator.Visible = Options.UpperCaseSeparator && !Options.UnderscoreSeparator;
         panelUnderscoreSeparator.Visible = !Options.UpperCaseSeparator && Options.UnderscoreSeparator;
      }

      private void radioButtonNoWordComposition_CheckedChanged(object sender, EventArgs e)
      {
         Options.UpperCaseSeparator = radioButtonUpperCaseSeparator.Checked;
         Options.UnderscoreSeparator = radioButtonUnderscoreSeparator.Checked;
         panelUpperCaseSeparator.Visible = Options.UpperCaseSeparator && !Options.UnderscoreSeparator;
         panelUnderscoreSeparator.Visible = !Options.UpperCaseSeparator && Options.UnderscoreSeparator;
      }
   }
}
