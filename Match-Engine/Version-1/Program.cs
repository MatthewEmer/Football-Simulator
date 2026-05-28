using System;
using System.Threading;

Random rng = new Random();

const int HALF_LENGTH = 45;
const int NUMBER_OF_HALVES = 2;


static void SimulateMatch(Random rng, int HALF_LENGTH, int NUMBER_OF_HALVES, string homeTeam, double homeTeamAbility, string awayTeam, double awayTeamAbility)
{
    int[] score = {0,0};

    Console.WriteLine($"| K/O  | {homeTeam} vs. {awayTeam}");
    for (int halfNumber = 0; halfNumber < NUMBER_OF_HALVES; halfNumber++)
    {
        SimulateHalf(rng, HALF_LENGTH, ref score, halfNumber, homeTeam, homeTeamAbility, awayTeam, awayTeamAbility);
        
        if (halfNumber == 0)
        {
            Console.WriteLine($"| H/T  | {homeTeam} {score[0]}-{score[1]} {awayTeam}");
        }
        else
        {
            Console.WriteLine($"| F/T  | {homeTeam} {score[0]}-{score[1]} {awayTeam}");
        }

        Thread.Sleep(2000);
    }
}


static void SimulateHalf(Random rng, int HALF_LENGTH, ref int[] score, int halfNumber, string homeTeam, double homeTeamAbility, string awayTeam, double awayTeamAbility)
{
    double ballPosition = 50;
    int addedTime = 0;

    for (int minute = 1; minute <= HALF_LENGTH; minute++)
    {
        int currentMinute = minute + (halfNumber * HALF_LENGTH) + addedTime;
        SimulateMinute(rng, ref score, ref ballPosition, currentMinute, homeTeam, homeTeamAbility, awayTeam, awayTeamAbility);
        SimulateRandomEvents(rng, ref score, homeTeam, ref homeTeamAbility, awayTeam, ref awayTeamAbility, currentMinute);
        
        if (minute == 45 & addedTime == 0)
        {
            addedTime = GetAddedTime(rng, score);     
            minute -= addedTime;
        }

        Thread.Sleep(100);
    }
}


static void SimulateMinute(Random rng, ref int[] score, ref double ballPosition, int minute, string homeTeam, double homeTeamAbility, string awayTeam, double awayTeamAbility)
{
    ballPosition += SimulateBallMovement(rng, homeTeamAbility, awayTeamAbility);

    if (ballPosition < 0 || ballPosition > 100)
    {
        if (ballPosition > 100)
        {
            Console.WriteLine($"| GOAL | {homeTeam.ToUpper()} {++score[0]}-{score[1]} {awayTeam} ({minute}')");
        }
        else if (ballPosition < 0)
        {
            Console.WriteLine($"| GOAL | {homeTeam} {score[0]}-{++score[1]} {awayTeam.ToUpper()} ({minute}')");
        }
        ballPosition = 50;
    }
}


static double SimulateBallMovement(Random rng, double homeTeamAbility, double awayTeamAbility)
{
    double direction = rng.Next(1, (int)(homeTeamAbility + awayTeamAbility));
    double distance = rng.NextDouble() * 19;

    if (direction > homeTeamAbility) 
    { 
        distance *= -1; 
    }

    return distance;
}


static int GetAddedTime(Random rng, int[] score)
{
    int addedTime = rng.Next(1, 3 + score[0] + score[1]);

    return addedTime;
}


static void SimulateRandomEvents(Random rng, ref int[] score, string homeTeam, ref double homeTeamAbility, string awayTeam, ref double awayTeamAbility, int minute)
{
    int eventNumber = rng.Next(1,2001);

    if (eventNumber == 1 || eventNumber == 2)
    {
        if (eventNumber == 1) // Home team red card.
        {
            Console.WriteLine($"| RED  | {homeTeam.ToUpper()} {score[0]}-{score[1]} {awayTeam} ({minute}')");
            homeTeamAbility *= 0.91;
            return;
        }
        else if (eventNumber == 2) // Away team red card.
        {
            Console.WriteLine($"| RED  | {homeTeam} {score[0]}-{score[1]} {awayTeam.ToUpper()} ({minute}')");
            awayTeamAbility *= 0.91;
            return;
        }
    }

    else if (eventNumber <= 4) // Home team injury.
    {
        homeTeamAbility *= 0.95;
        return;
    }
    else if (eventNumber <= 6) // Away team injury.
    {
        awayTeamAbility *= 0.95;
        return;
    }

    else if (eventNumber <= 8) // Home team penalty.
    {
        if (eventNumber == 7)
        {
            Console.WriteLine($"| GOAL | {homeTeam.ToUpper()} {++score[0]}-{score[1]} {awayTeam} ({minute}' pen)");
        }
        return;
    }
    else if (eventNumber <= 10) // Away team penalty.
    {
        if (eventNumber == 9)
        {
            Console.WriteLine($"| GOAL | {homeTeam} {score[0]}-{++score[1]} {awayTeam.ToUpper()} ({minute}' pen)");
        }
        return;
    }
}

Dictionary<string, double> teamAbilities = new Dictionary<string, double>()
{
    {"Bournemouth", 79},
    {"Arsenal", 90},
    {"Aston Villa", 82},
    {"Brentford", 78},
    {"Brighton", 81},
    {"Burnley", 74},
    {"Chelsea", 85},
    {"Crystal Palace", 79},
    {"Everton", 78},
    {"Fulham", 79},
    {"Leeds", 76},
    {"Liverpool", 91},
    {"Manchester City", 91},
    {"Manchester Utd.", 84},
    {"Newcastle", 84},
    {"Nott. Forest", 80},
    {"Sunderland", 76},
    {"Spurs", 83},
    {"West Ham", 78},
    {"Wolves", 76}
};

List<string[]> matches = new List<string[]>()
{
    new string[] {"Brighton", "Manchester Utd."},
    new string[] {"Burnley", "Wolves"},
    new string[] {"Crystal Palace", "Arsenal"},
    new string[] {"Fulham", "Newcastle"},
    new string[] {"Liverpool", "Brentford"},
    new string[] {"Manchester City", "Aston Villa"},
    new string[] {"Nott. Forest", "Bournemouth"},
    new string[] {"Sunderland", "Chelsea"},
    new string[] {"Spurs", "Everton"},
    new string[] {"West Ham", "Leeds"}
};

Thread[] matchThreads = new Thread[10];

for (int i = 0; i < 10; i++)
{
    string homeTeam = matches[i][0];
    double homeTeamAbility = 0;
    string awayTeam = matches[i][1];
    double awayTeamAbility = 0;

    try
    {
        homeTeamAbility = teamAbilities[homeTeam];
        awayTeamAbility = teamAbilities[awayTeam];
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    matchThreads[i] = new Thread(() => SimulateMatch(rng, HALF_LENGTH, NUMBER_OF_HALVES, homeTeam, homeTeamAbility, awayTeam, awayTeamAbility));
}

foreach (Thread match in matchThreads)
{
    match.Start();
}