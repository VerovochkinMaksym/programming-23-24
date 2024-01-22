public class Program
{
    static bool technical = true;                          // constructors flag
    static bool help = true;
    static short random = -1;
    static CustomTeam ct = null;
    static RandomTeam rt = null;
    delegate int RecomputeHandler(string p);
    static List<string> positions = new List<string>() { "goalkeeper", "defender", "midfielder", "attacker" };
    static string GetSpeciality(object player) => player.GetType().Name.ToLower();
    interface IPlayer<T> where T : Player
    {
        static Player CreatePlayer()
        {
            if ((random < 0 ? ct.TeamCount : rt.TeamCount) >= 16)
                throw new OutOfLimitException(new OutOfLimitExceptionArgs(typeof(T).Name));
            string? position = null;
            do
            {
                Console.Write("Enter a player's position (goalkeeper, defender, midfielder or attaker): ");
                try { position = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!positions.Contains(position));
            Func<string, Player> Create = (p) => typeof(T) == typeof(Amateur) ? Amateur.CreateAmateur(p) : Professional.CreateProfessional(p);
            return Create(position);
        }
    }
    abstract class Team
    {
        protected string? _name;
        protected List<Player> _team = new List<Player>();
        public int TeamCount => _team.Count;
        public int CountProfessional(string position) => _team.Count(x => GetSpeciality(x) == position);
        public int Count(string position) => _team.Count(x => x.info.position == position);
        public static string Winner => ct._team.FindAll(x => x.info.position != null).Sum(x => x.info.power * 100) >=
                rt._team.FindAll(x => x.info.position != null).Sum(x => x.info.power * 100) ? ct._name : rt._name;
        protected int[]? _tactics;
        public abstract void Start(bool new_coach);
        public void Add(Player p)
        {
            Refresh();
            _team.Add(p);
            Refresh();
            _team.Last().SetPower(_team.Last().Recompute(_team.Last().info.position));
        }
        public void Refresh()
        {
            Player.TacticsByPosition = (p) => _tactics[positions.IndexOf(p) - 1];
            Player.CountCheck = (p) => CountProfessional(p) >= Player.TacticsByPosition.Invoke(p);
            Player.ContainsPlayer = (n) => _team.Exists(x => x.info.number == n);
        }
        public void CreateTeam()
        {
            foreach (Player player in _team)
                if (GetSpeciality(player) != "coach")
                    player.Dispose();
            Start(false);
        }
        public void Switch()
        {
            string? position = null;
            do
            {
                Console.Write("Enter a position to switch (goalkeeper, defender, midfielder or attacker): ");
                try { position = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!positions.Contains(position));
            List<Player> l = _team.FindAll(x => x.info.position == position);
            if (l.Count == 0)
            {
                Console.WriteLine("There are no players at the positions of this type...");
                return;
            }
            foreach (Player player in l)
                Console.WriteLine("({0}) {1} - {2} - {3}", player.info.number, player.info.name, player.info.power * 100, GetSpeciality(player));
            int? number1 = null;
            do
            {
                Console.Write("Enter the number of the first player to switch: ");
                try { number1 = int.Parse(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!_team.FindAll(x => x.info.position == position).Exists(x => x.info.number == number1));
            foreach (Player player in _team.FindAll(x => x.info.position != null && x.info.number != number1))
                Console.WriteLine("({0}) {1} [{2}] - {3} - {4}", player.info.number, player.info.name, player.info.position,
                                                                 player.info.power * 100, GetSpeciality(player));
            foreach (Player player in _team.FindAll(x => x.info.position == null && x.info.number != number1))
                Console.WriteLine("({0}) {1} - {2} - {3}", player.info.number, player.info.name, player.info.power * 100, GetSpeciality(player));
            int? number2 = null;
            do
            {
                Console.Write("Enter the number of the second player to switch: ");
                try { number2 = int.Parse(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (!_team.FindAll(x => x.info.number != number1).ToList().Exists(x => x.info.number == number2));
            _team[_team.FindIndex(x => x.info.number == number1)].info.position = _team[_team.FindIndex(x => x.info.number == number2)].info.position;
            _team[_team.FindIndex(x => x.info.number == number2)].info.position = position;
            foreach (Player player in _team)
                _team[_team.IndexOf(player)].info.power = player.Recompute(player.info.position);
        }
        public void WriteOut()
        {
            Console.WriteLine("Team: {0}", _name);
            Console.WriteLine("Tactics: {0}-{1}-{2}", _tactics[0], _tactics[1], _tactics[2]);
            Console.WriteLine("Team composition: ");
            foreach (Player player in _team.FindAll(x => x.info.position != null))
                Console.WriteLine("({0}) {1} [{2}] - {3} - {4}", player.info.number, player.info.name, player.info.position,
                                                                 player.info.power * 100, GetSpeciality(player));
            Console.WriteLine("Reserve players: ");
            foreach (Player player in _team.FindAll(x => x.info.position == null))
                Console.WriteLine("({0}) {1} - {2} - {3}", player.info.number, player.info.name, player.info.power * 100, GetSpeciality(player));
            Console.WriteLine("Team power: " + _team.FindAll(x => x.info.position != null).Sum(x => x.info.power * 100));
        }
        public void Restart()
        {
            foreach (Player player in _team)
                player.Dispose();
            Start(true);
        }
    }
    class CustomTeam : Team
    {
        static CustomTeam()
        {
            if (technical)
            {
                Console.WriteLine("\n >> CustomTeam static constructor. Press any key to continue...\n");
                Console.ReadKey();
            }
        }
        public override void Start(bool new_coach)
        {
            Console.Clear();
            _team = _team.FindAll(x => GetSpeciality(x) == "coach" && new_coach == false);
            Refresh();
            if (new_coach)
                Coach.CreateCoach();
            _name = null;
            do
            {
                Console.Write("Enter a team name: ");
                try { _name = Console.ReadLine(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
            }
            while (_name == null);
            _tactics = new int[3];
            do
            {
                Console.Write("Enter a tactic deployment (capacities of 10 positions df-mf-at), separated by '-': ");
                try { _tactics = Console.ReadLine().Split('-').Select(int.Parse).ToArray(); }
                catch (FormatException) { Console.WriteLine("Format error..."); }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            while (_tactics.Sum() != 10);
        }
    }
    class RandomTeam : Team
    {
        static RandomTeam()
        {
            if (technical)
            {
                Console.WriteLine("\n >> RandomTeam static constructor. Press any key to continue...\n");
                Console.ReadKey();
            }
        }
        public override void Start(bool new_coach)
        {
            Console.Clear();
            _name = "Enemy team";
            _tactics = new int[3];
            Random tactics_random = new Random();
            _tactics[0] = tactics_random.Next(1, 6);
            _tactics[1] = tactics_random.Next(Math.Max(5 - _tactics[0], 1), Math.Min(9 - _tactics[0], 5));
            _tactics[2] = 10 - _tactics.Sum();
            if (new_coach)
                random = 0;
            rt.Add(Coach.CreateCoach());
            for (random = 1; random <= 4; random++)
                rt.Add(Amateur.CreateAmateur(null));
            for (int i = random; random < i + 1; random++)
                rt.Add(Goalkeeper.CreateGoalkeeper());
            for (int i = random; random < i + _tactics[0]; random++)
                rt.Add(Defender.CreateDefender());
            for (int i = random; random < i + _tactics[1]; random++)
                rt.Add(Midfielder.CreateMidfielder());
            for (int i = random; random < i + _tactics[2]; random++)
                rt.Add(Attacker.CreateAttacker());
            random = -1;
            ct.Refresh();
        }
    }
    class Player : IPlayer<Amateur>, IPlayer<Professional>
    {
        public static Func<string, bool> CountCheck;
        public static Func<int, bool> ContainsPlayer;
        public static Func<string, int> TacticsByPosition;
        public Action<int> SetPower;
        public RecomputeHandler Recompute;
        protected static int count => random < 0 ? ct.TeamCount : rt.TeamCount;
        private string? _name = null;
        private int _number = -1;
        protected int _power;
        private string? _position = null;
        public GetPlayerInfo info;
        public struct GetPlayerInfo
        {
            public string? name = null;
            public int? number = null;
            public int? power = null;
            public string? position = null;
            public GetPlayerInfo(Player p)
            {
                name = p._name;
                number = p._number;
                power = p._power;
                position = p._position;
            }
        }
        protected Player(int power, string position)
        {
            Recompute = new RecomputeHandler(PowerComputing);
            SetPower = delegate (int a) { _power = a; };
            if (technical)
                Console.WriteLine("\n >> Player constructor\n");
            if (random == 0)
                {
                    _name = "Coach";
                    _number = new Random().Next(1, 100);
                    _power = power;
                    info = new GetPlayerInfo(this);
                }
            else if (random > 0)
                {
                    _name = "Player" + random;
                    do
                        _number = new Random().Next(1, 100);
                    while (ContainsPlayer == null ? false : ContainsPlayer(_number));
                    _power = power;
                    _position = position;
                    info = new GetPlayerInfo(this);
                }
            else
            {
                do
                {
                    Console.Write("Enter a {0}'s name: ", GetSpeciality(this));
                    try { _name = Console.ReadLine(); }
                    catch (FormatException) { Console.WriteLine("Format error..."); }
                }
                while (_name == null);
                int number = -1;
                do
                {
                    Console.Write("Enter a {0}'s number: ", GetSpeciality(this));
                    try { number = int.Parse(Console.ReadLine()); }
                    catch (FormatException) { Console.WriteLine("Format error..."); }
                }
                while (!(number >= 1 && number <= 99 && (ContainsPlayer == null ? true : !ContainsPlayer(number))));
                _number = number;
                _power = power;
                if (position != null && ct.Count(position) < (position == "goalkeeper" ? 1 : TacticsByPosition(position)))
                    _position = position;
                info = new GetPlayerInfo(this);
                ct.Add(this);
            }
        }
        protected virtual int PowerComputing(string position) => _power = new Random().Next(4, 13) / 4;
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
            protected override int PowerComputing(string position) => _power = new Random().Next(4, 13) / 4;
            ~Amateur() { if (technical) Console.WriteLine("\n >> Amateur destructor\n"); }
    }
    class Professional : Player
        {
            protected Professional(string position) : base(3, position)
            {
                if (technical)
                    Console.WriteLine("\n >> Professional constructor\n");
            }
            public static Professional CreateProfessional(string position) =>
                                    typeof(Goalkeeper).ToString().ToLower().Contains(position) ? Goalkeeper.CreateGoalkeeper() :
                                    typeof(Defender).ToString().ToLower().Contains(position) ? Defender.CreateDefender() :
                                    typeof(Midfielder).ToString().ToLower().Contains(position) ? Midfielder.CreateMidfielder() : Attacker.CreateAttacker();
            protected override int PowerComputing(string position) => position == GetSpeciality(this) ? _power = 3 : _power = new Random().Next(2, 4);
            ~Professional() { if (technical) Console.WriteLine("\n >> Professional destructor\n"); }
        }
    class Goalkeeper : Professional
        {
            public static int count { get { return random < 0 ? ct.CountProfessional("goalkeeper") : rt.CountProfessional("goalkeeper"); } set { } }
            private Goalkeeper() : base("goalkeeper")
            {
                if (technical)
                    Console.WriteLine("\n >> Goalkeeper constructor\n");
            }
            public static Goalkeeper CreateGoalkeeper()
            {
                if (Player.count >= 16 || count >= 1)
                    throw new OutOfLimitException(new OutOfLimitExceptionArgs("goalkeeper"));
                return new Goalkeeper();
            }
            ~Goalkeeper() { if (technical) Console.WriteLine("\n >> Goalkeeper destructor\n"); }
        }
    class Defender : Professional
        {
            private Defender() : base("defender")
            {
                if (technical)
                    Console.WriteLine("\n >> Defender constructor\n");
            }
            public static Defender CreateDefender()
            {
                if (count >= 16 || CountCheck.Invoke("defender"))
                    throw new OutOfLimitException(new OutOfLimitExceptionArgs("defender"));
                return new Defender();
            }
            ~Defender() { if (technical) Console.WriteLine("\n >> Defender destructor\n"); }
        }
    class Midfielder : Professional
        {
            private Midfielder() : base("midfielder")
            {
                if (technical)
                    Console.WriteLine("\n >> Midfielder constructor\n");
            }
            public static Midfielder CreateMidfielder()
            {
                if (count >= 16 || CountCheck.Invoke("midfielder"))
                    throw new OutOfLimitException(new OutOfLimitExceptionArgs("midfielder"));
                return new Midfielder();
            }
            ~Midfielder() { if (technical) Console.WriteLine("\n >> Midfielder destructor\n"); }
        }
    class Attacker : Professional
        {
            private Attacker() : base("attacker")
            {
                if (technical)
                    Console.WriteLine("\n >> Attacker constructor\n");
            }
            public static Attacker CreateAttacker()
            {
                if (count >= 16 || CountCheck.Invoke("attacker"))
                    throw new OutOfLimitException(new OutOfLimitExceptionArgs("attacker"));
                return new Attacker();
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
            Program p = new Program();
            technical = p.CheckAnswer("Do you want to show technical inscriptions?");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            Start();
            while (true)
            {
                if (help)
                    Console.WriteLine("amateur - add a new non-professional player to the team\n" +
                          "professional - add a new professional player to the team\n" +
                          "switch - switch the player at specific position\n" +
                          "write - show the current team information\n" +
                          "team - create a new team with current coach\n" +
                          "play - start the match with an enemy team\n" +
                          "restart - full restart\n" +
                          "exit - exit application\n" +
                          /*"gc - test garbage collector\n" +*/
                          "technical - show / hide technical inscriptions\n" +
                          "help - show / hide instructions\n");
                Console.Write("Enter the command: ");
                switch (Console.ReadLine().ToLower())
                {
                    case "amateur":
                        try { IPlayer<Amateur>.CreatePlayer(); }
                        catch (OutOfLimitException e) { Console.WriteLine(e.Message); }
                        break;
                    case "professional":
                        try { IPlayer<Professional>.CreatePlayer(); }
                        catch (OutOfLimitException e) { Console.WriteLine(e.Message); }
                        break;
                    case "switch": ct.Switch(); break;
                    case "write": ct.WriteOut(); break;
                    case "team": if (p.CheckAnswer("Are you sure you want to create new team instead the current one?")) ct.CreateTeam(); break;
                    case "play": if (p.CheckAnswer("Are you sure you want to play the match?")) Play(); break;
                    case "restart": if (p.CheckAnswer("Are you sure you want to restart?")) Restart(); break;
                    case "exit": if (p.CheckAnswer("Are you sure you want to exit?")) Environment.Exit(0); break;
                    /*case "gc": GarbageCollector(); break;*/
                    case "technical": technical = !technical; break;
                    case "help": help = !help; break;
                    default: Console.WriteLine("The command is not found."); break;
                }
                Console.WriteLine();
            }
        }
    static void Start()
        {
            ct = new CustomTeam();
            ct.Start(true);
            Console.Clear();
        }
    static void Restart()
        {
            ct.Restart();
        }
    static void Play()
        {
            rt = new RandomTeam();
            rt.Start(true);
            Console.Clear();
            ct.WriteOut();
            Console.WriteLine("------------------ VS ------------------");
            rt.WriteOut();
            Console.WriteLine("----------------------------------------\nWinner: " + Team.Winner);
        }
}
public static class ProgramExtension
{
    public static bool CheckAnswer(this Program program, string answer)
    {
        char? ans = null;
        do
        {
            Console.WriteLine(answer + " (Y / N)");
            try { ans = char.Parse(Console.ReadLine().ToLower()); }
            catch (FormatException) { Console.WriteLine("Format error..."); }
        } while (ans != 'y' && ans != 'n');
        return ans == 'y' ? true : false;
    }
}
public class OutOfLimitException : Exception
{
    public OutOfLimitException(OutOfLimitExceptionArgs a) : base(a.full_message) { }
}
public class OutOfLimitExceptionArgs
{
    private const string message = "s are out of limit. Cannot add more!";
    private string speciality = null;
    public string full_message => char.ToUpper(speciality[0]).ToString() + (speciality + message).Remove(0, 1);
    public OutOfLimitExceptionArgs(string speciality)
    {
        this.speciality = speciality;
    }
}