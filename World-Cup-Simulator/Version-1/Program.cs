MatchStats matchStats = new MatchStats();
double numberOfGames = 1;

for (int i = 0; i < numberOfGames; i += 1)
{
    Team homeTeam = new Team("Home Team", 100);
    Team awayTeam = new Team("Away Team", 100);

    MatchEngine me = new MatchEngine(true);
    (string winner, int[] score, MatchStats stats) = me.SimulateMatch(homeTeam, awayTeam);

    matchStats.AddedTime += stats.AddedTime;
    matchStats.FreeKicksGiven += stats.FreeKicksGiven;
    matchStats.FreeKicksScored += stats.FreeKicksScored;
    matchStats.Goals += stats.Goals;
    matchStats.Injuries += stats.Injuries;
    matchStats.PenaltiesGiven += stats.PenaltiesGiven;
    matchStats.PenaltiesScored += stats.PenaltiesScored;
    matchStats.RedCards += stats.RedCards;
}

class MatchEngine
{
    private const int HALF_LENGTH = 45;
    private const int NUMBER_OF_HALVES = 2;

    Random rng;

    private Team currentHomeTeam;
    private Team currentAwayTeam;

    private MatchStats matchStats;

    private bool vidiprinter;

    public MatchEngine(bool useVidiprinter)
    {
        rng = new Random();
        vidiprinter = useVidiprinter;
    }


    public Tuple<string,int[],MatchStats> SimulateMatch(Team homeTeam, Team awayTeam)
    {
        currentHomeTeam = homeTeam;
        currentAwayTeam = awayTeam;
        matchStats = new MatchStats();

        if (vidiprinter) { Console.WriteLine($"| K/O  | {currentHomeTeam.Name} vs. {currentAwayTeam.Name}"); }

        for (int halfNumber = 0; halfNumber < NUMBER_OF_HALVES; halfNumber += 1)
        {
            SimulateHalf(halfNumber);
            if (vidiprinter && halfNumber == 0) { Console.WriteLine($"| H/T  | {GetCurrentScoreString()}"); }
        }

        string winner = GetWinner();
        if (vidiprinter) { Console.WriteLine($"| F/T  | {GetCurrentScoreString(winner)}"); }

        Console.WriteLine("Stats:");
        Console.WriteLine($"- Added Time p90: {matchStats.AddedTime}.");
        Console.WriteLine($"- Goals p90: {matchStats.Goals}.");
        Console.WriteLine($"- Injuries p90: {matchStats.Injuries}.");
        Console.WriteLine($"- Red Cards p90: {matchStats.RedCards}.");
        Console.WriteLine($"- Penalties p90: {matchStats.PenaltiesGiven} ({matchStats.PenaltiesScored / (double)matchStats.PenaltiesGiven}).");
        Console.WriteLine($"- Free Kicks p90: {matchStats.FreeKicksGiven} ({matchStats.FreeKicksScored / (double)matchStats.FreeKicksGiven}).");

        return Tuple.Create(winner, new int[] {currentHomeTeam.Goals, currentAwayTeam.Goals}, matchStats);
    }


    void SimulateHalf(int halfNumber)
    {
        double playProgress = 0;
        int firstHalfAddedTime = 0;

        for (int minute = 0; minute <= HALF_LENGTH + (matchStats.AddedTime - firstHalfAddedTime); minute += 1)
        {
            int currentMinute = minute + (halfNumber * HALF_LENGTH);

            SimulateMinute(ref playProgress, currentMinute);
            firstHalfAddedTime = matchStats.AddedTime;
        }
    }


    void SimulateMinute(ref double playProgress, int minute)
    {
        SimulatePlay(ref playProgress, minute);
        SimulateRandomEvents(ref playProgress, minute);
    }


    void SimulatePlay(ref double playProgress, int minute)
    {
        double abilityTotal = currentHomeTeam.CurrentAbility + currentAwayTeam.CurrentAbility;
        double direction = rng.Next(1, (int)abilityTotal);
        double distance = rng.NextDouble() * 38;

        if (direction > currentHomeTeam.Ability) { distance *= -1; }

        playProgress += distance;

        if (Math.Abs(playProgress) > 100) 
        { 
            if (playProgress > 0) 
            { 
                currentHomeTeam.Score(); 
                if (vidiprinter) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("home")} ({minute}')"); }
            }
            else 
            { 
                currentAwayTeam.Score(); 
                if (vidiprinter) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("away")} ({minute}')"); }
            }
        
            matchStats.Goals += 1; matchStats.AddedTime += 1; playProgress = 0; 
        }
    }


