using System;
using System.Collections.Generic;

public record Session
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime TimeStamp { get; }

    public Person Host { get; }

    public Dictionary<string, Person> Attendees { get; } = new();

    public Session(DateTime startTime, Person host)
    {
        TimeStamp = startTime;
        Host = host;
    }
}
