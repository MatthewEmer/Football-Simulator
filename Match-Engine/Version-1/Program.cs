Random rng = new Random();

const int HALF_LENGTH = 45;
const int NUMBER_OF_HALVES = 2;

double homeTeamAbility = 100;
double awayTeamAbility = 100;


static int[] SimulateMatch(Random rng, int HALF_LENGTH, int NUMBER_OF_HALVES, double homeTeamAbility, double awayTeamAbility)
{
    int[] score = {0,0};

    string[] startTexts = {"Kick Off", "Second Half Kick Off"};
    string[] endTexts = {$"Half-Time", $"Full-Time"};

    for (int halfNumber = 0; halfNumber < NUMBER_OF_HALVES; halfNumber++)
    {
        Console.WriteLine(startTexts[halfNumber]);
        SimulateHalf(rng, HALF_LENGTH, ref score, halfNumber, homeTeamAbility, awayTeamAbility);
        Console.WriteLine($"{endTexts[halfNumber]} ({score[0]} - {score[1]}).");
    }

    return score;
}


static void SimulateHalf(Random rng, int HALF_LENGTH, ref int[] score, int halfNumber, double homeTeamAbility, double awayTeamAbility)
{
    double ballPosition = 50;
    int addedTime = 0;

    for (int minute = 1; minute <= HALF_LENGTH; minute++)
    {
        int currentMinute = minute + (halfNumber * HALF_LENGTH) + addedTime;
        SimulateMinute(rng, ref score, ref ballPosition, currentMinute, homeTeamAbility, awayTeamAbility);
        SimulateRandomEvents(rng, ref score, ref homeTeamAbility, ref awayTeamAbility, currentMinute);
        
        if (minute == 45 & addedTime == 0)
        {
            addedTime = GetAddedTime(rng, score);     
            Console.WriteLine($"- {addedTime}' Added."); 
            minute -= addedTime;
        }
    }
}


static void SimulateMinute(Random rng, ref int[] score, ref double ballPosition, int minute, double homeTeamAbility, double awayTeamAbility)
{
    ballPosition += SimulateBallMovement(rng, homeTeamAbility, awayTeamAbility);

    if (ballPosition > 100)
    {
        Console.WriteLine($"- {minute}' Home Goal ({++score[0]} - {score[1]}).");
        ballPosition = 50;
    }
    else if (ballPosition < 0)
    {
        Console.WriteLine($"- {minute}' Away Goal ({score[0]} - {++score[1]}).");
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


static void SimulateRandomEvents(Random rng, ref int[] score, ref double homeTeamAbility, ref double awayTeamAbility, int minute)
{
    int eventNumber = rng.Next(1,2001);

    if (eventNumber == 1) // Home team red card.
    {
        Console.WriteLine($"- {minute}' Home Team Red Card ({score[0]} - {score[1]}).");
        homeTeamAbility *= 0.91;
        return;
    }
    else if (eventNumber == 2) // Away team red card.
    {
        Console.WriteLine($"- {minute}' Away Team Red Card ({score[0]} - {score[1]}).");
        awayTeamAbility *= 0.91;
        return;
    }

    else if (eventNumber <= 4) // Home team injury.
    {
        Console.WriteLine($"- {minute}' Home Team Injury ({score[0]} - {score[1]}).");
        homeTeamAbility *= 0.95;
        return;
    }
    else if (eventNumber <= 6) // Away team injury.
    {
        Console.WriteLine($"- {minute}' Away Team Injury ({score[0]} - {score[1]}).");
        awayTeamAbility *= 0.95;
        return;
    }

    else if (eventNumber <= 8) // Home team penalty.
    {
        Console.WriteLine($"- {minute}' Home Team Penalty...");
        if (eventNumber == 7)
        {
            Console.WriteLine($"- {minute}' Scored! ({++score[0]} - {score[1]}).");
        }
        else
        {
            Console.WriteLine($"- {minute}' Missed. ({score[0]} - {score[1]}).");
        }
        return;
    }
    else if (eventNumber <= 10) // Away team penalty.
    {
        Console.WriteLine($"- {minute}' Away Team Penalty...");
        if (eventNumber == 9)
        {
            Console.WriteLine($"- {minute}' Scored! ({score[0]} - {++score[1]}).");
        }
        else
        {
            Console.WriteLine($"- {minute}' Missed. ({score[0]} - {score[1]}).");
        }
        return;
    }
}

int numberOfMatches = 10;

for (int i = 0; i < numberOfMatches; i++)
{
    int[] score = SimulateMatch(rng, HALF_LENGTH, NUMBER_OF_HALVES, homeTeamAbility, awayTeamAbility);
    Console.WriteLine("\n\n");
}