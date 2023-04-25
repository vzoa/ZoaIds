using System.Text.RegularExpressions;
using ZoaIdsBackend.ExternalDataModels;

namespace ZoaIdsBackend.Models;

public class Atis
{
    public string IcaoId { get; set; }
    public AtisType Type { get; set; }
    public char InfoLetter { get; set; }
    public DateTime IssueTime { get; set; }
    public string RawText { get; set; }
    public string WeatherText { get; set; }
    public string UniqueId { get; private set; }

    public static Atis ParseFromClowdAtis(ClowdApiDatis clowdAtis)
    {
        var newAtis = new Atis()
        {
            IcaoId = clowdAtis.Airport,
            Type = ParseAtisType(clowdAtis.Type),
            RawText = clowdAtis.Datis
        };
        newAtis.UniqueId = newAtis.IcaoId + newAtis.Type;

        // Parse letter and time from first sentence and add to new Atis
        var letterTimePattern = @"INFO ([A-Z]) ([0-9]{2})([0-9]{2})Z";
        var matches = Regex.Match(clowdAtis.Datis, letterTimePattern);
        if (matches.Success)
        {
            newAtis.InfoLetter = char.Parse(matches.Groups[1].Value);
            newAtis.IssueTime = new DateTime(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month,
                DateTime.UtcNow.Day,
                int.Parse(matches.Groups[2].Value), // Hours
                int.Parse(matches.Groups[3].Value), // Minutes
                0,
                DateTimeKind.Utc
            );
            if (newAtis.IssueTime > DateTime.UtcNow)
            {
                newAtis.IssueTime -= TimeSpan.FromDays(1);
            }
        }

        // Take 2nd sentence as WX string (by convention)
        newAtis.WeatherText = clowdAtis.Datis.Split(". ")[1];

        return newAtis;
    }

    private static AtisType ParseAtisType(string type)
    {
        return type switch
        {
            "combined" => AtisType.Combined,
            "dep" => AtisType.Departure,
            "arr" => AtisType.Arrival
        };
    }
}

public enum AtisType
{
    Combined,
    Departure,
    Arrival
}