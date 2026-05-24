# CyberChatBot GUI
Project Overview
This project is a Windows Forms-based chatbot application designed to educate users about basic cybersecurity concepts. The chatbot interacts with users through a graphical user interface and provides responses based on keyword recognition, sentiment detection, and conversational flow.

The goal of the application is to simulate a simple intelligent assistant that helps users learn about staying safe online.

Features

1. Graphical User Interface (GUI)
- Built using Windows Forms (C#)
- Includes a chat display window, input textbox, and send button
- Styled interface with improved colours and layout for better user experience

2. Keyword Recognition
The chatbot detects cybersecurity-related keywords and responds accordingly:
- Password safety
- Phishing/scams
- Privacy protection

3. Sentiment Detection
The chatbot can detect simple user emotions such as:
- Worried
- Frustrated
- Curious

It adjusts responses to be more supportive and helpful based on user sentiment.

4. Memory Feature
The chatbot remembers the current topic of conversation (e.g., password, phishing, privacy) and uses this information to continue relevant discussions.

5. Conversation Flow
The chatbot supports follow-up inputs such as:
- "tell me more"
- "another tip"

This allows for a continuous and natural conversation experience.

6. Random Responses
For phishing-related queries, the chatbot randomly selects from multiple predefined responses to make interactions more dynamic.

7. Delegates
A delegate is used to handle chatbot responses, demonstrating event-driven programming and flexible method referencing.

8. Exit Function
The user can end the conversation by typing:
- "exit"
- "quit"

The chatbot will respond with a goodbye message and close the application.

Technologies Used
- C#
- Windows Forms (WinForms)
- .NET Framework
- Visual Studio

How to Run the Project
1. Open the project in Visual Studio
2. Build the solution
3. Run the application (Start button or F5)
4. Type messages into the chatbot interface

Author
Cybersecurity Chatbot Project — Student Implementation for POE Part 2

Notes
This chatbot is designed for educational purposes only and does not store real user data or connect to external services.
