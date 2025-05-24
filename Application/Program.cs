using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application;
using BoardGamesFramework;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Select game:");
        Console.WriteLine("1 - Numerical Tic Tac Toe");
        Console.WriteLine("2 - Notakto");
        Console.WriteLine("3 - Gomoku");

        string choice = Console.ReadLine();

        Console.WriteLine("Choose mode:");
        Console.WriteLine("1 - Human vs Human");
        Console.WriteLine("2 - Human vs Computer");
        string mode = Console.ReadLine();

        Player p1 = new HumanPlayer("Player 1");
        Player p2 = mode == "2" ? new ComputerPlayer("Computer") : new HumanPlayer("Player 2");

        IGame game = GameFactory.Create(choice, p1, p2);


        if (game == null)
        {
            Console.WriteLine("Invalid game choice.");
            return;
        }

        Console.WriteLine($"Starting {game.GameName} - Mode: {(mode == "2" ? "HvC" : "HvH")}");
        ShowHelp(game);

        while (!game.IsGameOver)
        {
            game.DisplayBoard();
            Console.WriteLine($"{game.CurrentPlayer.Name}'s turn.");
            if (game.CurrentPlayer.IsHuman)
            {
                string command = Console.ReadLine();
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                string cmd = parts[0].ToLower();
                if (cmd == "move")
                {
                    Move move = null;
                    try
                    {
                        if (game is NumericalTicTacToe nt)
                        {
                            if (parts.Length != 3)
                            {
                                Console.WriteLine("Usage: move position(0-8) number(1-9)");
                                continue;
                            }
                            if (!int.TryParse(parts[1], out int pos) || !int.TryParse(parts[2], out int num))
                            {
                                Console.WriteLine("Invalid numbers.");
                                continue;
                            }
                            move = new NTTMove(pos, num);
                        }
                        else if (game is Notakto ntk)
                        {
                            if (parts.Length != 3)
                            {
                                Console.WriteLine("Usage: move boardIndex(0-2) position(0-8)");
                                continue;
                            }
                            if (!int.TryParse(parts[1], out int bIdx) || !int.TryParse(parts[2], out int pos))
                            {
                                Console.WriteLine("Invalid numbers.");
                                continue;
                            }
                            move = new NotaktoMove(bIdx, pos);
                        }
                        else if (game is Gomoku gm)
                        {
                            if (parts.Length != 3)
                            {
                                Console.WriteLine($"Usage: move x(0-{gm.BoardSize - 1}) y(0-{gm.BoardSize - 1})");
                                continue;
                            }
                            if (!int.TryParse(parts[1], out int x) || !int.TryParse(parts[2], out int y))
                            {
                                Console.WriteLine("Invalid numbers.");
                                continue;
                            }
                            char piece = game.CurrentPlayer == game.Player1 ? 'X' : 'O';
                            move = new GomokuMove(x, y, piece);
                        }
                        if (!game.IsValidMove(move))
                        {
                            Console.WriteLine("Invalid move.");
                            continue;
                        }
                        game.MakeMove(move);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                else if (cmd == "undo")
                {
                    game.Undo();
                }
                else if (cmd == "redo")
                {
                    game.Redo();
                }
                else if (cmd == "save")
                {
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Usage: save filename");
                        continue;
                    }
                    game.Save(parts[1]);
                }
                else if (cmd == "load")
                {
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Usage: load filename");
                        continue;
                    }
                    game.Load(parts[1]);
                }
                else if (cmd == "help")
                {
                    ShowHelp(game);
                }
                else if (cmd == "exit")
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
                else
                {
                    Console.WriteLine("Unknown command. Type help for commands.");
                }
            }
            else
            {
                // Computer player's turn
                var move = game.CurrentPlayer.GetMove(game);
                if (move == null)
                {
                    Console.WriteLine("No valid moves available.");
                    break;
                }
                game.MakeMove(move);
            }
        }

        game.DisplayBoard();
        if (game.Winner != null)
            Console.WriteLine($"Game Over! Winner: {game.Winner.Name}");
        else
            Console.WriteLine("Game Over! It's a draw.");

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

    }
    public static void ShowHelp(IGame game)
    {
        Console.WriteLine("Commands:");
        if (game is NumericalTicTacToe)
        {
            Console.WriteLine("move pos(0-8) number(1-9) - place your number");
        }
        else if (game is Notakto)
        {
            Console.WriteLine("move boardIndex(0-2) position(0-8) - place X");
        }
        else if (game is Gomoku gm)
        {
            Console.WriteLine($"move x(0-{gm.BoardSize - 1}) y(0-{gm.BoardSize - 1}) - place piece");
        }
        Console.WriteLine("undo - undo last move");
        Console.WriteLine("redo - redo last undone move");
        Console.WriteLine("save filename - save game");
        Console.WriteLine("load filename - load game");
        Console.WriteLine("help - show commands");
        Console.WriteLine("exit - exit game");
        Console.WriteLine();
    }

}

