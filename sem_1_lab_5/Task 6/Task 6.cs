class Program
{
    static void Main()
    {
        Restaurant.Order();
    }
}
public static class Restaurant
{
    private static Cook headcook = null;
    static Restaurant()
    {
        Cook cook1 = new CookFirst("Jack");
        Cook cook2 = new CookSecond("Nick");
        Cook cook3 = new CookMeet("Emily");
        Cook cook4 = new CookSalad("Max");
        Cook cook5 = new CookDessert("Liam");
        Cook cook6 = new CookDrinks("Kate");
        Cook no_cook = new CookUnpreparable();
        cook1.SetSuccessor(cook2);
        cook2.SetSuccessor(cook3);
        cook3.SetSuccessor(cook4);
        cook4.SetSuccessor(cook5);
        cook5.SetSuccessor(cook6);
        cook6.SetSuccessor(no_cook);
        headcook = cook1;
    }
    public static void Order()
    {
        List<string> order = new List<string>();
        Console.WriteLine("Menu:\n - first dish\n - second dish\n - meet\n - salad\n - dessert\n - drinks");
        do
        {
            Console.Write("Choose the meals you want to order (separated by comma): ");
            try { order = Console.ReadLine().Split(',').ToList(); }
            catch (FormatException) { Console.WriteLine("Wrong format..."); }
        } while (order == null);
        for (int i = 0; i < order.Count; i++) order[i] = order[i].StartsWith(' ') ? order[i].Substring(1) : order[i];
        Cook(order);
    }
    private static void Cook(List<string> dish)
    {
        Console.WriteLine("--------------------------------------------------");
        List<string> order = headcook.CookDish(dish);
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("Your order is prepared: ");
        foreach (string meal in order)
            Console.WriteLine(" - {0}", meal);
    }
}
public abstract class Cook
{
    protected Cook _next = null;
    protected string _dish;
    protected string _name;
    public void SetSuccessor(Cook successor) => _next = successor;
    public virtual List<string> CookDish(List<string> order)
    {
        List<string> dish = new List<string>();
        if (order.Contains(_dish))
        {
            Console.WriteLine("Cook {0} is preparing the {1}", _name, _dish);
            for (int i = 0; i < order.FindAll(x => x == _dish).Count; i++)
                dish.Add(_dish);
            order.RemoveAll(x => x == _dish);
        }
        if (order.Count > 0)
            foreach (string meal in _next.CookDish(order))
                dish.Add(meal);
        return dish;
    }
}
public class CookFirst : Cook
{
    public CookFirst(string name)
    {
        _dish = "first dish";
        _name = name;
    }
}
public class CookSecond : Cook
{
    public CookSecond(string name)
    {
        _dish = "second dish";
        _name = name;
    }
}
public class CookMeet : Cook
{
    public CookMeet(string name)
    {
        _dish = "meet";
        _name = name;
    }
}
public class CookSalad : Cook
{
    public CookSalad(string name)
    {
        _dish = "salad";
        _name = name;
    }
}
public class CookDessert : Cook
{
    public CookDessert(string name)
    {
        _dish = "dessert";
        _name = name;
    }
}
public class CookDrinks : Cook
{
    public CookDrinks(string name)
    {
        _dish = "drinks";
        _name = name;
    }
}
public class CookUnpreparable : Cook
{
    public override List<string> CookDish(List<string> order)
    {
        Console.WriteLine("Such dishes cannot be cooked");
        return new List<string>();
    }
}