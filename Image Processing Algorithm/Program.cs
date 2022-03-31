using Image_Processing_Algorithm;

// Image to modify
string input = "";
int menu;
do
{
    try
    {
        Console.WriteLine("Select a menu item");
        input = Console.ReadLine();
        menu = int.Parse(input);
    }
    catch (System.FormatException ex)
    {
        Console.WriteLine(ex.Message);
    }


} while (input.ToLower() != "quit" && input.ToLower() != "exit" && input.ToLower() != "leave");


