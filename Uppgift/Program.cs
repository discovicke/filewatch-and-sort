using Uppgift;

public class Program
{
    public static int Main()
    {
        if (!ConfigValidator.IsValidConfigFile("Inställningar.xml"))
            return -1;
        
        

        return 0;
    }
}