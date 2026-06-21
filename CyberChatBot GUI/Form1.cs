using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace CyberChatBot_GUI
{
    class Question
    {
        public string Text { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
    }

    class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool Completed { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }


    public partial class Form1 : Form

    {   //Stores the current chatbot topic to support follow up conversations.
        string currentTopic = "";
        delegate string ChatbotResponse(string input);
        //Generates random phishing awareness tips for users
        Random random = new Random();
        List<string> activityLog = new List<string>();
        // Quiz state
        List<Question> quizQuestions = new List<Question>();
        int currentQuestionIndex = 0;
        int score = 0;
        bool quizActive = false;
        // Tasks
        List<TaskItem> tasks = new List<TaskItem>();
        string taskFile = "tasks.txt";
        // MySQL connection string
        private readonly string connectionString = "server=localhost;user=root;password=MySQL@2026;database=cyberchatbot;";
        bool waitingForTaskTitle = false;
        bool waitingForTaskDescription = false;
        bool waitingForReminder = false;
        bool tempTaskTwoFactor = false;
        string tempTaskTitle = "";
        string tempTaskDescription = "";
        private void LoadQuiz()
        {
            quizQuestions = new List<Question>()
    {
        new Question
        {
            Text = "What is phishing?",
            Options = new string[]
            {
                "A safe website",
                "A scam to steal information",
                "A password manager",
                "A browser update"
            },
            CorrectAnswer = 1
        },

        new Question
        {
            Text = "What should you do with a suspicious email?",
            Options = new string[]
            {
                "Open it",
                "Click links",
                "Report it",
                "Reply with info"
            },
            CorrectAnswer = 2
        },

        new Question
        {
            Text = "A strong password should:",
            Options = new string[]
            {
                "Be short",
                "Be reused everywhere",
                "Include letters, numbers, symbols",
                "Be your name"
            },
            CorrectAnswer = 2
        }
    };
        }

       
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

        { SoundPlayer player = new SoundPlayer(Application.StartupPath + "\\part2.wav");
        player.Play();
            LoadTasks();
            // optional: test the DB connection on startup (comment out if not needed)
            //string dbTest = TestMySqlConnection();
            //LogActivity("DB Test: " + dbTest);
            {
                richTextBox1.AppendText(
                    " ===========================\n" +
                    "CYBER SECURITY CHATBOT\n" +
                    " ===========================\n\n" +
                    "Bot: Hi there user! Welcome to Cybersecurity Chatbot!\n" +
                    "You can ask me about:\n" +
                    "1. Password safety\n" +
                    "2. Phishing scams\n" +
                    "3. Privacy tips\n\n Please communicate in words or sentences!\n\n " +


                    "How to interact with me:\n" +
                    "Type a topic like 'password', 'phishing', or 'privacy`.\n" +
                    "Example questions you can ask:\n" +// Displays additional guidance !!
                    "- How do I create a strong password?\n" +
                    "- What is phishing?\n" +
                    "- How can I protect my privacy online?\n\n" +
                    "You can also ask for more tips by typing 'tell me more' or 'another tip'.\n" +
                    "Type 'help' if you need guidance on what to ask.\n" +
                    "To end the conversation, type 'exit' or 'quit'\n\n" +

                    "Im here to help you stay safe online, so feel free to ask me anything about cybersecurity!"
                );
            }
        }

        // Handles user input when the Send button is clicked
        // Displays user message, processes response, and updates chat window
        private void button1_Click_1(object sender, EventArgs e)
        {
            string rawInput = textBox1.Text;

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                richTextBox1.AppendText("Bot: Please type something.\n\n");
                // makes sure appended text is scrolled into view
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();
                return;
            }

            richTextBox1.AppendText("You: " + rawInput + "\n");

            // handles exit/quit commands immediately
            string inputNormForExit = rawInput.Trim().ToLowerInvariant();
            if (inputNormForExit == "exit" || inputNormForExit == "quit")
            {
                LogActivity("User requested to exit the application");
                richTextBox1.AppendText("Bot: Goodbye! Thank you for using Cyber Security Chatbot. Stay safe online!\n\n");

                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();

                Application.Exit();
                return;
            }

            // Included the class-level delegate as requested
            ChatbotResponse responseDelegate = GetResponse;
            string response = responseDelegate(rawInput);

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
    "Scammers often create urgency to trick users.",
    "Never download attachments from unknown senders.",
    "Check for spelling mistakes in suspicious emails.",
    "Banks usually do not ask for passwords through email.",
    
};
private void LogActivity(string action)
        {
            activityLog.Add(DateTime.Now.ToString("g") +" - " + action);
        }

        private void SaveTasks()
        {
            List<string> lines = new List<string>();

            foreach (var t in tasks)
            {
                lines.Add(
                    t.Title + "|" +
                    t.Description + "|" +
                    t.Reminder + "|" +
                    t.Completed + "|" +
                    t.TwoFactorEnabled
                );
            }

            System.IO.File.WriteAllLines(taskFile, lines);
        }

        private void LoadTasks()
        {
            if (!System.IO.File.Exists(taskFile))
                return;

            string[] lines = System.IO.File.ReadAllLines(taskFile);

            tasks.Clear();

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');

                if (parts.Length == 5)
                {
                    tasks.Add(new TaskItem
                    {
                        Title = parts[0],
                        Description = parts[1],
                        Reminder = parts[2],
                        Completed = bool.Parse(parts[3]),
                        TwoFactorEnabled = bool.Parse(parts[4])
                    });
                }
            }
        }
        private string ShowQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                quizActive = false;

                return "Quiz finished!\nYour score: " + score + "/" + quizQuestions.Count;
            }
            Question q = quizQuestions[currentQuestionIndex];

            string text =
                q.Text + "\n\n" +
                "A) " + q.Options[0] + "\n" +
                "B) " + q.Options[1] + "\n" +
                "C) " + q.Options[2] + "\n" +
                "D) " + q.Options[3] + "\n\n" +
                "Type A, B, C or D";

            return text;
        }

        // duplicate Random/return removed (phishingResponses and rand are declared above)

        private string GetResponse(string input)
        {
            // preserve raw for titles/descriptions
            string raw = input ?? string.Empty;
            string norm = raw.Trim().ToLowerInvariant();
            // --- Task conversational flow (title, description, 2FA, reminder) ---
            // start add-task
            if (norm == "add task" || norm.StartsWith("add task ") || norm == "add")
            {
                tempTaskTwoFactor = false;
                waitingForTaskTitle = true;
                return "Please enter the task title. (Type 'enable 2fa' to enable two-factor for this task)";
            }

            if (waitingForTaskTitle)
            {
                if (norm == "enable 2fa")
                {
                    tempTaskTwoFactor = true;
                    return "Two-factor authentication will be enabled for this task. Now enter the task title.";
                }

                tempTaskTitle = raw.Trim();
                waitingForTaskTitle = false;
                waitingForTaskDescription = true;
                return "Please enter the task description. (Type 'enable 2fa' to enable two-factor for this task)";
            }

            if (waitingForTaskDescription)
            {
                if (norm == "enable 2fa")
                {
                    tempTaskTwoFactor = true;
                    return "Two-factor authentication will be enabled. Now enter the task description.";
                }

                tempTaskDescription = raw.Trim();
                waitingForTaskDescription = false;
                waitingForReminder = true;
                return "Would you like a reminder? Type number of days (e.g. '3') or 'none'.";
            }

            if (waitingForReminder)
            {
                string reminder;
                if (norm == "none")
                {
                    reminder = "none";
                }
                else
                {
                    int days;
                    if (int.TryParse(norm, out days) && days > 0)
                        reminder = days + " days";
                    else
                        return "Please enter a valid number of days for the reminder, or 'none'.";
                }

                var task = new TaskItem
                {
                    Title = tempTaskTitle,
                    Description = tempTaskDescription,
                    Reminder = reminder,
                    Completed = false,
                    TwoFactorEnabled = tempTaskTwoFactor
                };

                // persist task to MySQL
                try
                {
                    string query = "INSERT INTO tasks (title, description, reminder, completed, twoFactorEnabled) VALUES (@title, @desc, @rem, @done, @twofa)";

                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", task.Title);
                            cmd.Parameters.AddWithValue("@desc", task.Description);
                            cmd.Parameters.AddWithValue("@rem", task.Reminder);
                            // convert booleans to ints for MySQL TINYINT(1) columns
                            cmd.Parameters.AddWithValue("@done", task.Completed ? 1 : 0);
                            cmd.Parameters.AddWithValue("@twofa", task.TwoFactorEnabled ? 1 : 0);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    LogActivity("Task saved to MySQL: " + task.Title);
                }
                catch (Exception ex)
                {
                    // if DB insert fails, fall back to local storage and log the error
                    LogActivity("MySQL insert failed: " + ex.Message);
                }

                // keep task in-memory for immediate use and persist to file as backup
                tasks.Add(task);

                // persist tasks (backup)
                SaveTasks();

                LogActivity("Task added: " + task.Title);

                // RESET STATE (IMPORTANT)
                waitingForTaskTitle = false;
                waitingForTaskDescription = false;
                waitingForReminder = false;
                tempTaskTitle = string.Empty;
                tempTaskDescription = string.Empty;
                tempTaskTwoFactor = false;

                return "Task added successfully! Title: " + task.Title + ", 2FA: " + (task.TwoFactorEnabled ? "enabled" : "disabled") + ", Reminder: " + task.Reminder + ".";
            }

            // view / complete / delete tasks (use normalized input 'norm')
            if (norm.Contains("view tasks") || norm == "tasks")
            {
                if (tasks.Count == 0)
                    return "No tasks available.";

                string output = "TASK LIST:\n\n";

                for (int i = 0; i < tasks.Count; i++)
                {
                    var t = tasks[i];

                    output +=
                        (i + 1) + ". " + t.Title + "\n" +
                        "   Description: " + t.Description + "\n" +
                        "   2FA: " + (t.TwoFactorEnabled ? "Enabled" : "Disabled") + "\n" +
                        "   Reminder: " + t.Reminder + "\n" +
                        "   Status: " + (t.Completed ? "Completed" : "Pending") + "\n\n";
                }

                return output;
            }

            if (norm.Contains("complete task"))
            {
                int index;

                if (int.TryParse(norm.Replace("complete task", "").Trim(), out index))
                {
                    index--; // user starts from 1, list starts from 0

                    if (index >= 0 && index < tasks.Count)
                    {
                        tasks[index].Completed = true;
                        SaveTasks();
                        LogActivity("Task completed: " + tasks[index].Title);

                        return "Task marked as completed: " + tasks[index].Title;
                    }
                }

                return "Please type: complete task 1, complete task 2, etc.";
            }

            if (norm.Contains("delete task"))
            {
                int index;

                if (int.TryParse(norm.Replace("delete task", "").Trim(), out index))
                {
                    index--;

                    if (index >= 0 && index < tasks.Count)
                    {
                        string removed = tasks[index].Title;
                        tasks.RemoveAt(index);

                        SaveTasks();
                        LogActivity("Task deleted: " + removed);

                        return "Task deleted: " + removed;
                    }
                }

                return "Please type: delete task 1, delete task 2, etc.";
            }

            // Quiz answer handling
            if (quizActive)
            {
                int userAnswer = -1;

                if (input.Length > 0)
                {
                    char c = input[0];
                    if (c == 'a') userAnswer = 0;
                    else if (c == 'b') userAnswer = 1;
                    else if (c == 'c') userAnswer = 2;
                    else if (c == 'd') userAnswer = 3;
                }

                if (userAnswer != -1)
                {
                    string resultMsg;
                    if (userAnswer == quizQuestions[currentQuestionIndex].CorrectAnswer)
                    {
                        score++;
                        LogActivity("Quiz answer correct");
                        resultMsg = "Correct! Your current score: " + score + "/" + (currentQuestionIndex + 1);
                    }
                    else
                    {
                        LogActivity("Quiz answer incorrect");
                        resultMsg = "Incorrect. The correct answer was: " +
                                    (char)('A' + quizQuestions[currentQuestionIndex].CorrectAnswer) + ") " +
                                    quizQuestions[currentQuestionIndex].Options[quizQuestions[currentQuestionIndex].CorrectAnswer] +
                                    "\nYour current score: " + score + "/" + (currentQuestionIndex + 1);
                    }

                    // advance to next question
                    currentQuestionIndex++;

                    // if quiz finished
                    if (currentQuestionIndex >= quizQuestions.Count)
                    {
                        quizActive = false;
                        return resultMsg + "\n\nQuiz finished!\nYour score: " + score + "/" + quizQuestions.Count;
                    }

                    // show next question
                    return resultMsg + "\n\n" + ShowQuestion();
                }
                // if input wasn't an answer, prompt user to answer
                return "Please answer with A, B, C or D.";
            }

            if (input.Contains("quiz") || input.Contains("start quiz"))
            {
                LoadQuiz();
                quizActive = true;
                currentQuestionIndex = 0;
                score = 0;

                return ShowQuestion();
            }
            // Handles user mood detection for support
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

            if (input.Contains("show activity log") ||
    input.Contains("what have you done for me") ||
    input.Contains("activity log"))
            {
                if (activityLog.Count == 0)
                    return "No activity recorded yet.";

                string logText = "Recent Activity:\n\n";

                foreach (string log in activityLog.Skip(Math.Max(0, activityLog.Count - 10)))
                {
                    logText += "- " + log + "\n";
                }

                return logText;
            }


            // ---------------- HELP ----------------
            if (input.Contains("help"))
            {
                LogActivity("User requested help");
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
                LogActivity("User requested about password information");

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
                LogActivity("User requested about phishing information");
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
                LogActivity("User requested about privacy information");
                string response =
                    "Privacy means protecting your personal information online. " +
                    "Always review app permissions and limit what you share. " +
                    "Turn off location access when not needed."+ 
                    "";

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

