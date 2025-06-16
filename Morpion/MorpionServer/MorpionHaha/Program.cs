using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class MorpionServer
{
    static TcpListener listener;
    static string[] board = new string[9];
    static TcpClient[] clients = new TcpClient[2];
    static NetworkStream[] streams = new NetworkStream[2];
    static int currentPlayer = 0;
    static object lockObj = new object();

    static void Main()
    {
        for (int i = 0; i < 9; i++) board[i] = " ";
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Serveur démarré sur le port 5000...");

        // Connexion des 2 clients
        for (int i = 0; i < 2; i++)
        {
            clients[i] = listener.AcceptTcpClient();
            streams[i] = clients[i].GetStream();
            SendToClient(i, $"ID:{i}");
            int id = i;
            new Thread(() => HandleClient(id)).Start();
            Console.WriteLine($"Client {id} connecté");
        }

        BroadcastBoard();
    }

    static void HandleClient(int id)
    {
        NetworkStream stream = streams[id];

        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytes);

                if (int.TryParse(message, out int move) && move >= 0 && move < 9)
                {
                    lock (lockObj)
                    {
                        if (id == currentPlayer && board[move] == " ")
                        {
                            board[move] = id == 0 ? "X" : "O";
                            currentPlayer = 1 - currentPlayer;
                            BroadcastBoard();
                            CheckGameOver();
                        }
                        else
                        {
                            SendToClient(id, "INVALID");
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Client {id} déconnecté.");
                return;
            }
        }
    }

    static void SendToClient(int id, string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        streams[id].Write(data, 0, data.Length);
    }

    static void BroadcastBoard()
    {
        string state = string.Join(",", board) + ";" + currentPlayer;
        string fullMsg = "BOARD:" + state;

        for (int i = 0; i < 2; i++)
        {
            streams[i].Write(Encoding.UTF8.GetBytes(fullMsg));
            if (i == currentPlayer)
            {
                streams[i].Write(Encoding.UTF8.GetBytes("YOURTURN"));
            }
        }
    }

    static void CheckGameOver()
    {
        int[][] wins = {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
            new[] {0,4,8}, new[] {2,4,6}
        };

        foreach (var combo in wins)
        {
            if (board[combo[0]] != " " &&
                board[combo[0]] == board[combo[1]] &&
                board[combo[1]] == board[combo[2]])
            {
                string msg = $"END:Le joueur {(board[combo[0]] == "X" ? "X (0)" : "O (1)")} a gagné !";
                BroadcastMessage(msg);
                return;
            }
        }

        if (Array.TrueForAll(board, b => b != " "))
        {
            BroadcastMessage("END:Match nul !");
        }
    }

    static void BroadcastMessage(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        foreach (var stream in streams)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}
