namespace ArribaEats
{
    public abstract class UserMenus //scaffold for menus
    {
        public abstract void UserMainMenu(); //abstract method must be inherited
    }
    public class Menu //Class that enables the creation of a menu with options, new instance of this class can be created for each menu
    {
        public List<string> Options { get; private set; } //List containing menu options
        public Menu()
        {
            Options = new List<string>();
        }

        public void AddOption(string option) //method to add new menu option
        {
            Options.Add(option);
        }
        public void Display() //method to display list of menu options
        {

            for (int i = 0; i < Options.Count; i++) //for loop that prints a menu option with desired (N: ... ) formatting, this is a scaffold that displays, it doesn't serve any other function all other logic is handled in the individual menu instances
            {
                Console.WriteLine($"{i + 1}: {Options[i]}");
            }
            Console.WriteLine($"Please enter a choice between 1 and {Options.Count}:"); //prints option to select menu between 1 and the number of options available for the user to select.
        }

        public int Selection() //handles the input logic that can be used in the methods.
        {
            while (true)
            {
                // Console.Write("Enter your choice: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int userSelect) && userSelect >= 1 && userSelect <= Options.Count) //invalid input handling
                {
                    return userSelect;
                }
                Console.WriteLine("Invalid choice.");
                Console.WriteLine($"Please enter a choice between 1 and {Options.Count}"); //repeat if wrong
            }
        }


    }

    public class MainMenu //main/intro menu class, this is global (everyone sees this) hence seperate class
    {
        private static Menu IntroMenu = new(); //new instance of menu 
        private Login _login;
        private Register _register;

        public MainMenu() //main menu constructor
        {
            _login = new Login(this);
            _register = new Register(this);
        }

        public void DisplayMainMenu()
        {
            Console.WriteLine("Please make a choice from the menu below:");

            // Clear options to avoid duplicates if MainMenu is called multiple times
            IntroMenu.Options.Clear();
            IntroMenu.AddOption("Login as a registered user");
            IntroMenu.AddOption("Register as a new user");
            IntroMenu.AddOption("Exit");

            IntroMenu.Display();

            int LoginSelection = IntroMenu.Selection();

            const int LOGIN = 1;
            const int REGISTER = 2;
            const int EXIT = 3;

            switch (LoginSelection)
            {
                case LOGIN:
                    _login.LoginMenu(); // call the login method on an instance
                    break;
                case REGISTER:
                    _register.RegisterMenu(); // call the register method
                    break;
                case EXIT:
                    Console.WriteLine("Thank you for using Arriba Eats!");
                    Environment.Exit(0); //quit because that's what the user wants
                    break;
                default:
                    Console.WriteLine("Invalid selection."); //this should never happen because the input is validated in the menu class
                    break;
            }
        }

        public static void DisplayUserInfo() //
        {
            var user = GlobalState.LoggedInUser;
            if (user == null)
            {
                Console.WriteLine("No user is currently logged in.");
                return;
            }
            user.DisplayUserInfo();
        }
    }



}