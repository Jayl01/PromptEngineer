using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace AIPromptOptimizerExtension
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class PromptEngineerWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptEngineerWindowControl"/> class.
        /// </summary>
        public PromptEngineerWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void OnSubmitPrompt(object sender, RoutedEventArgs e)
        {
            /*MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "ToolWindow1");*/


        }

        private void TextBlock_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }
    }
}