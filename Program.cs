namespace ArribaEats
{
    
   internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Arriba Eats!");
            var mainMenu = new MainMenu(); //start the application by calling the main menu
            mainMenu.DisplayMainMenu();
        }
    } 
}

