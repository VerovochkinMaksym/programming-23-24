class Program
{
    static void Main()
    {
        IApprehend ap = new Apprehended();
        ap.Apprehend(3);
        ap.Apprehend(5);
        ap.Apprehend(6);
        ap.Apprehend(3);
        ap.Apprehend(3);
        ap.Apprehend(18);
        ap = new Apprehended();
        ap.Apprehend(3);
        ap.Apprehend(3);
        ap.Apprehend(18);
        Console.Write("Enter the offender's name to show its proceedings: ");
        foreach (string proceeding in Registry.ReadProceedings(Console.ReadLine()))
            Console.WriteLine("\n" + proceeding);
    }
}
public static class Registry
{
    private static List<string> proceedings = new List<string>();
    public static string ReadProceeding(string name) => proceedings.Find(x => x.Contains(name));
    public static List<string> ReadProceedings(string name) => proceedings.FindAll(x => x.Contains("Name: " + name));
    public static void AddProceeding(string proceeding) => proceedings.Add(proceeding);
}
public interface IApprehend
{
    string ApprehendInfo(int time);
    void Apprehend(int time)
    {
        Console.WriteLine("\nApprehending information:\n{0}-------------------------------------", ApprehendInfo(time));
    }
}
public class Offender : IApprehend
{
    private string _name;
    private short _age;
    private short _height;
    public Offender()
    {
        Console.WriteLine("New offender was apprehended. Fill its case...");
        Console.Write("Offender's name: ");
        _name = Console.ReadLine();
        Console.Write("Offender's age: ");
        _age = short.Parse(Console.ReadLine());
        Console.Write("Offender's height: ");
        _height = short.Parse(Console.ReadLine());
    }
    public string ApprehendInfo(int time)
    {
        return String.Format("Name: {0}\nAge: {1}\nHeight {2}\nDuration of apprehending: {3}\n", _name, _age, _height, time);
    }
}
public class Apprehended : IApprehend
{
    const int minimum_time = 3;
    private string _date;
    private string _reason;
    private Offender offender;
    public string ApprehendInfo(int time)
    {
        if (offender == null)
            offender = new Offender();
        return time > minimum_time ? ProceedingInfo(time) : offender.ApprehendInfo(time);
    }
    private string ProceedingInfo(int time)
    {
        Info();
        string proceeding = offender.ApprehendInfo(time) + 
                            String.Format("Date of apprehending: {0}\nReason of apprehending: {1}\n", _date, _reason);
        Registry.AddProceeding(proceeding);
        return proceeding;
    }
    private void Info()
    {
        Console.WriteLine("New apprehending of the offender. Fill apprehendings details...");
        Console.Write("Date of apprehending: ");
        _date = Console.ReadLine();
        Console.Write("Reason of apprehending: ");
        _reason = Console.ReadLine();
    }
}