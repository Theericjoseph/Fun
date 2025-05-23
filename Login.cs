using System;
using System.Collections.Generic;
using System.Linq;

namespace ArribaEats
{
    public class Login
    {
        private static bool isFirstLogin = true;
        public MainMenu _mainMenu;

        public Login(MainMenu mainMenu)
        { 
            _mainMenu = mainMenu;
        }
        public (User? user, List<User>? userList) Authenticate(string? email, string? password)
        {
            var customer = Customer.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
            if (customer != null) return (customer, Customer.Customers.Cast<User>().ToList());

            var deliverer = Deliverer.Deliverers.FirstOrDefault(d => d.Email == email && d.Password == password);
            if (deliverer != null) return (deliverer, Deliverer.Deliverers.Cast<User>().ToList());

            var client = Client.Clients.FirstOrDefault(cl => cl.Email == email && cl.Password == password);
            if (client != null) return (client, Client.Clients.Cast<User>().ToList());

            return (null, null);
        }

        public void LoginMenu()
        {
            Console.WriteLine("Email: ");
            string? email = Console.ReadLine();

            Console.WriteLine("Password: ");
            string? password = Console.ReadLine();

            var (user, userInfo) = Authenticate(email, password);
            if (user != null)
            {
                GlobalState.LoggedInUser = user;
                UserLogin(user);
            }
            else
            {
                Console.WriteLine("Invalid email or password.");
                _mainMenu.DisplayMainMenu();
            }
        }

        private void UserLogin(User user)
        {
            if (isFirstLogin)
            {
                isFirstLogin = false;
                Console.WriteLine($"Welcome back, {GlobalState.LoggedInUser?.Name}!");
            }

            switch (user)
            {
                case Customer:
                    var customerMenus = new CustomerMenus(_mainMenu);
                    customerMenus.UserMainMenu();
                    break;
                case Deliverer:
                    var delivererMenus = new DelivererMenus(_mainMenu);
                    delivererMenus.UserMainMenu();
                    break;
                case Client:
                    // You may want to implement a ClientMenus class for client-specific menus
                    var clientMenus = new ClientMenus(_mainMenu);
                    clientMenus.UserMainMenu();
                    break;
                default:
                    Console.WriteLine("Unknown user type.");
                    _mainMenu.DisplayMainMenu();
                    break;
            }
        }

        public static void Logout()
        {
            Console.WriteLine("You are now logged out.");
            GlobalState.LoggedInUser = null;
            isFirstLogin = true;
        }
    }
}