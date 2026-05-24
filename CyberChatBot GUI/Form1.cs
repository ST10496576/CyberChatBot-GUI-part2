using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberChatBot_GUI
{
    public partial class Form1 : Form
    {
        string currentTopic = "";
        delegate string ChatbotResponse(string input);
        Random random = new Random();
        // Cybersecurity Awareness Chatbot GUI
        // This application provides users with cybersecurity tips
        // based on keyword recognition, sentiment detection, and conversational flow.
        // It is designed using Windows Forms and demonstrates event-driven programming.
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        // Displays welcome message and ASCII-style banner when the application starts
        // Introduces the user to available cybersecurity topics
        private void Form1_Load(object sender, EventArgs e)
       
        {
            richTextBox1.AppendText(
                " ===========================\n" +
                "CYBER SECURITY CHATBOT\n" +
                " ===========================\n\n" +
                "Bot: Hi there user! Welcome to Cybersecurity Chatbot!\n" +
                "You can ask me about:\n" +
                "1. Password safety\n" +
                "2. Phishing scams\n" +
                "3. Privacy tips\n\n Please communicate in words or sentences!\n\n "+


                "How to interact with me:\n" +
                "Type a topic like 'password', 'phishing', or 'privacy`.\n" +
                "You can also ask for more tips by typing 'tell me more' or 'another tip'.\n" +
                "Type 'help' if you need guidance on what to ask.\n" +
                "To end the conversation, type 'exit' or 'quit'\n\n"+

                "Im here to help you stay safe online, so feel free to ask me anything about cybersecurity!"
            );
        }

        // Handles user input when the Send button is clicked
        // Displays user message, processes response, and updates chat window
        private void button1_Click_1(object sender, EventArgs e)
        {
            string input = textBox1.Text.ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                richTextBox1.AppendText("Bot: Please type something.\n\n");
                // makes sure appended text is scrolled into view
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();
                return;
            }

            richTextBox1.AppendText("You: " + input + "\n");

            // handles exit/quit commands immediately
            if (input == "exit" || input == "quit")
            {
                richTextBox1.AppendText("Bot: Goodbye! Stay safe online!\n\n");
                // scroll to the latest message
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();
                Application.Exit();
                return;
            }

            // Included the class-level delegate as requested
            ChatbotResponse responseDelegate = GetResponse;
            string response = responseDelegate(input);

            richTextBox1.AppendText("Bot: " + response + "\n\n");
            // scroll to the latest message
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            richTextBox1.Refresh();

            textBox1.Clear();
        }

        // Processes user input and returns chatbot responses
        // Includes keyword recognition, sentiment detection,
        // conversation flow handling, and default error response

        List<string> phishingResponses = new List<string>()
{
    "Be cautious of emails asking for personal details.",
    "Always verify the sender before clicking links.",
    "Scammers often create urgency to trick users."
};

        // duplicate Random/return removed (phishingResponses and rand are declared above)

        private string GetResponse(string input)
        
        {
            string userMood = "";

            // ---------------- SENTIMENT DETECTION (NO RETURNS HERE) ----------------
            if (input.Contains("worried"))
                userMood = "worried";
            else if (input.Contains("frustrated"))
                userMood = "frustrated";
            else if (input.Contains("curious"))
                userMood = "curious";
            // ---------------- MOOD-BASED RESPONSES  --------------------
            if (input.Contains("worried") && input.Contains("password") == false &&

                input.Contains("phishing") == false && input.Contains("privacy") == false)

            {

                return "It's okay to feel worried. Let’s take things step by step and keep you safe online.\n\n You can continue by asking about password safety, phishing scams, privacy tips or type 'help' for more options.";

            }

            if (input.Contains("frustrated") && input.Contains("password") == false &&

                input.Contains("phishing") == false && input.Contains("privacy") == false)

            {

                return "I understand this can feel frustrating. I’ll guide you step by step so it becomes easier.";

            }

            if (input.Contains("curious") && input.Contains("password") == false &&

                input.Contains("phishing") == false && input.Contains("privacy") == false)

            {

                return "Great curiosity! Let’s explore cybersecurity together step by step.You can continue by asking about password safety, phishing scams, privacy tips or type 'help' for more options.";

            }
            // ---------------- HELP ----------------
            if (input.Contains("help"))
            {
                return "I’m here to help you stay safe online. " +
                       "You can ask about password safety, phishing scams, or privacy tips. " +
                       "Try typing 'password', 'phishing', or 'privacy'.";
            }

            // ---------------- FOLLOW-UP FLOW ----------------
            if (input.Contains("tell me more") || input.Contains("another tip"))
            {
                if (currentTopic == "password")
                {
                    return "More password tips: Use 8–12+ characters with uppercase, lowercase, numbers and symbols. " +
                           "Avoid personal details and never reuse passwords.";
                }

                if (currentTopic == "phishing")
                {
                    return "More phishing tips: Always check sender emails carefully. " +
                           "If something feels urgent or suspicious, do not click links.";
                }

                if (currentTopic == "privacy")
                {
                    return "More privacy tips: Limit app permissions and control what you share online.";
                }

                return "Please choose a topic: password, phishing, or privacy.";
            }

            // ---------------- PASSWORD ----------------
            if (input.Contains("password"))
            {
                currentTopic = "password";

                string response =
                    "Strong passwords protect your accounts from hackers. " +
                    "Use 8–12 characters with uppercase, lowercase, numbers, and symbols. " +
                    "Avoid using your name or birthdate. " +
                    "Never reuse passwords across accounts.";

                if (userMood == "worried")
                    response = "I understand you're worried. Let’s take it step by step. " + response;

                if (userMood == "frustrated")
                    response = "I know this can feel confusing. Let’s simplify it. " + response;

                return response;
            }

            // ---------------- PHISHING (WITH RANDOM RESPONSE) ----------------
            if (input.Contains("phishing") || input.Contains("scam"))
            {
                currentTopic = "phishing";

                string baseResponse =
                    "Phishing scams are fake messages designed to trick you into giving personal information. " +
                    "They often look like emails or SMS from trusted companies. " +
                    "Always check the sender carefully and avoid suspicious links. " +
                    "If something feels urgent, it is usually a scam.";

                string randomTip = phishingResponses[random.Next(phishingResponses.Count)];

                string response = baseResponse + " " + randomTip;

                if (userMood == "frustrated")
                    response = "I understand this can be frustrating. Let’s break it down. " + response;

                if (userMood == "worried")
                    response = "It's okay to feel worried. Let’s go step by step. " + response;

                return response;
            }

            // ---------------- PRIVACY ----------------
            if (input.Contains("privacy"))
            {
                currentTopic = "privacy";

                string response =
                    "Privacy means protecting your personal information online. " +
                    "Always review app permissions and limit what you share. " +
                    "Turn off location access when not needed.";

                if (userMood == "curious")
                    response = "Great curiosity! Let’s explore this together. " + response;

                return response;
            }

            // ---------------- DEFAULT RESPONSE ----------------
            return "I didn’t understand that. Please ask about password safety, phishing scams, or privacy tips.";
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}

