using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleSheetsAndCsharp
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Google Sheets API .NET Tuto";
        static readonly string spreadsheetId = "14yddzQ6o8ONZ18UM63oh-X0PJz6p-XgacM3lOuF5IMg";
        static readonly string sheet = "Class Data";
        private static SheetsService service;
        static void Main(string[] args)
        {
            GoogleCredential credential;

            using (var stream =
                new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            DeleteEntry();
        }

        static void ReadEntries()
        {
            var range = $"{sheet}!A2:E";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        static void CreateEntry()
        {
            var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();

            var objectList = new List<object>() {"Hello!", "this", "is", "a", "new", "line"};
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        static void UpdateEntry()
        {
            var range = $"{sheet}!E32";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { "update"};
            valueRange.Values = new List<IList<object>> { objectList };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }

        static void DeleteEntry()
        {
            var range = $"{sheet}!E32";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }
    }
}
