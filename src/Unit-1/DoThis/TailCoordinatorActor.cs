using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        #region Message types

        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReportActor = reporterActor;
            }


            public IActorRef ReportActor { get; }


            public string FilePath { get; }
        }

        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }


            public string FilePath { get; }
        }

        #endregion

        protected override void OnReceive(object message)
        {
            if (message is StartTail startTail)
            {
                Context.ActorOf(Props.Create(() => new TailActor(startTail.ReportActor, startTail.FilePath)));
            }
        }


        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x =>
            {
                switch (x)
                {
                    case ArithmeticException _:
                        return Directive.Resume;
                    case NotSupportedException _:
                        return Directive.Stop;
                    default:
                        return Directive.Restart;
                }
            });
        }
    }
}