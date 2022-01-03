using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InboxManager : Singleton<InboxManager>
{
    public Queue<InboxData> Inboxes { get; private set; } = new Queue<InboxData>();

    public static void RetrieveInboxesFromPlayerServer()
    {

    }
}
