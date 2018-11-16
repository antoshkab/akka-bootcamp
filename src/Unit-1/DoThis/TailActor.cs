using System.IO;
using System.Text;
using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        #region Message type

        public class FileWrite
        {
            public string FileName { get; }

            public FileWrite(string fileName)
            {
                FileName = fileName;
            }
        }

        public class FileError
        {
            public string FileName { get; }

            public string Reason { get; }

            public FileError(string fileName, string reason)
            {
                FileName = fileName;
                Reason = reason;
            }
        }

        public class InitialRead
        {
            public string FileName { get; }

            public string Text { get; }

            public InitialRead(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }
        }

        #endregion

        private readonly string _filePath;
        private readonly IActorRef _reportActor;
        private FileObserver _observer;
        private Stream _fileStream;
        private StreamReader _fileStreamReader;


        public TailActor(IActorRef reportActor, string filePath)
        {
            _reportActor = reportActor;
            _filePath = filePath;
        }


        protected override void PreStart()
        {
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            _fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, text));
        }


        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }


        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reportActor.Tell(text);
                }
            }
            else if (message is FileError fileError)
            {
                _reportActor.Tell($"Tail error: {fileError.Reason}");
            }
            else if (message is InitialRead initialRead)
            {
                _reportActor.Tell(initialRead.Text);
            }
        }
    }
}