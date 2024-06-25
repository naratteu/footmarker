using Naratteu.FootMarker;

_ = Task.Run(async () =>
{
    for (; ; await Task.Delay(3000))
        await (Task = SelfCodeMarker.Run(async c =>
        {
            var now = DateTime.Now.Mark(c);
            var len = (now.Second % 5).Mark(c);
            for(var i = 0; i.Mark(c) < len; i++)
            {
                "Hello,".Mark(c);
                await Task.Delay(1000 / len).Mark(c);
            }
            "World!".Mark(c);
        }, out print));
});
_ = Task.Run(async () =>
{
    for (; ; await Task.Delay(10))
    {
        Console.Clear();
        foreach (var (line, infos) in print ?? [])
        {
            if (infos.Any())
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(line);
                Console.ResetColor();
                Console.WriteLine(" <--" + string.Join(',', infos));
            } else Console.WriteLine(line);

        }
    }
});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.Run();

partial class Program
{
    public static Task Task = Task.CompletedTask;
    public static IEnumerable<SelfCodeMarker.LineInfos>? print;
}