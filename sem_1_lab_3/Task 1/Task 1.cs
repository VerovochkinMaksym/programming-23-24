class Program
{
    static void Main()
    {
        Product p1 = new Medicines("Paracetamol", "antipyretic", "2025 May", 180.30);
        Product p2 = new Medicines("Citramon", "analgesics", "2028 June", 109.00);
        Product p3 = new ImportedMedicines(new Medicines("Acetylsalicylic acid", "analgesics", "2024 May", 205.45), true);
        Product p4 = new Medicines("Analgin", "analgesics", "2024 December", 23.40);
        Product p5 = new ImportedMedicines(new Medicines("Ambroxol extra", "cough medicine", "2027 April", 456.88), false); 
        List<Product> products = new List<Product> { p1, p2, p3, p4, p5 };
        Pharmacy.Add(products);
        foreach (Product product in products)
            product.CheckOrder(new Random().Next(1, 6));
    }
}
public static class Pharmacy
{
    private static List<Product> _storage = new List<Product>();
    public static Product CheckStorage(string name) => _storage.Find(x => x.Name == name);
    public static List<string> Analogues(string name)
    {
        List<string> a = new List<string>();
        foreach(Product product in _storage.FindAll(x => x.Group == CheckStorage(name).Group && x.Name != name))
            a.Add(product.Name);
        return a;
    }
        
    public static void Add(Product p) => _storage.Add(p);
    public static void Add(List<Product> lp) => _storage = new List<Product>(lp);
}
public abstract class Product
{
    protected string _name;
    protected string _group;
    protected string _period;
    protected double _price;
    public string Name => _name;
    public string Group => _group;
    public string Period => _period;
    public double Price => _price;
    protected virtual void Order(int count)
    {
        Console.WriteLine("You ordered the {0} in the amount of {1} pieces. Amount due: {2}\n",
                           _name, count, Pharmacy.CheckStorage(_name)._price * count);
    }
    public virtual void CheckOrder(int count)
    {
        if (Pharmacy.CheckStorage(_name) == null) { Console.WriteLine("There is no such the product on the storage.\n"); return; }
        OrderInfo();
        Order(count);  
    }
    protected virtual void OrderInfo()
    {
        Console.WriteLine("Name: {0}\nMedicine group: {1}\nPeriod till: {2}\nPrice: {3:0.00}\n", _name, _group, _period, _price);
    }
}
public class Medicines : Product
{
    public Medicines(string name, string group, string period, double price)
    {
        _name = name;
        _group = group;
        _period = period;
        _price = price;
    }
}
public class ImportedMedicines : Product
{
    bool _cert;
    public ImportedMedicines(Medicines medicines, bool cert)
    {
        _name = medicines.Name;
        _group = medicines.Group;
        _period = medicines.Period;
        _price = medicines.Price;
        _cert = cert;
    }
    public override void CheckOrder(int count)
    {
        if (!_cert) { Console.WriteLine("The product is not certified. Cannot be sold."); return; }
        if (Pharmacy.CheckStorage(_name) == null) { Console.WriteLine("There is no such the product on the storage."); return; }
        OrderInfo();
        Order(count);
    }
    protected override void OrderInfo()
    {
        base.OrderInfo();
        Console.WriteLine("The product is certified. Allowed for sale.");
        Console.WriteLine("Domestic analogues of the drug: ");
        List<string> a = Pharmacy.Analogues(_name);
        for (int i = 0; i < a.Count; i++)
            Console.WriteLine(" {4}.\tName: {0}\n\tMedicine group: {1}\n\tPeriod till: {2}\n\tPrice: {3:0.00}\n", Pharmacy.CheckStorage(a[i]).Name, 
                                Pharmacy.CheckStorage(a[i]).Group, Pharmacy.CheckStorage(a[i]).Period, Pharmacy.CheckStorage(a[i]).Price, i + 1);
    }
}