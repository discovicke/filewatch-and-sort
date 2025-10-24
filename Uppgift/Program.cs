using Uppgift;

public class Program
{
    public static int Main()
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, "Inställningar.xml");

        int configReturn = ConfigValidator.ÄrOgiltigKonfiguration(xmlPath);

        if (configReturn != 0)
        {
            return -1;
        }

        return 0;
    }
}