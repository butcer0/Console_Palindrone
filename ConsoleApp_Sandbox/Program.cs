using System;
using System.Linq;
using ConsoleApp_Sandbox.Consts;
using ConsoleApp_Sandbox.Exceptions;
using ConsoleApp_Sandbox.Puzzles;

namespace ConsoleApp_Sandbox
{
    class Program
    {
        const string MENU_QUESETION_TEXT = "Which app would you like to run?";
        const string GOODBYE_MESSAGE = "Have a great day!";
        const string CONTINUE_MESSAGE = "\nPress enter to continue...";
        
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.Title = "Sandbox Applications";
                    switch (selectApp())
                    {
                        case SANDBOX_APPS.PALINDRONE:
                            PalindronePuzzle.Run();
                            break;
                        case SANDBOX_APPS.PATHFINDING:
                            PathFindingPuzzle.Run();
                            break;
                        default:
                            throw new IntendedExitException();
                    }

                    Console.Write(CONTINUE_MESSAGE);
                    Console.ReadLine();
                }
            }
            catch (IntendedExitException)
            {
                Console.Clear();
                Console.WriteLine(GOODBYE_MESSAGE);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception was thrown {e.Message}");
            }
           
        }

        static SANDBOX_APPS selectApp()
        {
            Console.Clear();
            Console.WriteLine(MENU_QUESETION_TEXT);
            foreach (var x in Enum.GetNames(typeof(SANDBOX_APPS)).Select((appEnum, index) => new {index, appEnum}))
            {
                Console.WriteLine($"{x.index.ToString()}. {x.appEnum}");
            }
            Console.Write("Selection: ");
            var selectedAppRaw = Console.ReadLine();
            var exitIndex = (int)SANDBOX_APPS.EXIT;
            var intParseSuccess = int.TryParse(selectedAppRaw, out var selectedAppValue);
            Console.Clear();
            return (intParseSuccess && selectedAppValue < (exitIndex))
                ? (SANDBOX_APPS) selectedAppValue
                : SANDBOX_APPS.EXIT;
        }
    }
}