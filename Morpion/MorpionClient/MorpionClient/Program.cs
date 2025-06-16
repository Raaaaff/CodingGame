using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class MorpionClient
{
    static TcpClient client;
    static NetworkStream stream;
    static string[] board = new string[9];
    static int playerId = -1;
    static int currentPlayer = -1;

    static void Main()
    {
        client = new TcpClient("127.0.0.1", 5000);
        stream = client.GetStream();
        Console.WriteLine("Connecté au serveur...");

        // Recevoir l'ID joueur
        byte[] buffer = new byte[1024];
        int bytes = stream.Read(buffer, 0, buffer.Length);
        string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
        if (msg.StartsWith("ID:"))
        {
            playerId = int.Parse(msg.Substring(3));
            Console.WriteLine($"Vous êtes le joueur {(playerId == 0 ? "X" : "O")} (ID: {playerId})");
        }

        new Thread(ReceiveLoop).Start();
    }

    static void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

            if (msg.StartsWith("BOARD:"))
            {
                string[] parts = msg.Substring(6).Split(';');
                board = parts[0].Split(',');
                currentPlayer = int.Parse(parts[1]);
                DisplayBoard();
            }
            else if (msg == "YOURTURN")
            {
                bool validMove = false;

                while (!validMove)
                {
                    Console.Write("Entrez une case (0-8) : ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 8)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(input);
                        stream.Write(data, 0, data.Length);

                        byte[] responseBuffer = new byte[1024];
                        int responseBytes = stream.Read(responseBuffer, 0, responseBuffer.Length);
                        string response = Encoding.UTF8.GetString(responseBuffer, 0, responseBytes);

                        if (response == "INVALID")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Case déjà utilisée ou coup invalide.");
                            Console.ResetColor();
                        }
                        else
                        {
                            validMove = true;
                            ProcessServerMessage(response);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entrée invalide. Entrez un chiffre entre 0 et 8.");
                    }
                }
            }
            else if (msg.StartsWith("END:"))
            {
                Console.Clear();
                Console.WriteLine("Partie terminée !");
                Console.WriteLine(msg.Substring(4));
                Environment.Exit(0);
            }
        }
    }

    static void ProcessServerMessage(string msg)
    {
        if (msg.StartsWith("BOARD:"))
        {
            string[] parts = msg.Substring(6).Split(';');
            board = parts[0].Split(',');
            currentPlayer = int.Parse(parts[1]);
            DisplayBoard();
        }
        else if (msg.StartsWith("END:"))
        {
            Console.Clear();
            Console.WriteLine("Partie terminée !");
            Console.WriteLine(msg.Substring(4));
            Environment.Exit(0);
        }
    }

    static void DisplayBoard()
    {
        Console.Clear();
        Console.WriteLine($"C'est au joueur {(currentPlayer == 0 ? "X" : "O")} de jouer\n");

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == " ")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" {i} ");
                Console.ResetColor();
            }
            else
            {
                Console.Write($" {board[i]} ");
            }

            if (i % 3 != 2) Console.Write("|");
            if (i % 3 == 2 && i != 8) Console.WriteLine("\n-----------");
        }
        Console.WriteLine();
    }
}
