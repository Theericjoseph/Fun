namespace ArribaEats
{
    public class Register
    {
        private MainMenu _mainMenu;

        public Register(MainMenu mainMenu) {
            _mainMenu = mainMenu;
        }
        public void RegisterMenu()
        {
            Menu RegisterMenu = new();

            Console.WriteLine("Which type of user would you like to register as?\n");
            RegisterMenu.AddOption("Customer");
            RegisterMenu.AddOption("Deliverer");
            RegisterMenu.AddOption("Client");
            RegisterMenu.AddOption("Return to the previous menu");

            RegisterMenu.Display();

            int selection = RegisterMenu.Selection();

            const int CUSTOMER = 1;
            const int DELIVERER = 2;
            const int CLIENT = 3;
            const int RETURN = 4;


            User? UserType;
            switch (selection)
            {
                case CUSTOMER:
                    UserType = new Customer();
                    UniversalSignUp();
                    LocationCheck();
                    SignUpFinish(UserType);
                    break;
                case DELIVERER:
                    UserType = new Deliverer();
                    UniversalSignUp();
                    LicenceCheck();
                    SignUpFinish(UserType);
                    break;
                case CLIENT:
                    UserType = new Client();
                    UniversalSignUp();
                    RestaurantCheck();
                    StyleSelect();
                    LocationCheck();
                    SignUpFinish(UserType);
                    break;
                case RETURN:
                    _mainMenu.DisplayMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid selection.");
                    break;
            }
        }

        private void SignUpFinish(User UserType)
        {
            UserType.Name = name;
            UserType.Age = age;
            UserType.Email = email;
            UserType.Phone = phone;
            UserType.Password = password;

            switch (UserType)
            {
                case Deliverer deliverer:
                    deliverer.Licence = licence;
                    Deliverer.Deliverers.Add(deliverer);
                    break;

                case Client client:
                    client.RestName = RestName;
                    client.FoodStyle = foodstyle;
                    client.Location = location;
                    Client.Clients.Add(client);
                    break;

                case Customer customer:
                    customer.Location = location;
                    Customer.Customers.Add(customer);
                    break;

                default:
                    Console.WriteLine("Unknown user type.");
                    break;
            }

            Console.WriteLine($"You have been successfully registered as a {UserType.GetType().Name.ToLower()}, {name}!");
            _mainMenu.DisplayMainMenu();
        }

        private string? name = null;
        private int? age = null;
        private string? email = null;
        private string? phone = null;


        private void UniversalSignUp() //All user accounts have this data, this process is the same for all users
        {
            // Reset fields to ensure fresh input each time
            name = null;
            age = null;
            email = null;
            phone = null;
            password = null;

            while (name == null)
            {
                Console.WriteLine("Please enter your name: ");
                string? inputName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputName)
                    && inputName.All(c => char.IsLetter(c) || c == ' ' || c == '\'' || c == '-'))
                {
                    name = inputName;
                }
                else
                {
                    Console.WriteLine("Invalid name.");
                }
            }

            while (age == null)
            {
                Console.WriteLine("Please enter your age (18-100):");
                string? inputAge = Console.ReadLine();
                if (int.TryParse(inputAge, out int parsedAge) && parsedAge >= 18 && parsedAge <= 100)
                {
                    age = parsedAge;
                }
                else
                {
                    Console.WriteLine("Invalid age.");
                }
            }


            while (email == null)
            {
                Console.WriteLine("Please enter your email address: ");
                string? inputEmail = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(inputEmail)
                    || !inputEmail.Contains('@')
                    || !inputEmail.Contains('.')
                    || inputEmail.IndexOf('@') <= 0)
                {
                    Console.WriteLine("Invalid email address.");
                    continue; // prompt again
                }

                // Check if email is already in use
                bool emailExists =
                    Customer.Customers.Any(c => c.Email == inputEmail) ||
                    Deliverer.Deliverers.Any(d => d.Email == inputEmail) ||
                    Client.Clients.Any(cl => cl.Email == inputEmail);

                if (emailExists)
                {
                    Console.WriteLine("This email address is already in use.");
                    continue; // prompt again
                }

                // Both checks passed
                email = inputEmail;
            }


            while (phone == null)
            {
                Console.WriteLine("Please enter your mobile phone number: "); // 10 digit number, no letters
                string? inputPhone = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputPhone)
                    && inputPhone.Length == 10
                    && inputPhone.All(char.IsDigit)
                    && inputPhone.StartsWith("0"))
                {
                    phone = inputPhone;
                }
                else
                {
                    Console.WriteLine("Invalid phone number.");
                }
            }

            password = PasswordProcess();

        }

        private string? password = null;
        private string PasswordProcess()
        {

            string? inputPassword = null;
            while (password == null)
            {
                while (password == null)
                {
                    Console.WriteLine("Your password must: \n-be at least 8 characters long\n- contain a number\n- contain a lowercase letter\n- contain an uppercase letter");
                    Console.WriteLine("Please enter a password: ");

                    inputPassword = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(inputPassword) && inputPassword.Length >= 8 && inputPassword.Any(char.IsDigit) && inputPassword.Any(char.IsLower) && inputPassword.Any(char.IsUpper))
                    {
                        password = inputPassword;
                    }
                    else
                    {
                        Console.WriteLine("Invalid password.");
                        password = null; // Reset password to null to prompt for re-entry
                    }
                }

                Console.WriteLine("Please confirm your password: ");
                string? inputPasswordConfirm = Console.ReadLine();
                if (inputPassword == inputPasswordConfirm)
                {
                    break; // Passwords match, exit the loop
                }
                else
                {
                    Console.WriteLine("Passwords do not match.");
                    password = null; // Reset password to null to prompt for re-entry
                }
            }

            return password; // Return the valid password, 
        }

        private int[]? location = null; //location is an int array [x, y] (x and y are both integers)

        public int[] LocationCheck()
        {
            location = null; //reset location to null to ensure we get a new input

            while (location == null) //loop until we get a valid location
            {
                Console.WriteLine("Please enter your location (in the form of X,Y): ");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    string[] parts = input.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int x) && int.TryParse(parts[1].Trim(), out int y)) //input cleaning
                    {
                        // Store as int array [x, y]
                        location = [x, y];
                        return (int[])location; //return the location as an int array
                    }
                    else
                    {
                        Console.WriteLine("Invalid location.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid location.");
                }

            }
            return location ?? [0, 0]; //return default value if location is null (it should never be null here)
        }

        private string? licence = null; //licence is a string
        public string LicenceCheck() //Must be between 1 and 8 characters long, may only contain uppercase letters, numbers and spaces, and may not consist entirely of spaces
        {
            while (licence == null) //loop until we get a valid licence plate
            {
                Console.WriteLine("Please enter your licence plate:");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.Length >= 1 && input.Length <= 8 && input.All(c => char.IsUpper(c) || char.IsDigit(c) || char.IsWhiteSpace(c)))
                {
                    licence = input;
                    return licence; //return the licence plate
                }
                else
                {
                    Console.WriteLine("Invalid licence plate.");

                }
            }
            return licence ?? new string(""); //default case to satisfy the compiler like the location check
        }

        private string? RestName;

        public string RestaurantCheck()
        {

            while (RestName == null) //loop until we get a valid restaurant name
            {
                Console.WriteLine("Please enter your restaurant's name:");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    RestName = input; //return the restaurant name
                }
                else
                {
                    Console.WriteLine("Invalid restaurant name.");
                }
            }
            return RestName ?? new string(""); //default case to satisfy the compiler like the location check and licence check
        }

        private string? foodstyle;
        private void StyleSelect()
        {
            Menu StyleMenu = new();
            Console.WriteLine("Please select your restaurant's style:");
            StyleMenu.AddOption("Italian");
            StyleMenu.AddOption("French");
            StyleMenu.AddOption("Chinese");
            StyleMenu.AddOption("Japanese");
            StyleMenu.AddOption("American");
            StyleMenu.AddOption("Australian");

            StyleMenu.Display();

            const int ITALIAN = 1;
            const int FRENCH = 2;
            const int CHINESE = 3;
            const int JAPANESE = 4;
            const int AMERICAN = 5;
            const int AUSTRALIAN = 6;


            int selection = StyleMenu.Selection();
            switch (selection)
            {
                case ITALIAN:
                    foodstyle = "Italian";
                    break;
                case FRENCH:
                    foodstyle = "French";
                    break;
                case CHINESE:
                    foodstyle = "Chinese";
                    break;
                case JAPANESE:
                    foodstyle = "Japanese";
                    break;
                case AMERICAN:
                    foodstyle = "American";
                    break;
                case AUSTRALIAN:
                    foodstyle = "Australian";
                    break;
            }
        }
    }
}