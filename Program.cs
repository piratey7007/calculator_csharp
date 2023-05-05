using System.Text;

Console.WriteLine("""
______________________________________________
~ The Calculator App ~
______________________________________________
Type out any basic operation (+, -, *, /)
and press Enter.
------------------------------
(Press Ctrl+C to exit the app)
------------------------------
Start whenever you're ready
------------------------------
""");


Result handleInput(string input)
{
    if (input == null || input == "")
    {
        return new Result(1, "Invalid input. Please enter an operation.", null);
    }

    Array characters = input.ToCharArray();
    List<object> parts = new List<object>();
    string? prevType = null;
    StringBuilder number = new StringBuilder();

    for (int i = 0; i < characters.Length + 1; i++)
    {
        if (i == characters.Length)
        {
            double finalNumber;
            if (number.Length != 0)
            {
                finalNumber = Convert.ToDouble(number.ToString());
                parts.Add(finalNumber);
            }
            break;
        }
        char character = (char)characters.GetValue(i)!;
        if (character >= '0' && character <= '9')
        {
            if (prevType != "number")
            {
                number = new StringBuilder();
            }
            number.Append(character);
            prevType = "number";
        }
        else if (character == '+' || character == '-' || character == '*' || character == '/')
        {
            // character is an operation
            if (prevType == "operation")
            {
                if (character == '-')
                {
                    number = new StringBuilder();
                    number.Append(character);
                    prevType = "number";
                }
                else
                {
                    return new Result(1, $"Invalid operation at character {i + 1}. Please enter a valid operation.", null);
                }
            }
            else
            {
                double finalNumber;
                if (number.Length != 0)
                {
                    finalNumber = Convert.ToDouble(number.ToString());
                    parts.Add(finalNumber);
                }
                parts.Add(character);
                prevType = "operation";
            }
        }
        else if (character == ' ')
        {
            // character is a space
        }
        else if (character == '(' || character == ')')
        {
            // character is a parenthesis
        }
        else
        {
            return new Result(1, $"Invalid character at character {i + 1}.", null);
        }
    }
    Console.WriteLine("_________________");
    foreach (var part in parts) Console.WriteLine(part);
    Console.WriteLine("_________________");
    return new Result(0, null, parts);
}

Result processParts(List<object> parts)
{
    for (int i = 0; i < parts.Count; i++)
    {
        if (parts[i] is char operation)
        {
            if (i == 0 || i == parts.Count - 1)
            {
                return new Result(1, $"Invalid order of operations at character {i + 1}", null);
            }
            if (operation == '*')
            {
                var previous = parts[i - 1];
                var next = parts[i + 1];
                if (previous is double && next is double)
                {
                    Console.WriteLine($"previous: {previous}, next: {next},  operation: {operation}, answer: {(double)previous * (double)next}");
                    parts[i] = (double)previous * (double)next;
                    parts.Remove(previous);
                    parts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, $"Invalid order of operations at character {i + 1}", null);
            }
            if (operation == '/')
            {
                var previous = parts[i - 1];
                var next = parts[i + 1];
                if (previous is double && next is double)
                {
                    Console.WriteLine($"previous: {previous}, next: {next},  operation: {operation}, answer: {(double)previous / (double)next}");
                    parts[i] = (double)previous / (double)next;
                    parts.Remove(previous);
                    parts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, $"Invalid order of operations at character {i + 1}", null);
            }
        }

    }
    for (int i = 0; i < parts.Count; i++)
    {
        if (parts[i] is char operation)
        {
            if (operation == '+')
            {
                var previous = parts[i - 1];
                var next = parts[i + 1];
                if (previous is double && next is double)
                {
                    parts[i] = (double)previous + (double)next;
                    parts.Remove(previous);
                    parts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
            if (operation == '-')
            {
                var previous = parts[i - 1];
                var next = parts[i + 1];
                if (previous is double && next is double)
                {
                    parts[i] = (double)previous - (double)next;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
        }

    }
    return new Result(0, null, parts[0]);
}

void loop()
{
    string? input = null;
    while (input == null || input == "")
    {
        input = Console.ReadLine();
    }
    var res = handleInput(input);
    if (res.Status != 0)
    {
        Console.WriteLine(res.Message);
        Console.WriteLine("Try again.");
        loop();
    }
    if (res.Data is not List<object>)
    {
        Console.WriteLine("Something went wrong. Please try again.");
        loop();
    }
    Result processedRes = processParts((List<object>)res.Data!);
    while (input == null || input == "")
    {
        input = Console.ReadLine();
    }
    if (processedRes.Status != 0)
    {
        Console.WriteLine(processedRes.Message);
        Console.WriteLine("Try again.");
        loop();
    }
    if (processedRes.Data is not double)
    {
        Console.WriteLine("Something went wrong. Please try again.");
        loop();
    }
    Console.WriteLine((double)processedRes.Data);
    loop();
};


loop();

public class Result
{
    public Result(double? status, string? message, dynamic? data)
    {
        if (status != null) Status = (double)status;
        if (message != null) Message = message;
        if (data != null) Data = data;
    }
    public double Status { get; set; }
    public string? Message { get; set; }
    public dynamic? Data { get; set; }
}