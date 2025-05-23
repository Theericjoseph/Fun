namespace ArribaEats
{
    public class CustomerMenus : UserMenus //Class that contains the menu for the customer after they have logged in
    {
        private MainMenu _mainMenu;

        public CustomerMenus(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        //Instance of the Login class to access its methods
        public override void UserMainMenu() // 
        {
            Menu customerMenu = new();

            Console.WriteLine("Please make a choice from the menu below:");
            customerMenu.AddOption("Display your user information");
            customerMenu.AddOption("Select a list of restaurants to order from");
            customerMenu.AddOption("See the status of your orders");
            customerMenu.AddOption("Rate a restaurant you've ordered from");
            customerMenu.AddOption("Log out");

            customerMenu.Display();
            int choice = customerMenu.Selection();

            const int USERINFO = 1;
            const int SELECTRESTAURANTS = 2;
            const int ORDERSTATUS = 3;
            const int RATERESTAURANT = 4;
            const int LOGOUT = 5;

            switch (choice)
            {
                case USERINFO:
                    // Display user information
                    MainMenu.DisplayUserInfo();
                    UserMainMenu(); // Return to the main menu
                    break;
                case SELECTRESTAURANTS:
                    RestaurantList(); // Call the method to sort the restaurant list
                    UserMainMenu(); // Return to the main menu
                    break;
                case ORDERSTATUS:
                    CustomerOrderStatus();
                    UserMainMenu(); // Return to the main menu
                    break;
                case RATERESTAURANT:
                    CustomerRateRestaurant();
                    break;
                case LOGOUT:
                    Login.Logout();
                    _mainMenu.DisplayMainMenu(); //return to the main menu
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }


        public void RestaurantList()
        {
            Menu RestaurantList = new Menu();

            Console.WriteLine("How would you like the list of restaurants ordered?");

            RestaurantList.AddOption("Sorted alphabetically by name");
            RestaurantList.AddOption("Sorted by distance");
            RestaurantList.AddOption("Sorted by style");
            RestaurantList.AddOption("Sorted by average rating");
            RestaurantList.AddOption("Return to the previous menu");

            RestaurantList.Display();

            int choice = RestaurantList.Selection();

            const int ALPHABETICAL = 1;
            const int DISTANCE = 2;
            const int STYLE = 3;
            const int RATING = 4;
            const int RETURN = 5;

            // Get all restaurants (clients)
            var restaurants = Client.Clients.ToList();

            // Get logged-in customer location for distance calculation
            var customer = GlobalState.LoggedInUser as Customer;
            int custX = 0, custY = 0;
            if (customer?.Location != null && customer.Location.Length >= 2)
            {
                custX = customer.Location[0];
                custY = customer.Location[1];
            }
            int TaxicabDist(int[]? loc)
            {
                if (loc == null || loc.Length < 2) return int.MaxValue;
                return Math.Abs(custX - loc[0]) + Math.Abs(custY - loc[1]);
            }

            // Style order for sorting
            Dictionary<string, int> styleOrder = new()
            {
                {"Italian", 1},
                {"French", 2},
                {"Chinese", 3},
                {"Japanese", 4},
                {"American", 5},
                {"Australian", 6}
            };

            switch (choice)
            {
                case ALPHABETICAL:
                    restaurants = restaurants.OrderBy(r => r.RestName, StringComparer.Ordinal).ToList();
                    break;
                case DISTANCE:
                    restaurants = restaurants.OrderBy(r => TaxicabDist(r.Location))
                                             .ThenBy(r => r.RestName, StringComparer.Ordinal)
                                             .ToList();
                    break;
                case STYLE:
                    restaurants = restaurants.OrderBy(r => styleOrder.ContainsKey(r.FoodStyle ?? "") ? styleOrder[r.FoodStyle!] : int.MaxValue)
                                             .ThenBy(r => r.RestName, StringComparer.Ordinal)
                                             .ToList();
                    break;
                case RATING:
                    restaurants = restaurants.OrderByDescending(r => r.AverageRating ?? -1)
                                             .ThenBy(r => r.RestName, StringComparer.Ordinal)
                                             .ToList();
                    break;
                case RETURN:
                    // Return to customer main menu
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            // Display restaurant table
            Console.WriteLine();
            Console.WriteLine("You can order from the following restaurants:");
            Console.WriteLine($"{"",3} {"Restaurant Name",-20} {"Loc",-8} {"Dist",-6} {"Style",-12} {"Rating",-7}");

            for (int i = 0; i < restaurants.Count; i++)
            {
                var r = restaurants[i];
                string name = r.RestName ?? "N/A";
                string locStr = (r.Location != null) ? string.Join(",", r.Location) : "N/A";
                int dist = TaxicabDist(r.Location);
                string style = r.FoodStyle ?? "N/A";
                string rating = r.AverageRating.HasValue ? r.AverageRating.Value.ToString("0.0") : "-";
                Console.WriteLine($"{i + 1,3}: {name,-20} {locStr,-8} {dist,-6} {style,-12} {rating,-7}");
            }
            int returnOption = restaurants.Count + 1;
            Console.WriteLine($"{returnOption,3}: Return to the previous menu");
            Console.WriteLine($"Please enter a choice between 1 and {returnOption}:");

            int selection;
            while (!int.TryParse(Console.ReadLine(), out selection) || selection < 1 || selection > returnOption)
            {
                Console.WriteLine($"Invalid choice. Please enter a choice between 1 and {returnOption}:");
            }
            if (selection == returnOption)
            {
                // Return to customer main menu
                return;
            }

            var selectedRestaurant = restaurants[selection - 1];

            Console.WriteLine($"\nPlacing order from {selectedRestaurant.RestName}.");

            // Show the restaurant menu (order/reviews/return)
            while (true)
            {
                Menu restaurantMenu = new Menu();
                restaurantMenu.AddOption("See this restaurant's menu and place an order");
                restaurantMenu.AddOption("See reviews for this restaurant");
                restaurantMenu.AddOption("Return to main menu");


                restaurantMenu.Display();
                int restaurantChoice = restaurantMenu.Selection();

                if (restaurantChoice == 1)
                {
                    // Place order
                    if (customer != null)
                        Order.PlaceOrder(customer, selectedRestaurant);
                    // After placing/cancelling order, show this menu again
                }
                else if (restaurantChoice == 2)
                {
                    RestaurantReviews(selectedRestaurant);
                    // Loop back to restaurant menu
                }
                else if (restaurantChoice == 3)
                {
                    // Return to restaurant list
                    return;
                }
            }
        }

        public void CustomerOrderStatus()
        {
            if (GlobalState.LoggedInUser is not Customer user)
            {
                Console.WriteLine("No user is currently logged in.");
                return;
            }

            // Assume Customer has a property: public List<Order> Orders { get; set; } = new();
            var orders = user?.Orders;
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("You have not placed any orders.");
                return;
            }

            foreach (var order in orders)
            {
                // Assume Order has: OrderNumber, RestaurantName, Status, Deliverer, Items
                // Deliverer is Deliverer object or null if not delivered
                Console.WriteLine($"Order #{order.OrderNumber} from {order.RestaurantName}: {order.Status}");

                if (order.Status == "Delivered" && order.Deliverer != null)
                {
                    Console.WriteLine($"This order was delivered by {order.Deliverer.Name} (licence plate: {order.Deliverer.Licence})");
                }

                // Combine items by name
                var itemGroups = order.Items
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

                Console.WriteLine(); // Blank line between orders
            }
        }

        public void CustomerRateRestaurant()
        {
            if (GlobalState.LoggedInUser is not Customer user)
            {
                Console.WriteLine("No user is currently logged in.");
                UserMainMenu();
                return;
            }

            // Get delivered orders that haven't been rated
            var eligibleOrders = user.Orders
                .Where(o => o.Status == "Delivered"
                    && o.OrderNumber != null
                    && (user.RatedOrders == null || !user.RatedOrders.ContainsKey(o.OrderNumber)))
                .ToList();

            // Use Menu class for order selection
            Menu rateMenu = new Menu();
            Console.WriteLine("Select a previous order to rate the restaurant it came from:");
            foreach (var order in eligibleOrders)
            {
                rateMenu.AddOption($"Order #{order.OrderNumber} from {order.RestaurantName}");
            }
            rateMenu.AddOption("Return to the previous menu");

            rateMenu.Display();

            int selection = rateMenu.Selection();

            if (selection == eligibleOrders.Count + 1)
            {
                UserMainMenu();
                return;
            }

            var orderToRate = eligibleOrders[selection - 1];
            Console.WriteLine($"You are rating order #{orderToRate.OrderNumber} from {orderToRate.RestaurantName}:");
            // Display items in the order
            var itemGroups = orderToRate.Items
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

            int rating;
            do
            {
                Console.WriteLine("Please enter a rating for this restaurant (1-5, 0 to cancel): ");
            } while (!int.TryParse(Console.ReadLine(), out rating) || rating < 0 || rating > 5);

            if (rating == 0)
            {
                UserMainMenu();
                return;
            }

            Console.WriteLine("Please enter a comment to accompany this rating: ");
            string comment = Console.ReadLine() ?? "";

            user.RatedOrders ??= [];
            if (orderToRate.OrderNumber != null)
            {
                user.RatedOrders[orderToRate.OrderNumber] = true;
            }

            if (GlobalState.LoggedInUser is Customer customer && orderToRate.RestaurantName != null)
            {
                // Find the restaurant (Client) by its name
                var clientRestaurant = Client.Clients.FirstOrDefault(cl => cl.RestName == orderToRate.RestaurantName);
                if (clientRestaurant != null)
                {
                    clientRestaurant.Ratings.Add(rating); // keep for average rating
                    clientRestaurant.Reviews.Add(new Review
                    {
                        ReviewerName = customer.Name ?? "Unknown",
                        Rating = rating,
                        Comment = comment
                    });
                }
            }

            Console.WriteLine($"Thank you for rating {orderToRate.RestaurantName}.");
            UserMainMenu();
        }

        private void RestaurantReviews(Client restaurant)
        {
            if (restaurant.Reviews.Count == 0)
            {
                Console.WriteLine("No reviews have been left for this restaurant.");
            }
            else
            {
                foreach (var review in restaurant.Reviews)
                {
                    Console.WriteLine($"Reviewer: {review.ReviewerName}");
                    Console.WriteLine($"Rating: {new string('*', review.Rating)}");
                    Console.WriteLine($"Comment: {review.Comment}");
                    Console.WriteLine();
                }
            }
        }

    }
}