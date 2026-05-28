using System.Runtime.InteropServices.Swift;

MatchEngine me = new MatchEngine();
me.SimulateMatch("home", 50, "away", 30, true);

class MatchEngine
{
    private const int HALF_LENGTH = 45;
    private const int NUMBER_OF_HALVES = 2;

    private Random rng;

    private Team currentHomeTeam;
    private Team currentAwayTeam;

    public MatchEngine()
    {
        rng = new Random();
    }


    public void SimulateMatch(Team homeTeam, Team awayTeam, bool vidiprinter = false)
    {
        currentHomeTeam = homeTeam;
        currentAwayTeam = awayTeam;

        if (vidiprinter) { Console.WriteLine($"| K/O  | {currentHomeTeam.Name} vs. {currentAwayTeam.Name}"); }

        for (int halfNumber = 0; halfNumber <= NUMBER_OF_HALVES; halfNumber++)
        {
            if (vidiprinter && halfNumber == 1) { Console.WriteLine($"| H/T  | {currentHomeTeam.Name} {currentHomeTeam.Goals}-{currentAwayTeam.Goals} {currentAwayTeam.Name}"); }
        }

        if (vidiprinter) { Console.WriteLine($"| F/T  | {currentHomeTeam.Name} {currentHomeTeam.Goals}-{currentAwayTeam.Goals} {currentAwayTeam.Name}"); }
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

        if (ability > 100) { ability = 100; }
        else if (ability < 1) { ability = 1; }
        else { ability = teamAbility; }
        currentAbility = ability;

        currentGoals = 0;
    }

    public string Name() { return name; }

    public double Ability() { return ability; }
    public void SetAbility(double newAbility) { ability = newAbility; }

    public double CurrentAbility() { return currentAbility; }
    public void AdjustCurrentAbility(double adjustment) { currentAbility += adjustment; }
    public void ResetCurrentAbility() { currentAbility = ability; }

    public int Goals() { return currentGoals; }
    public void Score() { currentGoals++; }

    public void ResetTeam()
    {
        currentAbility = ability;
        currentGoals = 0;
    }
}