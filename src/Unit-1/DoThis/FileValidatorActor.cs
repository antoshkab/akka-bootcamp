using System;
using System.IO;
using Akka.Actor;

namespace WinTail
{
    public class FileValidatorActor : UntypedActor
    {

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            var reporterActor = Context.ActorSelection("akka://MyActorSystem/user/consoleWriterActor");
            if (string.IsNullOrEmpty(msg))
            {
                reporterActor.Tell(new Messages.NullInputError("Input was blank. Please try again.\n"));
                Sender.Tell(new Messages.ContinueProcessing());
            }
            else
            {
                if (IsFileUri(msg))
                {
                    reporterActor.Tell(new Messages.InputSuccess($"Starting processing for {msg}"));
                    Context.ActorSelection("akka://MyActorSystem/user/tailCoordinatorActor").Tell(new TailCoordinatorActor.StartTail(msg, reporterActor.ResolveOne(TimeSpan.FromSeconds(5)).Result));
                }
                else
                {
                    reporterActor.Tell(new Messages.ValidatingError($"{msg} is not an existing URI on disk."));
                    Sender.Tell(new Messages.ContinueProcessing());
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }


        private static bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}