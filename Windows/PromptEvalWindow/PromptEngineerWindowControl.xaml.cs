using OpenAI.Chat;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace AIPromptOptimizerExtension.Windows.PromptEvalWindow
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class PromptEngineerWindowControl : UserControl
    {
        //Verbatim text.
        private const string MainPrompt = @"
            You are Prompty, an expert prompt engineer.
            A user will use {{userPrompt}} as a prompt.

            Instructions:
            1. Score the prompt from 0% to 100% based on clarity, conciceness, and contextuality.
            2. If there is an issue with clarity, explain the parts of the prompt that are not clear.
            3. If there is an issue with conciceness, explain where the prompt is not concise.
            4. If there is an issue with contextuality, explain where the prompt lacks context.
            5. Praise the good qualities of the prompt.
            6. Explain the benefits of adjusting the input in terms of Token efficiency and generation speed.
            ";

        private string userPrompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptEngineerWindowControl"/> class.
        /// </summary>
        public PromptEngineerWindowControl() => InitializeComponent();

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void OnSubmitPrompt(object sender, RoutedEventArgs e)
        {
            ChatClient client = new ChatClient(
                model: "gpt-5-mini",
                apiKey: ""
            );

            string inputPrompt = MainPrompt.Replace("{{userPrompt}}", "userPrompt");
            ChatCompletion response = client.CompleteChat("This is a prompt");
            string result = response.Content[0].Text;

            Console.WriteLine(result);
        }

        private void Key_Input(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            userPrompt = e.Text;
        }

        private void Prompt_Input(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }
    }
}