﻿using System;
using System.Diagnostics.CodeAnalysis;
using Cli.Commands;
using System.Text.Json;

namespace Cli
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string idToken = null;
            string accessToken = null;
            string email = null;
            string name = null;

            Console.WriteLine("\u001b[36mWelcome to doc#umentation translator!\u001b[0m");
            Console.WriteLine("Type 'help' to view commands\n");

            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();
                string[] inputs = input.Split(' ');
                string command = inputs[0].ToLower();

                if (command == "help")
                {
                    HelpCommand.Run();
                }
                else if (command == "clear")
                {
                    ClearCommand.Run();
                }
                else if (command == "login")
                {
                    Console.WriteLine("Requesting Authorization");
                    string clientId = "";
                    string clientSecret = "";
                    string[] scopes = new string[] { "https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/userinfo.profile" };
                    string redirectUri = "";
                    try
                    {
                        using (StreamReader reader = new StreamReader("client_secrets.json"))
                        {
                            string json = reader.ReadToEnd();
                            JsonDocument document = JsonDocument.Parse(json);
                            JsonElement root = document.RootElement;
                            clientId = root.GetProperty("installed").GetProperty("client_id").GetString();
                            clientSecret = root.GetProperty("installed").GetProperty("client_secret").GetString();
							redirectUri = root.GetProperty("installed").GetProperty("redirect_uri").GetString();
						}
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\u001b[31mError reading client secrets: {0}\u001b[0m", ex.Message);
                        return;
                    }

                    (idToken, accessToken, name, email) = await LoginCommand.Run(clientId, clientSecret, redirectUri);

                    await LanguagesCommand.InitLanguages(idToken);
                }
                else if (command == "logout")
                {
                    if (accessToken !=  null)
                    {
                        await LogoutCommand.Run(accessToken);
                        idToken = null;
                        accessToken = null;
                        email = null;
                        name = null;
                    }
                    else
                    {
                        Console.WriteLine("\u001b[31mYou must be logged in to logout\u001b[0m");
                    }
                }
                else if (command == "translate")
                {
                    if (accessToken != null)
                    {
                        if (inputs.Length < 3)
                        {
                            Console.WriteLine("\u001b[31mPlease enter a valid language and filepath\u001b[0m");
                        }
                        else
                        {
                            string language = inputs[1];

							string filepath = string.Join(" ", inputs, 2, inputs.Length - 2);

							if (filepath.StartsWith("\"") && filepath.EndsWith("\""))
							{
								filepath = filepath.Substring(1, filepath.Length - 2);
							}

							await TranslateCommand.Run(idToken, language, filepath);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\u001b[31mYou must be logged in to use this command\u001b[0m");
                    }
                }
                else if (command == "documents")
                {
                    if (accessToken != null)
                    {
                        await DocumentsCommand.Run(idToken);
                    }
                    else
                    {
                        Console.WriteLine("\u001b[31mYou must be logged in to use this command\u001b[0m");
                    }
                }
				else if (command == "languages")
				{
					if (accessToken != null)
					{
						LanguagesCommand.Run();
					}
					else
					{
						Console.WriteLine("\u001b[31mYou must be logged in to use this command\u001b[0m");
					}
				}
				else if (command == "download")
                {
                    if (accessToken != null)
                    {
                        if (inputs.Length < 3)
                        {
                            Console.WriteLine("\u001b[31mPlease enter a valid document id and directory path\u001b[0m");
                        }
                        else
                        {
                            string filename = inputs[1];
                            string filepath = inputs[2];
                            await DownloadCommand.Run(idToken, filename, filepath);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\u001b[31mYou must be logged in to use this command\u001b[0m");
                    }
                }
                else if (command == "exit")
                {
                    ExitCommand.Run();
                }
                else
                {
                    Console.WriteLine("\u001b[31mInvalid command. Enter 'help' to view list of valid commands\u001b[0m");
                }
            }
        }
    }
}
