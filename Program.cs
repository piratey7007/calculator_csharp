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
    char? prevChar = null;
    StringBuilder number = null;
    int pCount = 0;

    for (int i = 0; i < characters.Length + 1; i++)
    {
        if (i == characters.Length)
        {
            double finalNumber;
            if (number != null && number.Length != 0)
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
            prevChar = character;
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
                    prevChar = character;
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
            else if (prevType == "number")
            {
                double finalNumber;
                if (number.Length != 0)
                {
                    finalNumber = Convert.ToDouble(number.ToString());
                    parts.Add(finalNumber);
                    number = null;
                }
            }
            parts.Add(character);
            prevType = "operation";
            prevChar = character;
        }
        else if (character == '(' || character == ')')
        {
            // Character is a parenthesis
            if (prevType == "operation" && character == ')') return new Result(1, $"90Invalid parenthesis at character {i + 1}.", null);
            else if (prevType == "number")
            {
                double finalNumber;
                if (number.Length != 0)
                {
                    finalNumber = Convert.ToDouble(number.ToString());
                    parts.Add(finalNumber);
                    number = null;
                }
            }
            else if (prevType == "parenthesis")
            {
                if (prevChar == ')' && character == '(')
                {
                    parts.Add('*');
                }
            }
            parts.Add(character);
            if (character == '(') pCount++;
            else pCount--;
            if (pCount < 0) return new Result(1, $"94Invalid parenthesis at character {i + 1}", null);
            prevType = "parenthesis";
            prevChar = character;
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
                    oParts[i] = (double)previous / (double)next;
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
    Result run()
    {
        while (true)
        {
            int? open = null;
            bool found = false;
            for (int i = 0; i < fParts.Count; i++)
            {
                if (fParts[i] is char p)
                {
                    if (p == '(')
                    {
                        open = i;
                    }
                    else if (p == ')')
                    {
                        found = true;
                        if (open == null) return new Result(1, "Expected opening parenthesis.", null);
                        List<object> range = fParts.GetRange(open.Value + 1, i - open.Value - 1);
                        Result res = handleOperations((List<object>)range!);
                        fParts.RemoveRange(open.Value, i - open.Value + 1);
                        fParts.Insert(open.Value, res.Data);
                        i = open.Value; // Update loop index
                        break; // Break the loop and start over
                    }
                }
            }
            if (!found) break;
        }
        return new Result(0, null, fParts);
    }
    return run();
}


Result processParts(List<object> parts)
{
    Result res = findParentheses(parts);
    Result res2 = handleOperations(res.Data);
    return res2;
}

void loop()
{
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