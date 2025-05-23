namespace ArribaEats
{
    public class Order
    {
        public string? OrderNumber { get; set; }
        public string? RestaurantName { get; set; }
        public string? Status { get; set; } // "Ordered", "Cooking", "Cooked", "Being Delivered", "Delivered"
        public Deliverer? Deliverer { get; set; }
        public List<(MenuItem Item, int Quantity)> Items { get; set; } = []; //list of all items in the order initialized to empty

        public bool DelivererArrivedAtRestaurant { get; set; } = false;

        private static int NextOrderNumber = 1; // Static counter for unique order numbers

        public static void PlaceOrder(Customer customer, Client restaurant)
        {
            var orderItems = new List<(MenuItem, int)>();
            float total = 0f;

            while (true)
            {
                Console.WriteLine($"\nCurrent order total: ${total:0.00}");

                // Use Menu class for item selection
                Menu orderMenu = new Menu();
                foreach (var item in restaurant.MenuItems)
                {
                    orderMenu.AddOption($"{("$" + item.ItemPrice.GetValueOrDefault().ToString("0.00")).PadLeft(7)}  {item.ItemName}");
                }
                orderMenu.AddOption("Complete order");
                orderMenu.AddOption("Cancel order");

                orderMenu.Display();
                int selection = orderMenu.Selection();

                int completeOption = restaurant.MenuItems.Count + 1;
                int cancelOption = restaurant.MenuItems.Count + 2;

                if (selection == completeOption)
                {
                    if (orderItems.Count == 0)
                    {
                        Console.WriteLine("You must add at least one item to complete the order.");
                        continue;
                    }
                    // Place the order
                    var order = new Order
                    {
                        OrderNumber = NextOrderNumber.ToString(),
                        RestaurantName = restaurant.RestName,
                        Status = "Ordered",
                        Items = orderItems.Select(x => (x.Item1, x.Item2)).ToList()
                    };
                    customer.Orders.Add(order);
                    NextOrderNumber++;
                    Console.WriteLine($"\nYour order has been placed. Your order number is #{order.OrderNumber}.");
                    return;
                }
                else if (selection == cancelOption)
                {
                    // Cancel order
                    return;
                }
                else
                {
                    int itemIndex = selection - 1;
                    var menuItem = restaurant.MenuItems[itemIndex];
                    Console.WriteLine($"Adding {menuItem.ItemName} to order.");
                    int quantity = -1;
                    while (quantity < 0)
                    {
                        Console.WriteLine("Please enter quantity (0 to cancel): ");
                        string? qtyInput = Console.ReadLine();
                        if (!int.TryParse(qtyInput, out quantity) || quantity < 0)
                        {
                            Console.WriteLine("Invalid quantity.");
                            quantity = -1;
                        }
                        else if (quantity == 0)
                        {
                            break; // Cancel adding this item
                        }
                        else
                        {
                            orderItems.Add((menuItem, quantity));
                            total += menuItem.ItemPrice.GetValueOrDefault() * quantity;
                            Console.WriteLine($"Added {quantity} x {menuItem.ItemName} to order.");
                        }
                    }
                }
            }
        }



    }
}