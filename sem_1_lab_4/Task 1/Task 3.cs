class Program
{
    static void Main()
    {
        Firm firm = new FirmState();
        foreach (ITicket ticket in firm.Print("a", "b", 3))
            Console.WriteLine("------------------------------\n" + ticket.GetInfo());
        Console.WriteLine("====================================================================");
        firm = new FirmTicketStand();
        foreach (ITicket ticket in firm.Print("a", "b", 5))
            Console.WriteLine("------------------------------\n" + ticket.GetInfo());
        Console.WriteLine("====================================================================");
        firm = new FirmTicketSpecial();
        foreach (ITicket ticket in firm.Print("a", "b", 2))
            Console.WriteLine("------------------------------\n" + ticket.GetInfo());
    }
}
public interface ITicket
{
    string GetInfo();
}
public abstract class AbstractTicket : ITicket
{
    protected string _event;
    protected string _place;
    protected string Ticket;
    public string GetInfo()
    {
        return Ticket;
    }
}
public class TicketStand : AbstractTicket
{
    private const string pattern = "This ticket is the basis for admission to the concert\nEvent: {0}\nPlace: {1}";
    public TicketStand(string e, string p)
    {
        _event = e;
        _place = p;
        Ticket = String.Format(pattern, _event, _place);
    }
}
public class TicketSeat : AbstractTicket
{
    private const string pattern = "This ticket is the basis for admission to the concert\nEvent: {0}\nPlace: {1}\nSeat: {2}";
    private int _seat;
    public TicketSeat(string e, string p, int s)
    {
        _event = e;
        _place = p;
        _seat = s;
        Ticket = String.Format(pattern, _event, _place, _seat);
    }
}
public class TicketSpecial : AbstractTicket
{
    private const string pattern = "This ticket is the basis for admission to the concert\nEvent: {0}\nPlace: {1}\nSeat: {2}";
    private const string invitation = "Dear Guests,\n\nJoin us for an exclusive private concert. \n" +
                                      "Experience an unforgettable evening of music and entertainment with us.";
    private int _seat;
    public TicketSpecial(string e, string p, int s)
    {
        _event = e;
        _place = p;
        _seat = s;
        Ticket = invitation + "\n\n" + String.Format(pattern, _event, _place, _seat);
    }
}
public abstract class Firm
{
    protected double Price;
    public abstract List<ITicket> Print(string e, string place, int count);
    protected void Receipt(int count) => Console.WriteLine("Your price for {0} tickets is {1}", count, Price * count);
}

public class FirmState : Firm
{
    private const double price = 250.00;
    public FirmState()
    {
        Price = price;
    }
    public override List<ITicket> Print(string event_, string place, int count)
    {
        List<ITicket> tickets = new List<ITicket>();
        for (int i = 1; i <= count; i++)
            tickets.Add(new TicketSeat(event_, place, i));
        Receipt(count);
        return tickets;
    }
}
public class FirmTicketStand : Firm
{
    private const double price = 200.00;
    public FirmTicketStand()
    {
        Price = price;
    }
    public override List<ITicket> Print(string event_, string place, int count)
    {
        List<ITicket> tickets = new List<ITicket>();
        for (int i = 1; i <= count; i++)
            tickets.Add(new TicketStand(event_, place));
        Receipt(count);
        return tickets;
    }
}
public class FirmTicketSpecial : Firm
{
    private const double price = 420.00;
    public FirmTicketSpecial()
    {
        Price = price;
    }
    public override List<ITicket> Print(string event_, string place, int count)
    {
        List<ITicket> tickets = new List<ITicket>();
        for (int i = 1; i <= count; i++)
            tickets.Add(new TicketSpecial(event_, place, i));
        Receipt(count);
        return tickets;
    }
}