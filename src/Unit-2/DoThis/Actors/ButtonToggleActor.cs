using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ButtonToggleActor : UntypedActor
    {
        #region Message type

        public class Toggle { }

        #endregion

        private readonly CounterType _myCounterType;
        private bool _isToggleOn;
        private readonly Button _myButton;
        private readonly IActorRef _coordinatorActor;


        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton, CounterType myCounterType, bool isToggleOn = false)
        {
            _coordinatorActor = coordinatorActor;
            _myButton = myButton;
            _isToggleOn = isToggleOn;
            _myCounterType = myCounterType;
        }


        protected override void OnReceive(object message)
        {
            if (message is Toggle && _isToggleOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_myCounterType));

                FlipToggle();
            }
            else if (message is Toggle && !_isToggleOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));

                FlipToggle();
            }
            else
            {
                Unhandled(message);
            }
        }


        private void FlipToggle()
        {
            _isToggleOn = !_isToggleOn;
            _myButton.Text = $"{_myCounterType.ToString().ToUpperInvariant()} ({(_isToggleOn ? "ON" : "OFF")})";
        }
    }
}