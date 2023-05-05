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

Result processParts(List<object> parts)
{
    Result handleOperations(List<object> oParts)
    {
        if (oParts.Count == 1) return new Result(0, null, oParts[0]);
        for (int i = 0; i < oParts.Count; i++)
        {
            if (oParts[i] is char operation)
            {
                if (operation == '*' || operation == '/')
                {
                    if (i == 0 || i == oParts.Count - 1)
                    {
                        return new Result(1, $"Invalid order of operations at character {i + 1}", null);
                    }
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
                        else return new Result(1, $"Invalid order of operations at character {i + 1}", null);
                    }
                    if (operation == '/')
                    {
                        var previous = parts[i - 1];
                        var next = parts[i + 1];
                        if (previous is double && next is double)
                        {
                            parts[i] = (double)previous / (double)next;
                            parts.Remove(previous);
                            parts.Remove(next);
                            i = i - 2;
                        }
                        else return new Result(1, $"Invalid order of operations at character {i + 1}", null);
                    }
                }
            }
        }

        for (int i = 0; i < oParts.Count; i++)
        {
            if (oParts[i] is char operation)
            {
                if (operation == '+')
                {
                    System.Console.WriteLine(oParts[0]);
                    System.Console.WriteLine(oParts[1]);
                    System.Console.WriteLine(oParts[2]);

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
        return new Result(0, null, oParts[0]);
    }
    Result handleParentheses(List<object> pParts)
    {
        Result findParentheses(List<object> pParts)
        {
            int? open = null;
            for (int i = 0; i < pParts.Count; i++)
            {
                if (pParts[i] is char p)
                {
                    if (p == '(')
                    {
                        open = i;
                        pParts.RemoveAt(i);
                    }
                    else if (p == ')')
                    {
                        if (open == null) return new Result(1, "Expected opening parenthesis.", null);
                        pParts.RemoveAt(i);
                        return new Result(2, null, pParts.GetRange(open.Value, i - open.Value + 1));
                    }
                }
            }
            if (open == null) return new Result(0, null, pParts);
            return new Result(1, "Expected closing parenthesis.", null);
        }
        Result fPResult = findParentheses(pParts);
        double status = fPResult.Status;
        while (status == 2)
        {
            Result res = handleOperations((List<object>)fPResult.Data!);
            status = res.Status;
            fPResult = findParentheses(pParts);
        }
        if (fPResult.Status != 1) return fPResult;
        return new Result(0, null, fPResult.Data);
    }
    Result res = handleParentheses(parts);
    return handleOperations(res.Data);
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