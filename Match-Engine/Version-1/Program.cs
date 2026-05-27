Random rng = new Random();

const int HALF_LENGTH = 45;
const int NUMBER_OF_HALVES = 2;


static int[] SimulateMatch(Random rng, int HALF_LENGTH, int NUMBER_OF_HALVES)
{
    string[] startTexts = {"Kick Off", "Second Half Kick Off"};
    string[] endTexts = {"Half-Time", "Full-Time"};

    int[] score = {0,0};
    double ballPosition = 50;
    int addedTime = 0;

    for (int half = 1; half <= NUMBER_OF_HALVES; half++)
    {
        Console.WriteLine(startTexts[half - 1]);
        addedTime = 0;

        for (int minute = 1; minute <= HALF_LENGTH; minute++)
        {
            SimulateMinute(rng, ref score, ref ballPosition, minute + ((half - 1) * HALF_LENGTH) + addedTime);
            
            if (minute == 45 & addedTime == 0)
            {
                addedTime = GetAddedTime(rng, score);
                minute -= addedTime;
            }
        }
        Console.WriteLine(endTexts[half - 1]);
    }

    return score;
}


static void SimulateMinute(Random rng, ref int[] score, ref double ballPosition, int minute)
{
    double direction = Math.Pow(-1, rng.Next(1,3));
    double movement = rng.NextDouble() * 10;

    ballPosition +=  direction * movement;

    //Console.WriteLine($"{ballPosition}, {direction}, {movement}");

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


static int GetAddedTime(Random rng, int[] score)
{
    int addedTime = rng.Next(1, 3 + score[0] + score[1]);
    Console.WriteLine($"- {addedTime}' Added ({score[0]} - {score[1]})."); 
    return addedTime;
}


int[] score = SimulateMatch(rng, HALF_LENGTH, NUMBER_OF_HALVES);

Console.WriteLine($"\nHome {score[0]} - {score[1]} Away");