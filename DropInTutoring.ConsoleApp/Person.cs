using System;

public class Person
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public bool CheckedIn { get; set; }

    public Person() {}

    public Person(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"Id: {Id}; Name: {Name}; Checked In: {CheckedIn}";
    }
}
