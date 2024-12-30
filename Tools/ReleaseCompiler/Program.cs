using System.Diagnostics;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Deploying Expense Tracker Tool");
        // TODO: change compile version
        Process.Start("build_expense_tracker_release_x64.bat");
    }
}