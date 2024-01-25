class Program
{
    static void Main()
    {
        Console.Write("Enter the number of products: ");
        int n = int.Parse(Console.ReadLine());
        List<Product> products = new List<Product>();
        Director director = new Director();
        for (int i = 0; i < n; i++)
            products.Add(director.Construct());
        foreach(Product product in products)
            product.GetInfo();
    }
}
public abstract class Builder
{
    public virtual void Carcass(Product product) { }
    public virtual void Facade(Product product) { }
    public virtual void Furniture(Product product) { }
}
public class CarcassNatural : Builder
{
    public override void Carcass(Product product) => product.AddPart("natural wood carcass");
}
public class CarcassChipboard : Builder
{
    public override void Carcass(Product product) => product.AddPart("chipboard carcass");
}
public class FacadeNatural : Builder
{
    public override void Facade(Product product) => product.AddPart("natural wood facade");
}
public class FacadeChipboard : Builder
{
    public override void Facade(Product product) => product.AddPart("chipboard facade");
}
public class FacadeMFDPainted : Builder
{
    public override void Facade(Product product) => product.AddPart("painted MDF facade");
}
public class FacadeMDFFilm : Builder
{
    public override void Facade(Product product) => product.AddPart("film MDF facade");
}
public class FurniturePlastic : Builder
{
    public override void Furniture(Product product) => product.AddPart("plastic furniture");
}
public class FurnitureMetal : Builder
{
    public override void Furniture(Product product) => product.AddPart("metal furniture");
}
public class FurnitureWood : Builder
{
    public override void Furniture(Product product) => product.AddPart("wood furniture");
}
public class Product
{
    private List<string> _parts = new List<string>();
    public void AddPart(string part) => _parts.Add(part);
    public void GetInfo()
    {
        Console.WriteLine("Product parts:");
        foreach (string part in _parts)
            Console.WriteLine(" - " + part);
    }
}
public class Director
{
    Builder? builder = null;
    static Dictionary<string, Builder> carcass = new Dictionary<string, Builder>();
    static Dictionary<string, Builder> facade = new Dictionary<string, Builder>();
    static Dictionary<string, Builder> furniture = new Dictionary<string, Builder>();
    static Dictionary<Dictionary<string, Builder>, string> builders = new Dictionary<Dictionary<string, Builder>, string>();
    static Director()
    {
        carcass.Add("Natural wood", new CarcassNatural());
        carcass.Add("Chipboard", new CarcassChipboard());
        facade.Add("Natural wood", new FacadeNatural());
        facade.Add("Chipboard", new FacadeChipboard());
        facade.Add("MFD painted", new FacadeMFDPainted());
        facade.Add("MDF film", new FacadeMDFFilm());
        furniture.Add("Plastic", new FurniturePlastic());
        furniture.Add("Metal", new FurnitureMetal());
        furniture.Add("Wood", new FurnitureWood());
        builders.Add(carcass, "carcass");
        builders.Add(facade, "facade");
        builders.Add(furniture, "furniture");
    }
    public Product Construct()
    {
        Product product = new Product();
        SetBuilder(carcass);
        builder.Carcass(product);
        SetBuilder(facade);
        builder.Facade(product);
        SetBuilder(furniture);
        builder.Furniture(product);
        return product;
    }
    private void SetBuilder(Dictionary<string, Builder> builders)
    {
        int select_builder = 0;
        Console.WriteLine("Choose the material of {0}:", Director.builders[builders]);
        for (int i = 0; i < builders.Count; i++)
            Console.WriteLine("{0}) {1}", i + 1, builders.Keys.ElementAt(i));
        do
        {
            try { select_builder = int.Parse(Console.ReadLine()); }
            catch (FormatException) { Console.WriteLine("Wrong format..."); }
        } while(select_builder < 1 || select_builder > builders.Count);
        builder = builders[builders.Keys.ElementAt(select_builder - 1)];
    }
}