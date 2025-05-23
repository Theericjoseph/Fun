namespace ArribaEats
{
    public class DelivererMenus : UserMenus //Class that contains the menu for the deliverer after they have logged in
    {
        private MainMenu _mainMenu;
        public DelivererMenus(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }
        public override void UserMainMenu()
        {
            Menu delivererMenu = new Menu();

            Console.WriteLine("Please make a choice from the menu below:");
            delivererMenu.AddOption("Display your user information");
            delivererMenu.AddOption("List orders available to deliver");
            delivererMenu.AddOption("Arrived at restaurant to pick up order");
            delivererMenu.AddOption("Mark this delivery as complete");
            delivererMenu.AddOption("Log out");

            delivererMenu.Display();

            int choice = delivererMenu.Selection();

            const int DISPLAYUSERINFO = 1;
            const int LISTORDERS = 2;
            const int ARRIVEDATRESTAURANT = 3;
            const int MARKCOMPLETE = 4;
            const int LOGOUT = 5;

            switch (choice)
            {
                case DISPLAYUSERINFO:
                    MainMenu.DisplayUserInfo();
                    JobInfo();
                    UserMainMenu(); // Return to the main menu
                    break;
                case LISTORDERS:
                    ListDeliveries();
                    break;
                case ARRIVEDATRESTAURANT:
                    ArrivedAtRestaurant();
                    UserMainMenu(); // Return to the main menu
                    break;
                case MARKCOMPLETE:
                    MarkAsDelivered();
                    break;
                case LOGOUT:
                    Login.Logout();
                    _mainMenu.DisplayMainMenu(); //return to the main menu
                    break;
            }
        }
        private void JobInfo()
        {
            if (GlobalState.LoggedInUser is not Deliverer deliverer)
            {
                // No deliverer logged in, output nothing
                return;
            }

            // Find the first order assigned to this deliverer that is not yet delivered
            var currentOrder = Customer.Customers
                .SelectMany(cust => cust.Orders, (cust, order) => new { Customer = cust, Order = order })
                .FirstOrDefault(x => x.Order.Deliverer == deliverer && x.Order.Status != "Delivered");

            if (currentOrder == null)
            {
                // No current delivery, output nothing
                return;
            }

            Console.WriteLine("Current delivery:");
            var order = currentOrder.Order;
            var customer = currentOrder.Customer;
            var restaurant = Client.Clients.FirstOrDefault(r => r.RestName == order.RestaurantName);

            string orderNo = order.OrderNumber ?? "N/A";
            string restName = order.RestaurantName ?? "N/A";
            string restLoc = restaurant?.Location != null ? $"{restaurant.Location[0]},{restaurant.Location[1]}" : "N/A";
            string custName = customer.Name ?? "N/A";
            string custLoc = customer.Location != null ? $"{customer.Location[0]},{customer.Location[1]}" : "N/A";

            Console.WriteLine($"Order #{orderNo} from {restName} at {restLoc}.");
            Console.WriteLine($"To be delivered to {custName} at {custLoc}.");
        }

        private void ListDeliveries()
        {
            // Check if logged in user is a Deliverer
            if (GlobalState.LoggedInUser is not Deliverer deliverer)
            {
                Console.WriteLine("No deliverer is currently logged in.");
                UserMainMenu();
                return;
            }

            // If deliverer already has an order assigned
            bool hasOrder = OrderHasBeenAcceptedByDeliverer(deliverer);
            if (hasOrder)
            {
                Console.WriteLine("You have already selected an order for delivery.");
                UserMainMenu();
                return;
            }

            // Prompt for deliverer's current location
            int[]? delivererLoc = null;
            while (delivererLoc == null)
            {
                Console.WriteLine("Please enter your location (in the form of X,Y):");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    string[] parts = input.Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int x) && int.TryParse(parts[1].Trim(), out int y))
                    {
                        delivererLoc = new int[] { x, y };
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

            // Find all orders that have not been accepted by any deliverer
            var availableOrders = Customer.Customers
                .SelectMany(cust => cust.Orders, (cust, order) => new { Customer = cust, Order = order })
                .Where(x => x.Order.Deliverer == null && x.Order.Status != "Delivered")
                .OrderBy(x => int.TryParse(x.Order.OrderNumber, out int n) ? n : int.MaxValue)
                .ToList();

            if (availableOrders.Count == 0)
            {
                Console.WriteLine("There are no orders available for delivery.");
                UserMainMenu();
                return;
            }

            // Display available orders using Menu
            Menu orderMenu = new Menu();
            for (int i = 0; i < availableOrders.Count; i++)
            {
                var x = availableOrders[i];
                var order = x.Order;
                var customer = x.Customer;
                var restaurant = Client.Clients.FirstOrDefault(r => r.RestName == order.RestaurantName);

                int dist = int.MaxValue;
                if (restaurant?.Location != null && customer.Location != null)
                {
                    dist = Math.Abs(delivererLoc[0] - restaurant.Location[0]) + Math.Abs(delivererLoc[1] - restaurant.Location[1])
                         + Math.Abs(restaurant.Location[0] - customer.Location[0]) + Math.Abs(restaurant.Location[1] - customer.Location[1]);
                }

                string display = $"{order.OrderNumber ?? "N/A",-7} {order.RestaurantName ?? "N/A",-20} " +
                                 $"{(restaurant?.Location != null ? $"{restaurant.Location[0]},{restaurant.Location[1]}" : "N/A"),-8} " +
                                 $"{customer.Name ?? "N/A",-16} " +
                                 $"{(customer.Location != null ? $"{customer.Location[0]},{customer.Location[1]}" : "N/A"),-8} " +
                                 $"{dist,-5}";
                orderMenu.AddOption(display);
            }
            orderMenu.AddOption("Return to the previous menu");



            Console.WriteLine("The following orders are available for delivery. Select an order to accept it:");
            Console.WriteLine($"{"",3} {"Order",-7} {"Restaurant Name",-20} {"Loc",-8} {"Customer Name",-16} {"Loc",-8} {"Dist",-5}");


            for (int i = 0; i < availableOrders.Count; i++)
            {
                Console.WriteLine($"{i + 1,3}: {orderMenu.Options[i]}");
            }
            Console.WriteLine($"{availableOrders.Count + 1,3}: Return to the previous menu");

            Console.WriteLine("Please enter a choice between 1 and " + (availableOrders.Count + 1) + ":");


            int selection = orderMenu.Selection();

            switch (selection)
            {
                case int n when n == availableOrders.Count + 1:
                    UserMainMenu();
                    return;
                default:
                    var selected = availableOrders[selection - 1];
                    selected.Order.Deliverer = deliverer;
                    // Do NOT set the status here you silly man 
                    var restaurantSelected = Client.Clients.FirstOrDefault(r => r.RestName == selected.Order.RestaurantName);
                    string restLocStr = restaurantSelected?.Location != null ? $"{restaurantSelected.Location[0]},{restaurantSelected.Location[1]}" : "N/A";
                    Console.WriteLine($"Thanks for accepting the order. Please head to {selected.Order.RestaurantName} at {restLocStr} to pick it up.");
                    UserMainMenu();
                    break;
            }
        }

        // Helper method to check if deliverer already has an order assigned
        private bool OrderHasBeenAcceptedByDeliverer(Deliverer deliverer)
        {
            return Customer.Customers
                .SelectMany(cust => cust.Orders)
                .Any(order => order.Deliverer == deliverer && order.Status != "Delivered");
        }

        private void ArrivedAtRestaurant()
        {
            // Check if logged in user is a Deliverer
            if (GlobalState.LoggedInUser is not Deliverer deliverer)
            {
                Console.WriteLine("No deliverer is currently logged in.");
                UserMainMenu();
                return;
            }

            // Find the first order assigned to this deliverer that is not yet delivered
            var currentOrder = Customer.Customers
                .SelectMany(cust => cust.Orders, (cust, order) => new { Customer = cust, Order = order })
                .FirstOrDefault(x => x.Order.Deliverer == deliverer && x.Order.Status != "Delivered");

            if (currentOrder == null)
            {
                Console.WriteLine("You have no orders assigned.");
                UserMainMenu();
                return;
            }

            // Mark the order as arrived at the restaurant
            currentOrder.Order.DelivererArrivedAtRestaurant = true;
            Console.WriteLine($"Thanks. We have informed {currentOrder.Order.RestaurantName} that you have arrived and are ready to pick up order #{currentOrder.Order.OrderNumber}.");
            Console.WriteLine("Please show the staff this screen as confirmation.");
            if (currentOrder.Order.Status != "Cooked")
            {
                Console.WriteLine("The order is still being prepared, so please wait patiently until it is ready.");
            }
            if (currentOrder.Customer != null && currentOrder.Customer.Location != null)
            {
                Console.WriteLine($"When you have the order, please deliver it to {currentOrder.Customer.Name} at {currentOrder.Customer.Location[0]},{currentOrder.Customer.Location[1]}.");
            }
            else
            {
                Console.WriteLine("Customer information or location is unavailable for this order.");
            }
        }
        private void MarkAsDelivered()
        {
            if (GlobalState.LoggedInUser is not Deliverer deliverer)
            {
                Console.WriteLine("No deliverer is currently logged in.");
                UserMainMenu();
                return;
            }

            // Find the first order assigned to this deliverer that is not yet delivered
            var currentOrder = Customer.Customers
                .SelectMany(cust => cust.Orders, (cust, order) => new { Customer = cust, Order = order })
                .FirstOrDefault(x => x.Order.Deliverer == deliverer && x.Order.Status != "Delivered");

            if (currentOrder == null)
            {
                Console.WriteLine("You have not yet accepted an order.");
                UserMainMenu();
                return;
            }

            if (currentOrder.Order.Status != "Being Delivered")
            {
                Console.WriteLine("You have not yet picked up this order.");
                UserMainMenu();
                return;
            }

            // Mark as delivered
            currentOrder.Order.Status = "Delivered";
            Console.WriteLine("Thank you for making the delivery.");

            UserMainMenu();
        }
    }
}