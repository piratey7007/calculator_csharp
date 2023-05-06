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
    System.Console.WriteLine("handleInput start");
    if (input == null || input == "")
    {
        return new Result(1, "Invalid input. Please enter an operation.", null);
    }

    Array characters = input.ToCharArray();
    List<object> parts = new List<object>();
    string? prevType = null;
    StringBuilder number = new StringBuilder();
    int pCount = 0;

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
            // character is a number
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
                    // Character is a negative sign
                    number = new StringBuilder();
                    number.Append(character);
                    prevType = "number";
                }
                else if (character == '+')
                {
                    // Character is a positive sign
                    // Nothing happens
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
        else if (character == '(' || character == ')')
        {
            // Character is a parenthesis
            if (prevType == "operation" && character == ')') return new Result(1, $"90Invalid parenthesis at character {i + 1}.", null);
            parts.Add(character);
            if (character == '(') pCount++;
            else pCount--;
            if (pCount < 0) return new Result(1, $"94Invalid parenthesis at character {i + 1}", null);
            prevType = "parenthesis";
        }
        else if (character == ' ')
        {
            // Character is a space
            // Nothing happens
        }
        else
        {
            return new Result(1, $"Invalid character at character {i + 1}.", null);
        }
    }
    if (pCount != 0) return new Result(1, "Expected closing parenthesis.", null);
    return new Result(0, null, parts);
}

Result handleOperations(List<object> oParts)
{
    System.Console.WriteLine("handleOperations start");
    if (oParts.Count == 1) return new Result(0, null, oParts[0]);
    for (int i = 0; i < oParts.Count; i++)
    {
        if (oParts[i] is char operation)
        {
            if (operation == '*')
            {
                var previous = oParts[i - 1];
                var next = oParts[i + 1];
                if (previous is double && next is double)
                {
                    oParts[i] = (double)previous * (double)next;
                    oParts.Remove(previous);
                    oParts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
            if (operation == '/')
            {
                var previous = oParts[i - 1];
                var next = oParts[i + 1];
                if (previous is double && next is double)
                {
                    oParts[i] = (double)previous * (double)next;
                    oParts.Remove(previous);
                    oParts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
        }
    }

    for (int i = 0; i < oParts.Count; i++)
    {
        if (oParts[i] is char operation)
        {
            if (operation == '+')
            {
                Console.WriteLine(i + " " + oParts.Count);
                oParts.ForEach(Console.WriteLine);
                var previous = oParts[i - 1];
                var next = oParts[i + 1];
                if (previous is double && next is double)
                {
                    oParts[i] = (double)previous + (double)next;
                    oParts.Remove(previous);
                    oParts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
            if (operation == '-')
            {
                var previous = oParts[i - 1];
                var next = oParts[i + 1];
                if (previous is double && next is double)
                {
                    oParts[i] = (double)previous - (double)next;
                    oParts.Remove(previous);
                    oParts.Remove(next);
                    i = i - 2;
                }
                else return new Result(1, "Invalid order of operations", null);
            }
        }
    }
    return new Result(0, null, oParts[0]);
}

Result findParentheses(List<object> fParts)
{
    System.Console.WriteLine("findParentheses start");
    Result run()
    {
        int? open = null;
        int? close = null;
        for (int i = 0; i < fParts.Count; i++)
        {
            if (fParts[i] is char p)
            {
                if (p == '(') open = i;
                else if (p == ')')
                {
                    if (open == null) return new Result(1, "Expected opening parenthesis.", null);
                    close = i;
                    fParts.RemoveAt(close.Value);
                    fParts.RemoveAt(open.Value);
                    List<object> range = fParts.GetRange(open.Value, close.Value - open.Value);
                    Result res = handleOperations((List<object>)range!);
                    System.Console.WriteLine("209: handleOperations:");
                    if (res.Data.GetType() == typeof(double)) Console.WriteLine(res.Data);
                    else if (res.Data.GetType() == typeof(List<object>)) foreach (object o in res.Data!) System.Console.WriteLine(o);
                    fParts.RemoveRange(open.Value, close.Value - open.Value);
                    fParts.Insert(open.Value, res.Data);
                    run();
                }
            }
        }
        return new Result(0, null, fParts);
    }
    return run();
}

Result processParts(List<object> parts)
{
    Result res = findParentheses(parts);
    Console.WriteLine("232: handleAllParentheses:");
    if (res.Data.GetType() == typeof(double)) Console.WriteLine(res.Data);
    else if (res.Data.GetType() == typeof(List<object>)) foreach (object o in res.Data!) System.Console.WriteLine(o);
    Result res2 = handleOperations(res.Data);
    Console.WriteLine("236: handleOperations:");
    if (res2.Data.GetType() == typeof(double)) Console.WriteLine(res2.Data);
    else if (res2.Data.GetType() == typeof(List<object>)) foreach (object o in res2.Data!) System.Console.WriteLine(o);
    return res2;
}

void loop()
{
    System.Console.WriteLine("loop");
    string? input = null;
    while (input == null || input == "")
    {
        Console.Write("Q: ");
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
    Console.WriteLine($"A: {(double)processedRes.Data}");
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