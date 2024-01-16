class Program
{
    static bool technical = false;                          // constructors flag
    interface IPlayer<T>
    {
        static abstract T CreatePlayer();
    }
    abstract class Player : IPlayer<Amateur>, IPlayer<Footballer>
    {
        private static List<string> positions = new List<string>() { "gk", "df", "mf", "at" };
        private static List<Player> team = new List<Player>();
        private static string team_name;
        protected static int count => team.Count;
        protected static int[] tactics;
        static Player()
        {
            if (technical)
            {
                Console.WriteLine(" >> Player static constructor. Press any key to continue...");
                Console.ReadKey();
            }
            Start(true);
        }
        private static void Start(bool new_coach)
        {
            Console.Clear();
            team = team.FindAll(x => x.GetSpeciality() == "coach" && new_coach == false);
            if (new_coach)
                Coach.CreateCoach();
            team_name = null;
            do
            {
                Console.Write("Enter a team name: ");
                try { team_name = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (team_name == null);
            tactics = new int[3];
            do
            {
                Console.Write("Enter a tactic deployment (capacities of 10 positions df-mf-at), separated by '-': ");
                try { tactics = Console.ReadLine().Split('-').Select(int.Parse).ToArray(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            while (tactics.Sum() != 10);
        }
        private string? _name = null;
        private int _number;
        protected int _power;
        private string? _position = null;
        /*
        public Player()
        {
            if (technical)
                Console.WriteLine("\n >> Player default constructor\n");
            Add();
        }*/
        protected Player(int power, string position)
        {
            if (technical)
                Console.WriteLine("\n >> Player constructor\n");
            do
            {
                Console.Write("Enter a {0}'s name: ", GetSpeciality());
                try { _name = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (_name == null);
            int number = -1;
            do
            {
                Console.Write("Enter a {0}'s number: ", GetSpeciality());
                try { number = int.Parse(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!(number >= 1 && number <= 99 && !team.Exists(x => x._number == number)));
            _number = number;
            _power = power;
            if (position != null && Count(position) < (position == "gk" ? 1 : tactics[positions.IndexOf(position) - 1]))
                _position = position;
            Add();
        }
        static Amateur IPlayer<Amateur>.CreatePlayer()
        {
             if (count >= 16)
                 return null;
             string position = null;
             do
             {
                 Console.Write("Enter a player's position (gk, df, mf or at): ");
                 try { position = Console.ReadLine(); }
                 catch (FormatException) { Console.WriteLine("Format error..."); }
             }
             while (!positions.Contains(position));
             return Amateur.CreateAmateur(position);
        }

        static Footballer IPlayer<Footballer>.CreatePlayer()
        {
            if (count >= 16)
                return null;
            string position = null;
            do
            {
                Console.Write("Enter a player's position (gk, df, mf or at): ");
                try { position = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!positions.Contains(position));
            return Footballer.CreateFootballer(position);
        }
        private void Add()
        {
            team.Add(this);
            team.Last()._power = PowerComputing(_position);
        }
        protected static int Count(string position) => team.Count(x => x._position == position);
        protected string GetSpeciality() => GetType().Name.ToLower();
        protected abstract int PowerComputing(string position);
        public static void Switch()
        {
            string position = null;
            do
            {
                Console.Write("Enter a position to switch (gk, df, mf or at): ");
                try { position = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!positions.Contains(position));
            foreach (Player person in team.FindAll(x => x._position == position))
                Console.WriteLine("{0} - {1}", person._name, person._power * 100);
            string name1 = null;
            do
            {
                Console.Write("Enter the first player to switch: ");
                try { name1 = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!team.FindAll(x => x._position == position).Exists(x => x._name == name1));
            foreach (Player person in team.SkipWhile(x => x._name == name1))
                Console.WriteLine("{0} - {1}", person._name, person._power * 100);
            string name2 = null;
            do
            {
                Console.Write("Enter the second player to switch: ");
                try { name2 = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!team.SkipWhile(x => x._name == name1).ToList().Exists(x => x._name == name2));
            team[team.FindIndex(x => x._name == name1)]._position = team[team.FindIndex(x => x._name == name2)]._position;
            team[team.FindIndex(x => x._name == name2)]._position = position;
            foreach (Player player in team)
                team[team.IndexOf(player)]._power = player.PowerComputing(player._position);
        }
        public static void WriteOut()
        {
            Console.WriteLine("Team: {0}", team_name);
            Console.WriteLine("Tactics: {0}-{1}-{2}", tactics[0], tactics[1], tactics[2]);
            Console.WriteLine("Team composition: ");
            foreach (Player player in team.FindAll(x => x._position != null))
                Console.WriteLine("({0}) {1} [{2}] - {3} - {4}", player._number, player._name, player._position,
                                                                 player._power * 100, player.GetSpeciality());
            Console.WriteLine("Reserve players: ");
            foreach (Player player in team.FindAll(x => x._position == null))
                Console.WriteLine("({0}) {1} - {2} - {3}", player._number, player._name, player._power * 100, player.GetSpeciality());
            Console.WriteLine("Team power: " + team.FindAll(x => x._position != null).Sum(x => x._power * 100));
        }
        public static void CreateTeam()
        {
            foreach (Player player in team)
                if (player.GetSpeciality() != "coach")
                    player.Dispose();
            Start(false);
        }
        public static void Restart()
        {
            foreach (Player player in team)
                player.Dispose();
            Start(true);
        }
        public void Dispose()
        {
            Console.WriteLine("Disposing...");
            GC.SuppressFinalize(this);
        }
        ~Player() { if (technical) Console.WriteLine("\n >> Player destructor\n"); }
    }
    class Coach : Player
    {
        private static Coach coach = null;
        private Coach() : base(new Random().Next(0, 2), null)
        {
            if (technical)
                Console.WriteLine("\n >> Coach constructor\n");
        }
        public static Coach CreateCoach()
        {
            if (coach == null)
                return new Coach();
            return null;
        }
        protected override int PowerComputing(string position) => _power;
        ~Coach() { if (technical) Console.WriteLine("\n >> Coach destructor\n"); }
    }
    class Amateur : Player
    {
        protected Amateur(string position) : base(1, position)
        {
            if (technical)
                Console.WriteLine("\n >> Amateur constructor\n");
        }
        public static Amateur CreateAmateur(string position) => new(position);
        protected override int PowerComputing(string position)
        {
            Random R = new Random();
            return _power = R.Next(4, 13) / 4;
        }
}
    class Footballer : Player
    {
        protected Footballer(string position) : base(3, position)
        {
            if (technical)
                Console.WriteLine("\n >> Footballer constructor\n");
        }
        public static Footballer CreateFootballer(string position)
        {
            return null;
        }
        protected override int PowerComputing(string position)
        {
            Random R = new Random();
            return _power = R.Next(4, 13) / 4;
        }
        ~Footballer() { if (technical) Console.WriteLine("\n >> Footballer destructor\n"); }
    }
    class Goalkeeper : Footballer
    {
        protected static int count { get { return Count("gk"); } set { } }
        private Goalkeeper() : base("gk")
        {
            if (technical)
                Console.WriteLine("\n >> Goalkeeper constructor\n");
        }
        public static Goalkeeper CreateGoalkeeper()
        {
            if (Player.count >= 16 || count >= 1)
                return null;
            count++;
            return new Goalkeeper();
        }
        protected override int PowerComputing(string position)
        {
            if (position == "gk")
                return _power = 3;
            else
            {
                Random R = new Random();
                return _power = R.Next(2, 4);
            }
        }
        ~Goalkeeper() { if (technical) Console.WriteLine("\n >> Goalkeeper destructor\n"); }
    }
    class Defender : Footballer
    {
        static int count { get { return Count("df"); } set { } }
        private Defender() : base("df")
        {
            if (technical)
                Console.WriteLine("\n >> Defender constructor\n");
        }
        public static Defender CreateDefender()
        {
            if (Player.count >= 16 || count >= tactics[0])
                return null;
            count++;
            return new Defender();
        }
        protected override int PowerComputing(string position)
        {
            if (position == "df")
                return _power = 3;
            else
            {
                Random R = new Random();
                return _power = R.Next(2, 4);
            }
        }
        ~Defender() { if (technical) Console.WriteLine("\n >> Defender destructor\n"); }
    }
    class Midfielder : Footballer
    {
        static int count { get { return Count("mf"); } set { } }
        private Midfielder() : base("mf")
        {
            if (technical)
                Console.WriteLine("\n >> Midfielder constructor\n");
        }
        public static Midfielder CreateMidfielder()
        {
            if (Player.count >= 16 || count >= tactics[1])
                return null;
            count++;
            return new Midfielder();
        }
        protected override int PowerComputing(string position)
        {
            if (position == "mf")
                return _power = 3;
            else
            {
                Random R = new Random();
                return _power = R.Next(2, 4);
            }
        }
        ~Midfielder() { if (technical) Console.WriteLine("\n >> Midfielder destructor\n"); }
    }
    class Attacker : Footballer
    {
        static private int count { get { return Count("at"); } set { } }
        private Attacker() : base("at")
        {
            if (technical)
                Console.WriteLine("\n >> Attacker constructor\n");
        }
        public static Attacker CreateAttacker()
        {
            if (Player.count >= 16 || count >= tactics[2])
                return null;
            count++;
            return new Attacker();
        }
        protected override int PowerComputing(string position)
        {
            if (position == "at")
                return _power = 3;
            else
            {
                Random R = new Random();
                return _power = R.Next(2, 4);
            }
        }
        ~Attacker() { if (technical) Console.WriteLine("\n >> Attacker destructor\n"); }
    }
    /*
static void GarbageCollector()
{
   for (int i = 0; i < 10000; i++)
   {
       Player p = new Player();
       Console.WriteLine("Generation: {0}", GC.GetGeneration(p));
       if (GC.GetTotalMemory(false) < 3000000)
           Console.WriteLine("Heap size: {0}", GC.GetTotalMemory(false));
   }
   GC.Collect();
   GC.WaitForPendingFinalizers();
   Console.WriteLine("Times the garbage was collected: {0}\nHeap size: {1}\nPress any key to continue...", GC.CollectionCount(0), GC.GetTotalMemory(false));
   Console.ReadKey();
   for (int i = 0; i < 5000; i++)
   {
       Player p = new Player();
       Console.WriteLine("Generation: {0}", GC.GetGeneration(p));
       if (GC.GetTotalMemory(true) < 1200000)
           Console.WriteLine("Heap size: {0}", GC.GetTotalMemory(false));
   }
   GC.Collect();
   GC.WaitForPendingFinalizers();
   Console.WriteLine("Times the garbage was collected: {0}\nHeap size: {1}\nPress any key to continue...", GC.CollectionCount(0), GC.GetTotalMemory(false));
   Console.ReadKey();
   Player.Restart();
}*/
    static void Main()
    {
        Console.WriteLine("amateur - add a new non-professional player to the team\n" +
                          "footballer - add a new professional player to the team\n" +
                          "switch - switch the player at specific position\n" +
                          "write - show the current team information\n" +
                          "team - create a new team with current coach\n" +
                          "restart - full restart\n" +
                          "exit - exit application\n" +
                          /*"gc - test garbage collector\n" +*/
                          "technical - show / hide technical inscriptions\n");
        while (true)
        {
            Console.Write("Enter the command: ");
            switch (Console.ReadLine().ToLower())
            {
                case "amateur":
                    try {  }
                    catch (NullReferenceException) { Console.WriteLine("You are out of the limit"); }
                    break;
                case "footballer":
                    try {  }
                    catch (NullReferenceException) { Console.WriteLine("You are out of the limit"); }
                    break;
                case "switch": Player.Switch(); break;
                case "write": Player.WriteOut(); break;
                case "team": Player.CreateTeam(); break;
                case "restart": Player.Restart(); break;
                case "exit": Environment.Exit(0); break;
                /*case "gc": GarbageCollector(); break;*/
                case "technical": technical = !technical; break;
                default: Console.WriteLine("The command is not found."); break;
            }
            Console.WriteLine();
        }
    }
}