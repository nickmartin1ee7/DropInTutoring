using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text.Json;

var app = new TutorApp();
await app.LaunchAsync();

public class TutorApp
{
    private const string _fileName = "LastSession.json";

    private Session _session;

    public async Task LaunchAsync()
    {
        while (true)
        {
            Console.Clear();

            try
            {
                if (File.Exists(_fileName))
                {
                    _session = JsonSerializer.Deserialize<Session>(await File.ReadAllTextAsync(_fileName)); 
                }
            }
            catch (Exception)
            {
                // Oh well, guess we'll start over
                File.Delete(_fileName);
            }

            if (_session is null)
            {
                _session = await StartNewSessionAsync();
                Console.Clear();
            }

            Console.WriteLine("-- Tutoring Menu --");

            PrintSession();

            await MenuInteractionAsync();
        }
    }

    private Task<Session> StartNewSessionAsync()
    {
        Console.WriteLine("-- New Session --");

        Console.Write("Host's ID: ");
        var hId = Console.ReadLine();

        Console.Write("Host's Name: ");
        var hName = Console.ReadLine();

        var host = new Person(hId, hName);
        return Task.FromResult(new Session(DateTime.Now, host));
    }

    private void PrintSession()
    {
        Console.WriteLine($"Session Id: {_session.Id}");
        Console.WriteLine($"Hosted by: {_session.Host.Name}");
        Console.WriteLine($"Started at: {_session.TimeStamp}");
        Console.WriteLine($"Attendees ({_session.Attendees.Count}):");

        foreach (var attendee in _session.Attendees)
        {
            Console.WriteLine($"\t{attendee}");
        }
    }

    private async Task MenuInteractionAsync()
    {
        Console.WriteLine();
        Console.WriteLine($"Menu Options:");

        var menuOptions = Enum.GetValues<MenuOptions>();
        for (int i = 0; i < menuOptions.Length; i++)
        {
            var item = menuOptions[i];
            Console.WriteLine($"{i + 1}. {item}");
        }

        Console.Write(">> ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out var menuItem)
            && menuItem > 0 && menuItem <= menuOptions.Length)
        {
            var selectedOption = (MenuOptions)(menuItem - 1);

            switch (selectedOption)
            {
                case MenuOptions.CheckIn:
                    await CheckInSubMenuAsync();
                    break;
                case MenuOptions.CheckOut:
                    await CheckOutSubMenuAsync();
                    break;
                case MenuOptions.EndSession:
                    await EndSessionSubMenuAsync();
                    break;
            }
        }
    }

    private Task CheckInSubMenuAsync()
    {
        Console.Write("Student ID: ");
        var aId = Console.ReadLine();

        Console.Write("Student Name: ");
        var aName = Console.ReadLine();

        Person newAttendee;

        if (_session.Attendees.ContainsKey(aId))
        {
            newAttendee = _session.Attendees[aId];
        }
        else
        {
            newAttendee = new Person(aId, aName);
            _session.Attendees.Add(aId, newAttendee);
        }
        
        newAttendee.CheckedIn = true;
        Console.WriteLine($"Checked in: {newAttendee}");
        Console.ReadKey();

        return Task.CompletedTask;
    }

    private Task CheckOutSubMenuAsync()
    {
        Console.Write("Student ID: ");
        var aId = Console.ReadLine();

        if (_session.Attendees.TryGetValue(aId, out var attendee))
        {
            attendee.CheckedIn = false;
            Console.WriteLine($"Checked out: {attendee}");
        }
        else
        {
            Console.WriteLine($"Did not find student with ID: {aId}");
        }

        Console.ReadKey();

        return Task.CompletedTask;
    }

    private Task EndSessionSubMenuAsync()
    {
        File.WriteAllText($"{_session.Id}{_fileName}", JsonSerializer.Serialize(_session));

        Console.WriteLine($"Session {_session.Id} ended");
        _session = null;
        
        Console.ReadKey();

        return Task.CompletedTask;
    }

    private enum MenuOptions
    {
        CheckIn,
        CheckOut,
        EndSession
    }
}