using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIPromptOptimizerExtension.Windows.PromptEvalWindow
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class PromptEngineerWindowControl : UserControl
    {
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
            //if (PromptInputText.Text == userPrompt)
                //return;

            userPrompt = PromptInputText.Text;
            if (userPrompt == null)
                userPrompt = "This is a prompt";

            PromptText.Text = "Generating...";
            //Joinable task factory was chosen so that VS doesn't lag and can also responsively queue new prompts.
            ReturnEvaluation(userPrompt);       //runs this operation in another thread.
        }

        /// <summary>
        /// Will call Gemini with the input prompt. Run in asynchronous conditions to keep the UI running!
        /// </summary>
        /// <param name="prompt">The prompt to evaluate.</param>
        public async Task ReturnEvaluation(string prompt)
        {
            AnalysisResults analysisResults = await GeminiAPI.GetResponseAsync(userPrompt);
            if (analysisResults == null)
            {
                ClarityText.Text = "An error has occured.";
                return;
            }

            //Dispatcher.Invoke(() => UpdateUI(prompt, analysisResults));
            UpdateUI(prompt, analysisResults);
        }

        public void UpdateUI(string prompt, AnalysisResults analysisResults)
        {
            PromptText.Text = prompt;
            ClarityText.Text = "Clarity: " + analysisResults.clarityExplanation;
            ConcicenessText.Text = "Conciceness: " + analysisResults.concicenessExplanation;
            ContextualityText.Text = "Contextuality: " + analysisResults.contextualityExplanation;
            PraiseText.Text = "Praise: " + analysisResults.praise;
            BenefitsText.Text = "Benefits: " + analysisResults.benefits;
        }
    }
}