using System.Runtime.CompilerServices;

namespace ArribaEats
{
    public class ClientMenus : UserMenus
    {
        public override void UserMainMenu()
        {
            Menu clientMenu = new();

            Console.WriteLine("Please make a choice from the menu below:");
            clientMenu.AddOption("Display your user information");
            clientMenu.AddOption("Add item to restaurant menu");
            clientMenu.AddOption("See current orders");
            clientMenu.AddOption("Start cooking order");
            clientMenu.AddOption("Finish cooking order");
            clientMenu.AddOption("Handle deliverers who have arrived");
            clientMenu.AddOption("Log out");
            clientMenu.Display();

            int choice = clientMenu.Selection();

            const int DISPLAYUSERINFO = 1;
            const int ADDITEM = 2;
            const int SEEORDERS = 3;
            const int STARTCOOKING = 4;
            const int FINISHCOOKING = 5;
            const int HANDLEDISPATCH = 6;
            const int LOGOUT = 7;

            switch (choice)
            {
                case DISPLAYUSERINFO:
                    Menus.DisplayUserInfo();
                    UserMainMenu(); // Return to the main menu
                    break;
                case ADDITEM:
                    AddItemToMenu();
                    UserMainMenu(); // Return to the main menu
                    break;
                case SEEORDERS:
                    SeeCurrentOrders();
                    UserMainMenu(); // Return to the main menu
                    break;
                case STARTCOOKING:
                    StartCookingOrder();
                    UserMainMenu(); // Return to the main menu
                    break;
                case FINISHCOOKING:
                    FinishCooking();
                    UserMainMenu(); // Return to the main menu
                    break;
                case HANDLEDISPATCH:
                    HandleDispatch();
                    UserMainMenu(); // Return to the main menu
                    break;
                case LOGOUT:
                    Login.Logout();
                    Menus.MainMenu(); //return to the main menu
                    break;
            }
        }

        public void AddItemToMenu()
        {
            // Display current menu
            Console.WriteLine("This is your restaurant's current menu:");
            if (GlobalState.LoggedInUser is not Client client)
            {
                Console.WriteLine("Error: No client is currently logged in.");
                UserMainMenu();
                return;
            }

            foreach (var item in client.MenuItems)
            {
                // $#.## right-justified in a column 7 spaces wide
                Console.WriteLine($"{("$" + item.ItemPrice.GetValueOrDefault().ToString("0.00")).PadLeft(7)}  {item.ItemName}");
            }

            // Prompt for new item name
            Console.WriteLine("Please enter the name of the new item (blank to cancel):");
            string? itemName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(itemName))
            {
                UserMainMenu();
                return;
            }

            // Prompt for price
            float price;
            while (true)
            {
                Console.WriteLine("Please enter the price of the new item (without the $):");
                string? itemPriceInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(itemPriceInput) ||
                    !float.TryParse(itemPriceInput, out price) ||
                    price < 0f || price > 999.99f)
                {
                    Console.WriteLine("Invalid price.");
                }
                else
                {
                    break;
                }
            }

            // Add new item to menu
            var newItem = new MenuItem
            {
                ItemName = itemName,
                ItemPrice = price
            };
            client.MenuItems.Add(newItem);
            Console.WriteLine($"Successfully added {itemName} (${price:0.00}) to menu.");

