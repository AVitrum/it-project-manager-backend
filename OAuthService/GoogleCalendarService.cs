using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace OAuthService;

public class GoogleCalendarService
{
    public static async Task AddEventToCalendarAsync(string accessToken, string summary, string description, string startDateTime, string endDateTime, string timeZone)
    {
        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
        };

        var calendarService = new CalendarService(initializer);

        var eventToInsert = new Event
        {
            Summary = summary,
            Description = description,
            Start = new EventDateTime
            {
                DateTime = DateTime.Parse(startDateTime).AddHours(3),
                TimeZone = timeZone
            },
            End = new EventDateTime
            {
                DateTime = DateTime.Parse(endDateTime).AddHours(3),
                TimeZone = timeZone
            }
        };

        var request = calendarService.Events.Insert(eventToInsert, "primary");
        await request.ExecuteAsync();
    }
}
