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

    Array characters = input.Split("");
    List<object> parts = new List<object>();
    string? prevType = null;
    StringBuilder number = new StringBuilder();

    for (int i = 0; i < characters.Length; i++)
    {
        char character = (char)characters.GetValue(i)!;
        if (character >= '0' && character <= '9')
        {
            if (prevType == "number")
            {
                number = new StringBuilder();
                number.Append(character);
            }
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
                }
                else
                {
                    return new Result(1, "Invalid operation. Please enter a valid operation.", null);
                }
                prevType = "number";
            }
            else
            {
                parts.Append(character);
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
            Console.WriteLine("Invalid character.");

            return new Result(1, "Invalid character.", null);
        }
    }
    return new Result(0, null, parts);
}

Result processParts(List<object> parts)
{
    return new Result(0, null, parts);
}

void loop()
{
    string input = null;
    while (input == null || input == "")
    {
        input = Console.ReadLine();
    }
    var res = handleInput(input);
    if (res.Status != 0)
    {
        Console.WriteLine("Try again.");
        loop();
    }
    if (res.Data is not List<object>)
    {

    }
};


loop();

public class Result
{
    public Result(int? status, string? message, dynamic? data)
    {
        if (status != null) Status = (int)status;
        if (message != null) Message = message;
        if (data != null) Data = data;
    }
    public int Status { get; set; }
    public string? Message { get; set; }
    public dynamic? Data { get; set; }
}