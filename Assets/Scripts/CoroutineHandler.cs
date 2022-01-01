using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This sealed class assisted other classes that do not inherit from Monobehaviour
/// and need to run a coroutine. It's highly recommended to not use this class's methods unless you have to.
/// </summary>
public sealed class CoroutineHandler : Singleton<CoroutineHandler>
{
    static Dictionary<IEnumerator, int> CoroutineLog = new Dictionary<IEnumerator, int>();
    public static void Execute(IEnumerator enumerator)
    {
        try
        {
            if (Instance != null && enumerator != null && !IsNull && !CoroutineLog.ContainsKey(enumerator))
            {
                Instance.StartCoroutine(enumerator);
                CoroutineLog.Add(enumerator, enumerator.GetHashCode());
            }
        }
        catch { return; }
    }

    public static void Halt(IEnumerator enumerator)
    {
        try
        {
            if (Instance != null && enumerator != null && !IsNull && CoroutineLog.ContainsKey(enumerator))
            {
                Instance.StopCoroutine(enumerator);
                CoroutineLog.Remove(enumerator);
            }
        }
        catch
        {
            return;
        }
        //Otherwise, there's no existing CoroutineEntry that we can stop, or that this object is null.
    }



    public static void ClearRoutines()
    {
        CoroutineLog.Clear();
        Instance.StopAllCoroutines();
    }
}


public struct CoroutineEntry
{
    public int EntryID { get; }
    public IEnumerator Coroutine { get; }
    
    public CoroutineEntry(IEnumerator coroutine, int entryID)
    {
        Coroutine = coroutine;
        EntryID = entryID;
    }
}