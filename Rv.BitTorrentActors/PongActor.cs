using System;
using System.IO;
using System.Reflection;
using Akka.Actor;
using Newtonsoft.Json;

namespace Rv.BitTorrentActors;

internal sealed class PongActor : ReceiveActor
{
    public string Id { get; set; }
    public int Counter { get; set; }

    public PongActor(string id)
    {
        Id = id;

        Receive<string>(ping =>
        {
            Counter += 1;
            Console.WriteLine($"Pong: {Counter}");
            Persist();
        });
    }

    protected override void PreStart()
    {
        if (GetPath().Exists)
            Load();

        base.PreStart();
    }

    private void Persist()
    {
        FileInfo path = GetPath();
        path.Directory!.Create();
        string state = JsonConvert.SerializeObject(this);
        File.WriteAllText(path.FullName, state);
    }

    private void Load()
    {
        string state = File.ReadAllText(GetPath().FullName);
        JsonConvert.PopulateObject(state, this);
    }

    private FileInfo GetPath() =>
        new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, $"Actors/pong-{Id}.json"));

    public static Props Props(string id) =>
        Akka.Actor.Props.Create(() => new PongActor(id));
}
