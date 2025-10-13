using Microsoft.VisualStudio.RpcContracts.Notifications;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using OpenAI.Chat;
using System;
using System.ComponentModel;
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
            You are an expert prompt engineer called Prompty.
            A user will use ""{{userPrompt}}"" as a prompt.

            Instructions:
            1. Score the prompt from 0% to 100% based on clarity, conciceness, and contextuality.
            2. If there is an issue with clarity, explain the parts of the prompt that are not clear.
            3. If there is an issue with conciceness, explain where the prompt is not concise.
            4. If there is an issue with contextuality, explain where the prompt lacks context.
            5. Praise the good qualities of the prompt.
            6. Explain the benefits of adjusting the input in terms of Token efficiency and generation speed.
            7. Identify the style of prompting that would work best for the use-case. Examples of styles of prompting are: Iterative Prompting, Few-shot prompting, Tree of Thoughts prompting, etc.

            Use less than 80 tokens.
            ";

        private string userPrompt;
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptEngineerWindowControl"/> class.
        /// </summary>
        public PromptEngineerWindowControl()
        {
            InitializeComponent();
            this.DataContext = this;
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
            string inputPrompt = MainPrompt.Replace("{{userPrompt}}", userPrompt);
            userPrompt = PromptInputText.Text;
            if (userPrompt == null)
                userPrompt = "This is a prompt";
            //Joinable task factory was chosen so that VS doesn't lag and can also responsively queue new prompts.
            AnalysisResults result = GeminiAPI.GetResponse(userPrompt);
            UpdateExistingElements(userPrompt, result);
        }

        public void UpdateExistingElements(string prompt, AnalysisResults analysisResults)
        {
            if (analysisResults == null)
            {
                ClarityText.Text = "An error has occured.";
                return;
            }

            PromptText.Text = prompt;
            ClarityText.Text = "Clarity: " + analysisResults.clarityExplanation;
            ConcicenessText.Text = "Conciceness: " + analysisResults.concicenessExplanation;
            ContextualityText.Text = "Contextuality: " + analysisResults.contextualityExplanation;
            PraiseText.Text = "Praise: " + analysisResults.praise;
            BenefitsText.Text = "Benefits: " + analysisResults.benefits;
        }
    }
}