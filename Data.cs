namespace ArribaEats
{
    public static class GlobalState
    {
        public static User? LoggedInUser { get; set; } //Global variable to store the logged-in user so that it can be accessed from anywhere in the program

    }
    public abstract class User
    {
        public string? Password { get; set; } //password property
        public string? Name { get; set; } //name property
        public int? Age { get; set; } //age property
        public string? Email { get; set; } //email property
        public string? Phone { get; set; } //phone property

        public virtual void DisplayUserInfo() //method to display user info
        {
            Console.WriteLine();
            Console.WriteLine("Your user details are as follows:");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Age: {Age}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Mobile: {Phone}");
        }
    }

    public class Customer : User //customer user class 
    {
        public int[]? Location { get; set; }
        public List<Order> Orders { get; set; } = [];
        public static List<Customer> Customers = [];
        public Dictionary<string, bool>? RatedOrders { get; set; } = [];

        public override void DisplayUserInfo() //customer variation of the user info display method
        {
            base.DisplayUserInfo(); //extend the base class method to include more information specific to customer
            string locationString = Location != null ? string.Join(",", Location) : "N/A";
            int orderCount = Orders?.Count ?? 0;
            decimal totalSpent = 0.00m;
            if (Orders != null)
            {
                foreach (var order in Orders) //iterate through orders and caculate total spent
                {
                    if (order.Items != null) //if there are items to calculate
                    {
                        foreach (var (item, quantity) in order.Items)
                        {
                            if (item.ItemPrice.HasValue) 
                                totalSpent += (decimal)item.ItemPrice.Value * quantity; 
                        }
                    }
                }
            }
            Console.WriteLine($"Location: {locationString}");
            Console.WriteLine($"You've made {orderCount} order(s) and spent a total of ${totalSpent:0.00} here."); //default amount is $0 if there is nothing calculated to avoid null errors
            Console.WriteLine();
        }
    }

    public class Deliverer : User //deliverer user class
    {
        public string? Licence { get; set; } 
        public static List<Deliverer> Deliverers = [];

        public override void DisplayUserInfo() //deliverer variant of the user info dipslay method
        {
            base.DisplayUserInfo();
            Console.WriteLine($"Licence plate: {Licence}"); 
            Console.WriteLine(); //think this was needed for formatting reasons.
        }
    }

    public class Client : User //client user class
    {
        public string? RestName { get; set; } //restaurant name
        public string? FoodStyle { get; set; }
        public int[]? Location { get; set; }
        public static List<Client> Clients = [];

        public List<Order> Orders { get; set; } = [];

        public List<MenuItem> MenuItems { get; set; } = [];

        // Store all ratings given to this restaurant
        public List<float> Ratings { get; set; } = [];

        // Calculate average rating; return null if there are no ratings.
        public double? AverageRating => Ratings.Count != 0 ? Math.Round(Ratings.Average(), 1) : null; //public variable so that the average of any restaurant can easily be called without having to recalc

        public List<Review> Reviews { get; set; } = [];

        public override void DisplayUserInfo() //restaurant/client user info (ig this app doesn't have to be specific to restaurants and it could theoretically be used for retail items too... it's uber delivery)
        {
            base.DisplayUserInfo();
            string locationString = Location != null ? string.Join(",", Location) : "N/A";
            Console.WriteLine($"Restaurant name: {RestName}");
            Console.WriteLine($"Restaurant style: {FoodStyle}");
            Console.WriteLine($"Restaurant location: {locationString}");
            Console.WriteLine();
        }
    }

    public class MenuItem
    {
        public string? ItemName { get; set; }
        public float? ItemPrice { get; set; }
        public List<MenuItem> Menu { get; set; } = []; //list of all menu items initialized to empty
    }

    public class Review //review class, technically users aren't the only things that could use this
    {
        public string ReviewerName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
    }


}