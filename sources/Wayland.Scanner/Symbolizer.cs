using Wayland.Scanner.Types.Xml;
using Enum = System.Enum;

namespace Wayland.Scanner;

public static class Symbolizer
{
    private static readonly string[] ReservedKeywords =
    [
        "event",
        "class",
        "object",
        "interface"
    ];

    public static List<Interface> Process(List<Interface> interfaces)
    {
        foreach (Interface @interface in interfaces)
        {
            @interface.OriginalName = @interface.Name;
            @interface.Name = ProcessSymbol(@interface.Name, true);

            foreach (Event @event in @interface.Events)
            {
                @event.Name = ProcessSymbol(@event.Name, true);
                foreach (Argument argument in @event.Arguments)
                {
                    argument.Name = ProcessSymbol(argument.Name, false);
                    argument.Type = ProcessType(argument.Type);
                }
            }

            foreach (Request request in @interface.Requests)
            {
                request.Name = ProcessSymbol(request.Name, true);
                foreach (Argument argument in request.Arguments)
                {
                    argument.Name = ProcessSymbol(argument.Name, false);
                    argument.Type = ProcessType(argument.Type);
                }
            }

            foreach (Types.Xml.Enum @enum in @interface.Enums)
                @enum.Name = ProcessSymbol(@enum.Name, true);
        }

        return interfaces;
    }

    private static string ProcessSymbol(string symbol, bool capitalizeFirstLetter)
    {
        if (symbol.StartsWith("wl_"))
            symbol = symbol.Remove(0, 3);

        if (capitalizeFirstLetter)
            symbol = char.ToUpper(symbol[0]) + symbol[1..];

        if (symbol.Contains('_'))
        {
            List<char> chars = symbol.ToCharArray().ToList();

            int i = 0;
            while (i < chars.Count)
            {
                if (chars[i] == '_')
                {
                    chars.RemoveAt(i);

                    if (i < chars.Count)
                        chars[i] = char.ToUpper(chars[i]);
                }

                ++i;
            }

            symbol = new string(chars.ToArray());
        }

        if (ReservedKeywords.Contains(symbol))
            symbol = '@' + symbol;

        return symbol;
    }

    private static string ProcessType(string type)
    {
        return type switch
        {
            "array" => "byte[]",
            "int" => "int",
            "uint" => "uint",
            "fixed" => "Fixed",
            "string" => "string",
            "fd" => "Fd",
            "object" => "ObjectId",
            "new_id" => "NewId",
            _ => throw new NotSupportedException($"Type '{type}' is not supported.")
        };
    }
}
