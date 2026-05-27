Random rng = new Random();

const int HALF_LENGTH = 45;
const int NUMBER_OF_HALVES = 2;

int homeTeamAbility = 75;
int awayTeamAbility = 75;


static int[] SimulateMatch(Random rng, int HALF_LENGTH, int NUMBER_OF_HALVES, int homeTeamAbility, int awayTeamAbility)
{
    string[] startTexts = {"Kick Off", "Second Half Kick Off"};
    string[] endTexts = {"Half-Time", "Full-Time"};
    int[] score = {0,0};

    for (int halfNumber = 0; halfNumber < NUMBER_OF_HALVES; halfNumber++)
    {
        Console.WriteLine(startTexts[halfNumber]);
        SimulateHalf(rng, HALF_LENGTH, ref score, halfNumber, homeTeamAbility, awayTeamAbility);
        Console.WriteLine(endTexts[halfNumber]);
    }

    return score;
}


static void SimulateHalf(Random rng, int HALF_LENGTH, ref int[] score, int halfNumber, int homeTeamAbility, int awayTeamAbility)
{
    double ballPosition = 50;
    int addedTime = 0;

    for (int minute = 1; minute <= HALF_LENGTH; minute++)
    {
        int currentMinute = minute + (halfNumber * HALF_LENGTH) + addedTime;
        SimulateMinute(rng, ref score, ref ballPosition, currentMinute, homeTeamAbility, awayTeamAbility);
        
        if (minute == 45 & addedTime == 0)
        {
            addedTime = GetAddedTime(rng, score);     
            Console.WriteLine($"- {addedTime}' Added ({score[0]} - {score[1]})."); 
            minute -= addedTime;
        }
    }
}


static void SimulateMinute(Random rng, ref int[] score, ref double ballPosition, int minute, int homeTeamAbility, int awayTeamAbility)
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


static double SimulateBallMovement(Random rng, int homeTeamAbility, int awayTeamAbility)
{
    double direction = rng.Next(1, homeTeamAbility + awayTeamAbility);
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


int totalGoals = 0;
int maxGoals = 0;
int[] maxScore = {0,0};
int nillNills = 0;

int numberOfMatches = 10_000;

for (int i = 0; i < numberOfMatches; i++)
{
    int[] score = SimulateMatch(rng, HALF_LENGTH, NUMBER_OF_HALVES, homeTeamAbility, awayTeamAbility);
    Console.WriteLine($"\nHome {score[0]} - {score[1]} Away");
    
    int scoreTotal = score[0] + score[1];
    if (scoreTotal == 0) { nillNills++; }
    else if (scoreTotal > maxGoals) { maxGoals = scoreTotal; maxScore = score; }
    totalGoals += scoreTotal;
}
Console.WriteLine($"\n\n{totalGoals} goals from {numberOfMatches} matches ({Math.Round(totalGoals / (double)numberOfMatches, 2)} gp90).");
Console.WriteLine($"{nillNills} 0-0s ({Math.Round(nillNills / (double)numberOfMatches, 2)}%), ({maxScore[0]} - {maxScore[1]}) was the highest scoring game.");