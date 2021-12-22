using System;
public class Condition
{
    public Func<bool> Predicate { get; private set; }

    /// <summary>
    /// Will check condition, and validate
    /// if Predicate was meet.
    /// </summary>
    public bool WasMet
    {
        get
        {
            return Predicate.Invoke();
        }
    }

    /// <summary>
    /// Create a new condition to be checked
    /// </summary>
    /// <param name="condition"></param>
    public Condition(Func<bool> condition)
    {
        Predicate = condition;
    }
}