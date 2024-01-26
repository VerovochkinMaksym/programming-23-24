
class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the count of workers");
        int n = int.Parse(Console.ReadLine());
        List<Worker> workers = new List<Worker>();
        Random R = new Random();
        for(int i = 0; i < n; i++)
            Company.AddWorker(new Worker(R.NextDouble() * 4000, (R.NextDouble() + 1) * 6 ));
        for (int i = 0; i < 3; i++)
            Company.ShowQuality();
    }
}
public interface IQuality
{
    void Calculate();
}
public class QualityProfit : IQuality
{
    public void Calculate()
    {
        Company.SetProfit();
        Console.WriteLine("Quality by the paying off the company's outcome is {0:0.00}%", Company.Income / Company.Outcome * 100);
    }
}
public class QualityExperience : IQuality
{
    public void Calculate()
    {
        Console.WriteLine("Quality by the middle work experiance level  of the workers in the company is {0:0.00}", 
                                                   Company.Workers.Sum(x => x.Experience) / Company.Workers.Count);
    }
}
public class QualityEducation : IQuality
{
    public void Calculate()
    {
        Console.WriteLine("Quality by the middle education level of the workers in the company is {0:0.00}",
                                             Company.Workers.Sum(x => x.Education) / Company.Workers.Count);
    }
}
public static class Company
{
    private static Dictionary<string, IQuality> qualities = new Dictionary<string, IQuality>();
    private static List<Worker> workers = new List<Worker>();
    private static double income = 1;
    private static double outcome = 1;
    public static List<Worker> Workers => workers;
    public static double Income => income;
    public static double Outcome => outcome;
    static Company()
    {
        qualities.Add("Quality by the paying off the company's outcome", new QualityProfit());
        qualities.Add("Quality by the middle work experiance level  of the workers in the company", new QualityExperience());
        qualities.Add("Quality by the middle education level of the workers in the company", new QualityEducation());
    }
    public static void SetProfit()
    {
        double income = -1;
        do
        {
            Console.Write("Enter the company income: ");
            try { income = double.Parse(Console.ReadLine()); }
            catch (FormatException) { Console.WriteLine("Wrong format..."); }
        } while (income < 0);
        double outcome = -1;
        do
        {
            Console.Write("Enter the company outcome: ");
            try { outcome = double.Parse(Console.ReadLine()); }
            catch (FormatException) { Console.WriteLine("Wrong format..."); }
        } while (outcome < 0);
        Company.income = income;
        Company.outcome = outcome;
    }
    public static void AddWorker(Worker worker) => workers.Add(worker);
    public static void AddWorker(List<Worker> workers) => Company.workers = workers;
    public static void ShowQuality()
    {
        int quality = 0;
        Console.WriteLine("Calculate the company quality by:");
        for (int i = 0; i < qualities.Count; i++)
            Console.WriteLine("{0}) {1}", i + 1, qualities.Keys.ElementAt(i));
        do
        {
            try { quality = int.Parse(Console.ReadLine()); }
            catch (FormatException) { Console.WriteLine("Wrong format..."); }
        } while (quality < 1 || quality > qualities.Count);
        try
        { qualities[qualities.Keys.ElementAt(quality - 1)].Calculate(); }
        catch (NullReferenceException) { Console.WriteLine("Workers list is null"); }
        catch (Exception) { Console.WriteLine("Data error"); }
    }
}
public class Worker
{
    private double _experience;
    private double _education;
    public double Experience => _experience;
    public double Education => _education;
    public Worker(double experience, double education)
    {
        _experience = experience;
        _education = education;
    }
}