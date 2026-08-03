using System.Collections;
using J2y;

static class Contract
{
    private static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static IEnumerator Delayed(List<string> events)
    {
        events.Add("start");
        yield return 0.1f;
        events.Add("end");
    }

    private static IEnumerator Child(List<string> events)
    {
        events.Add("child:start");
        yield return 0f;
        events.Add("child:end");
    }

    private static IEnumerator Parent(List<string> events)
    {
        events.Add("parent:start");
        yield return Child(events);
        events.Add("parent:end");
    }

    private static IEnumerator Forever()
    {
        while (true)
            yield return 0f;
    }

    public static void Main()
    {
        var delayedEvents = new List<string>();
        var delayed = new JCoroutines();
        delayed.Start(Delayed(delayedEvents));
        delayed.Update(0f);
        Assert(delayedEvents.SequenceEqual(["start"]), "routine must start on the first update");
        delayed.Update(0.05f);
        delayed.Update(0.05f);
        Assert(delayedEvents.SequenceEqual(["start"]), "delay must hold until a later update");
        delayed.Update(0f);
        Assert(delayedEvents.SequenceEqual(["start", "end"]), "routine must resume after delay");
        Assert(!delayed.Running, "completed routine must be removed");

        var nestedEvents = new List<string>();
        var nested = new JCoroutines();
        nested.Start(Parent(nestedEvents));
        for (var i = 0; i < 5 && nested.Running; i++)
            nested.Update(0f);
        Assert(
            nestedEvents.SequenceEqual(["parent:start", "child:start", "child:end", "parent:end"]),
            "parent must resume after its child completes"
        );

        var stopped = new JCoroutines();
        var first = Forever();
        var second = Forever();
        stopped.Start(first);
        stopped.Start(second);
        stopped.Stop(first);
        Assert(stopped.Count == 1, "Stop must remove only the selected routine");
        stopped.StopAll();
        Assert(stopped.Count == 0 && !stopped.Running, "StopAll must clear every routine");

        Console.WriteLine("JCoroutine contracts: 4/4 passed");
    }
}
