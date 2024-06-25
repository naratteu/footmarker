using Microsoft.CodeAnalysis.Text;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Naratteu.FootMarker;

public static class SelfCodeMarker
{
    public class Collect : ConcurrentQueue<(int line, object? info)>
    {
        public T Mark<T>(T t, [CallerLineNumber] int line = 0) { Enqueue((line, t)); return t; }
    }

    public static Task Run(Func<Collect, Task> func, out IEnumerable<LineInfos> print, [CallerLineNumber] int baseline = 0, [CallerArgumentExpression(nameof(func))] string code = "")
    {
        Collect marks = [];
        print = Print();
        return Catch(func(marks));

        IEnumerable<LineInfos> Print()
        {
            var lk = marks.ToLookup(mark => mark.line - baseline, mark => mark.info);
            foreach (var line in SourceText.From(code).Lines)
                yield return new(line, lk[line.LineNumber]);
        }

        async Task Catch(Task t) { try { await t; } catch (Exception ex) { marks.Mark(ex, baseline); throw; } }
    }

    public static T Mark<T>(this T t, Collect collect, [CallerLineNumber] int line = 0, [CallerArgumentExpression(nameof(t))] string name = "")
    {
        object info = t switch
        {
            Task /**/ task => new TaskInfo(task),
            ValueTask task => new TaskInfo(task.AsTask()),
            _ => $"{name}: {t}",
        };
        collect.Enqueue((line, info));
        return t;
    }
    
    public record LineInfos(TextLine Line, IEnumerable<object?> Infos);
    public class TaskInfo
    {
        public DateTime From { get; } = DateTime.Now;
        public DateTime? Done { get; private set; }
        public TaskInfo(Task t) => (Task = t).GetAwaiter().OnCompleted(() => Done = DateTime.Now);
        public Task Task { get; }

        public override string ToString() => $"{((Done ?? DateTime.Now) - From).TotalSeconds:0.00}sec";
    }
}