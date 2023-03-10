using System;
using Akka.Actor;
using Rv.BitTorrentActors.TrackerClient;

namespace Rv.BitTorrentActors;

public class Program
{
    public static void Main(string[] args)
    {
        TrackerHttpClient s = new TrackerHttpClient();



        ActorSystem actorSystem = ActorSystem.Create("test-system");

        string pongActorId = "ef0baa6c-5e26-46c8-9113-7334d99d554d";
        IActorRef actorRef = actorSystem.ActorOf(PongActor.Props(pongActorId), $"pong-{pongActorId}");

        for (int i = 0; i <= 10000; i++)
        {
            actorRef.Tell("ping", ActorRefs.NoSender);
        }

        Console.ReadLine();
    }
}
