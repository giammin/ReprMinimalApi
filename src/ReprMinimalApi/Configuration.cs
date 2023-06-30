using System.Text.RegularExpressions;

namespace ReprMinimalApi;

public static class Configuration
{
    public static bool Debug
#if DEBUG
        => true;
#else
        => false;
#endif
    public static readonly Regex BotRequestRegex = new("^.*(\\.yaml|\\.old|\\.bak|\\.xml|\\.cfg|\\.js|\\.ini|phpinfo|\\.txt|\\.php[\\d]?|\\.?env(\\.|\\/)?\\d?(old|prod|staging)?)$", RegexOptions.Compiled & RegexOptions.IgnoreCase & RegexOptions.Singleline, TimeSpan.FromSeconds(1));
    public static readonly string[] TenHoursOfFun =
    {
        "https://www.youtube.com/watch?v=wbby9coDRCk",
        "https://www.youtube.com/watch?v=nb2evY0kmpQ",
        "https://www.youtube.com/watch?v=z9Uz1icjwrM",
        "https://www.youtube.com/watch?v=Sagg08DrO5U",
        "https://www.youtube.com/watch?v=5XmjJvJTyx0",
        "https://www.youtube.com/watch?v=jScuYd3_xdQ",
        "https://www.youtube.com/watch?v=S5PvBzDlZGs",
        "https://www.youtube.com/watch?v=9UZbGgXvCCA",
        "https://www.youtube.com/watch?v=O-dNDXUt1fg",
        "https://www.youtube.com/watch?v=MJ5JEhDy8nE",
        "https://www.youtube.com/watch?v=VnnWp_akOrE",
        "https://www.youtube.com/watch?v=jwGfwbsF4c4",
        "https://www.youtube.com/watch?v=hGlyFc79BUE",
        "https://www.youtube.com/watch?v=xA8-6X8aR3o",
        "https://www.youtube.com/watch?v=7R1nRxcICeE",
        "https://www.youtube.com/watch?v=bIoe_IR9MB8",
        "https://www.youtube.com/watch?v=sCNrK-n68CM"
    };
}