            UserMainMenu();
        }

        private void SeeCurrentOrders()
        {
            if (GlobalState.LoggedInUser is not Client client)
            {
                Console.WriteLine("No client is currently logged in.");
                UserMainMenu();
                return;
            }

            var orders = GetOrdersForClient(client);

            foreach (var order in orders)
            {
                // Find the customer who placed this order
                var customer = Customer.Customers.FirstOrDefault(c => c.Orders.Contains(order));
                string customerName = customer?.Name ?? "Unknown";

                Console.WriteLine($"Order #{order.OrderNumber} for {customerName}: {order.Status}");
                foreach (var (item, quantity) in order.Items)
                {
                    Console.WriteLine($"{quantity} x {item.ItemName}");
                }
                Console.WriteLine();
            }
        }

        // Example method to get all orders for the logged-in client
        private List<Order> GetOrdersForClient(Client client)
        {
            return Customer.Customers
                .SelectMany(cust => cust.Orders)
                .Where(order => order.RestaurantName == client.RestName)
                .ToList();
        }

        private void StartCookingOrder()
        {
            if (GlobalState.LoggedInUser is not Client client)
            {
                Console.WriteLine("No client is currently logged in.");
                UserMainMenu();
                return;
            }

            // Get all orders for this restaurant with status "Ordered"
            var orders = GetOrdersForClient(client)
                .Where(order => order.Status == "Ordered")
                .ToList();


            Menu orderMenu = new Menu();
            foreach (var order in orders)
            {
                // Find the customer who placed this order
                var customer = Customer.Customers.FirstOrDefault(c => c.Orders.Contains(order));
                string customerName = customer?.Name ?? "Unknown";
                orderMenu.AddOption($"Order #{order.OrderNumber} for {customerName}");
            }
            orderMenu.AddOption("Return to the previous menu");

            Console.WriteLine("Select an order once you are ready to start cooking:");
            orderMenu.Display();

            int selection = orderMenu.Selection();

            if (selection == orders.Count + 1)
            {
                UserMainMenu();
                return;
            }

            var selectedOrder = orders[selection - 1];
            selectedOrder.Status = "Cooking";
            var selectedCustomer = Customer.Customers.FirstOrDefault(c => c.Orders.Contains(selectedOrder));
            string selectedOrderNo = selectedOrder.OrderNumber ?? "N/A";

            Console.WriteLine($"Order #{selectedOrderNo} is now marked as cooking. Please prepare the order, then mark it as finished cooking:");

            // Combine items by name for display (same as CustomerOrderStatus)
            var itemGroups = selectedOrder.Items
                .GroupBy(i => i.Item.ItemName)
                .Select(g => new
                {
                    Name = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                });

            foreach (var item in itemGroups)
            {
                Console.WriteLine($"{item.Quantity} x {item.Name}");
            }

            UserMainMenu();
        }

        private void FinishCooking()
        {
            if (GlobalState.LoggedInUser is not Client client)
            {
                Console.WriteLine("No client is currently logged in.");
                UserMainMenu();
                return;
            }

            // Get all orders for this restaurant with status "Cooking"
            var orders = GetOrdersForClient(client)
                .Where(order => order.Status == "Cooking")
                .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("There are no orders currently being cooked.");
                UserMainMenu();
                return;
            }

            Menu orderMenu = new Menu();
            foreach (var order in orders)
            {
                // Find the customer who placed this order
                var customer = Customer.Customers.FirstOrDefault(c => c.Orders.Contains(order));
                string customerName = customer?.Name ?? "Unknown";
                orderMenu.AddOption($"Order #{order.OrderNumber} for {customerName}");
            }
            orderMenu.AddOption("Return to the previous menu");

            Console.WriteLine("Select an order once you have finished preparing it:");
            orderMenu.Display();

            int selection = orderMenu.Selection();

            if (selection == orders.Count + 1)
            {
                UserMainMenu();
                return;
            }

            var selectedOrder = orders[selection - 1];
            selectedOrder.Status = "Cooked";
            string selectedOrderNo = selectedOrder.OrderNumber ?? "N/A";

            Console.WriteLine($"Order #{selectedOrderNo} is now ready for collection.");

            if (selectedOrder.Deliverer == null)
            {
                Console.WriteLine("No deliverer has been assigned yet.");
            }
            else
            {
                string licencePlate = selectedOrder.Deliverer.Licence ?? "Unknown";
                if (selectedOrder.DelivererArrivedAtRestaurant)
                {
                    Console.WriteLine($"Please take it to the deliverer with licence plate {licencePlate}, who is waiting to collect it.");
                }
                else
                {
                    Console.WriteLine($"The deliverer with licence plate {licencePlate} will be arriving soon to collect it.");
                }
            }

            UserMainMenu();
        }

        private void HandleDispatch()
        {
            if (GlobalState.LoggedInUser is not Client client)
            {
                Console.WriteLine("No client is currently logged in.");
                UserMainMenu();
                return;
            }

            // Find orders for this restaurant where:
            // - There is a deliverer assigned
            // - The deliverer has arrived at the restaurant
            // - The order is not yet "Being Delivered" or "Delivered"
            var orders = GetOrdersForClient(client)
                .Where(order =>
                    order.Deliverer != null &&
                    order.DelivererArrivedAtRestaurant &&
                    order.Status != "Being Delivered" &&
                    order.Status != "Delivered")
                .ToList();


            Menu orderMenu = new Menu();
            foreach (var order in orders)
            {
                var customer = Customer.Customers.FirstOrDefault(c => c.Orders.Contains(order));
                string customerName = customer?.Name ?? "Unknown";
                string licencePlate = order.Deliverer?.Licence ?? "Unknown";
                string status = order.Status ?? "Unknown";
                orderMenu.AddOption($"Order #{order.OrderNumber} for {customerName} (Deliverer licence plate: {licencePlate}) (Order status: {status})");
            }
            Console.WriteLine("These deliverers have arrived and are waiting to collect orders.");
            Console.WriteLine("Select an order to indicate that the deliverer has collected it:");

            orderMenu.AddOption("Return to the previous menu");


            orderMenu.Display();

            int selection = orderMenu.Selection();

            if (selection == orders.Count + 1)
            {
                UserMainMenu();
                return;
            }

            var selectedOrder = orders[selection - 1];

            if (selectedOrder.Status != "Cooked")
            {
                Console.WriteLine("This order has not yet been cooked.");
                UserMainMenu();
                return;
            }

            selectedOrder.Status = "Being Delivered";
            Console.WriteLine($"Order #{selectedOrder.OrderNumber} is now marked as being delivered.");

            UserMainMenu();
        }
    }
}