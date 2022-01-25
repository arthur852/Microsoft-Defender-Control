using Microsoft_Defender_Control;

bool isExit = false;
var defenderController = new DefenderController();

if (defenderController.IsAutoRun)
{
    defenderController.SafeModeStep();
    return;
}

do
{
    ShowMenu();
} while (!isExit);

void ShowMenu()
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"The Defender is now ");

    if (defenderController.IsEnable)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[ON]");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[OFF]");
    }

    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine("\n1. Enable Defender");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("2. Disable Defender");
    Console.ResetColor();
    Console.Write("Enter (1-2): ");
    string str = Console.ReadLine();

    switch (str)
    {
        case "1":
            defenderController.EnableDefender();
            isExit = true;
            return;
        case "2":
            defenderController.DisableDefender();
            isExit = true;
            return;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Beep();
            Console.WriteLine("Incorrect choice! Enter correct choice (1-2). For example 1 or 2");
            Console.ResetColor();
            Thread.Sleep(1400);
            Console.Clear();
            break;
    }
}