    void SimulateRandomEvents(ref double playProgress, int minute)
    {
        int eventValue = rng.Next(1, 10_001);

        if (eventValue <= 6) // Home Red Card.
        { 
            currentHomeTeam.AdjustCurrentAbility(-0.09); 
            matchStats.AddedTime += 2;
            matchStats.RedCards += 1;

            if (vidiprinter) { Console.WriteLine($"| RED  | {GetCurrentScoreString("home")} ({minute}')"); }
            
            if (playProgress < -65) { eventValue = 2000; } // Give Away Penalty / Free Kick.
            else { return; }
        }
        if (eventValue <= 12) // Away Red Card.
        { 
            currentAwayTeam.AdjustCurrentAbility(-0.09); 
            matchStats.AddedTime += 2;
            matchStats.RedCards += 1;

            if (vidiprinter) { Console.WriteLine($"| RED  | {GetCurrentScoreString("away")} ({minute}')"); }
            
            if (playProgress > 65) { eventValue = 2000; } // Give Home Penalty / Free Kick.
            else { return; }
        }

        if (eventValue <= 18) // Home Injury.
        {
            currentHomeTeam.AdjustCurrentAbility(-0.05);
            matchStats.AddedTime += 1;
            matchStats.Injuries += 1;
            return;
        }
        if (eventValue <= 24) // Away Injury.
        {
            currentHomeTeam.AdjustCurrentAbility(-0.05);
            matchStats.AddedTime += 1;
            matchStats.Injuries += 1;
            return;
        }

        if (eventValue > 224) { return; }

        int scoreChance = rng.Next(1,100);

        bool scored = false;
        if (Math.Abs(playProgress) > 80) // Penalty.
        {
            matchStats.PenaltiesGiven += 1;
            if (scoreChance < 76)
            {
                matchStats.PenaltiesScored += 1;
                playProgress /= 10;
                scored = true;
            }
            matchStats.AddedTime += 3;
        }
        if (Math.Abs(playProgress) > 65) // Home Free Kick.
        {
            matchStats.FreeKicksGiven += 1;
            if (scoreChance < 6)
            {
                matchStats.FreeKicksScored += 1;
                playProgress /= 10;
                scored = true;
            }
            matchStats.AddedTime += 1;
        }

        if (scored && playProgress > 0) 
        { 
            currentHomeTeam.Score(); 
            if (vidiprinter && playProgress > 8) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("home")} ({minute}' pen)"); }
            else if (vidiprinter) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("home")} ({minute}')"); }
        }
        if (scored && playProgress < 0) 
        { 
            currentAwayTeam.Score(); 
            if (vidiprinter && playProgress > 8) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("away")} ({minute}' pen)"); }
            else if (vidiprinter) { Console.WriteLine($"| GOAL | {GetCurrentScoreString("away")} ({minute}')"); }
        }
    }


    string GetCurrentScoreString(string highlightedTeam = "none")
    {
        if (highlightedTeam == "home") { return $"{currentHomeTeam.Name.ToUpper()} {currentHomeTeam.Goals}-{currentAwayTeam.Goals} {currentAwayTeam.Name}"; }
        if (highlightedTeam == "away") { return $"{currentHomeTeam.Name} {currentHomeTeam.Goals}-{currentAwayTeam.Goals} {currentAwayTeam.Name.ToUpper()}"; }
        return $"{currentHomeTeam.Name} {currentHomeTeam.Goals}-{currentAwayTeam.Goals} {currentAwayTeam.Name}";
    }


    string GetWinner()
    {
        if (currentHomeTeam.Goals > currentAwayTeam.Goals) { return "home"; }
        if (currentHomeTeam.Goals < currentAwayTeam.Goals) { return "away"; }
        return "none";
    }
}


struct Team
{
    private string name;

    private double ability;
    private double currentAbility;

    private int currentGoals;

    public Team(string teamName, double teamAbility)
    {
        name = teamName;

        if (teamAbility > 100) { ability = 100; }
        else if (teamAbility < 1) { ability = 1; }
        else { ability = teamAbility; }
        currentAbility = teamAbility;

        currentGoals = 0;
    }

    public string Name { get { return name; }}

    public double Ability { get { return ability; } set { ability = value; }}

    public double CurrentAbility { get { return currentAbility; } }
    public void AdjustCurrentAbility(double adjustment) { currentAbility *= (1 + adjustment); }
    public void ResetCurrentAbility() { currentAbility = ability; }

    public int Goals { get { return currentGoals; } }
    public void Score() { currentGoals += 1; }

    public void ResetTeam()
    {
        currentAbility = ability;
        currentGoals = 0;
    }
}


struct MatchStats
{
    private int goals;
    private int addedTime;

    private int penaltiesGiven;
    private int penaltiesScored;
    private int freeKicksGiven;
    private int freeKicksScored;

    private int redCards;
    private int injuries;

    public MatchStats()
    {
        goals = 0;
        addedTime = 0;

        penaltiesGiven = 0;
        penaltiesScored = 0;
        freeKicksGiven = 0;
        freeKicksScored = 0;

        redCards = 0;
        injuries = 0;
    }

    public int Goals { get { return goals;} set { goals += value; } }
    public int AddedTime { get { return addedTime;} set { addedTime += value; } }

    public int PenaltiesGiven { get { return penaltiesGiven;} set { penaltiesGiven += value; } }
    public int PenaltiesScored { get { return penaltiesScored;} set { penaltiesScored += value; } }
    public int FreeKicksGiven { get { return freeKicksGiven;} set { freeKicksGiven += value; } }
    public int FreeKicksScored { get { return freeKicksScored;} set { freeKicksScored += value; } }

    public int RedCards { get { return redCards;} set { redCards += value; } }
    public int Injuries { get { return injuries;} set { injuries += value; } }
}