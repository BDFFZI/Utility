public static class ConsoleFrame
{
    public static void WriteLine(object value, string? tag = null, ConsoleColor? color = null)
    {
        if (tag != null) tag = $"【{tag}】";

        ConsoleColor consoleColor = Console.ForegroundColor;
        Console.ForegroundColor = color ?? consoleColor;
        Console.WriteLine(tag + value);
        Console.ForegroundColor = consoleColor;
    }

    public static void WriteWarning(object value, ConsoleColor? color = null)
    {
        WriteLine(value, "提示", color);
    }

    public static async void Serve(string description, Func<Task> action)
    {
        try
        {
            WriteLine("下方说明中，中括号为所需参数，请通过启动命令传递参数。\n");
            WriteLine("功能说明：\n" + description, "系统");
            WriteLine("\n============开始执行============\n");
            await action();
            WriteLine("\n============执行成功============\n");
        }
        catch (Exception e)
        {
            WriteLine(e, "错误", ConsoleColor.Red);
        }
        finally
        {
            WriteLine("按任意键退出", "系统");
            Console.ReadKey();
        }
    }
}