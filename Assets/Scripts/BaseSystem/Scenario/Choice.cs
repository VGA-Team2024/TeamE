using System;

public class Choice
{
    public string description;
    public Action onSelect;

    public Choice(string description, Action onSelect)
    {
        this.description = description;
        this.onSelect = onSelect;
    }
}
