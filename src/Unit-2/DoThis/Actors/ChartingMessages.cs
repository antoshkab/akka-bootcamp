﻿using Akka.Actor;

namespace ChartApp.Actors
{
    #region Reporting

    public class GatherMetrics { }

    public class Metric
    {
        public string Series { get; }

        public float CounterValue { get; }

        public Metric(string series, float counterValue)
        {
            Series = series;
            CounterValue = counterValue;
        }
    }

    #endregion

    #region Perfomance Counter Managment

    public enum CounterType
    {
        Cpu,
        Memory,
        Disk
    }

    public class SubscribeCounter
    {
        public CounterType CounterType { get; }

        public IActorRef Subscriber { get; }


        public SubscribeCounter(CounterType counterType, IActorRef subscriber)
        {
            CounterType = counterType;
            Subscriber = subscriber;
        }
    }

    public class UnsubscribeCounter
    {
        public CounterType CounterType { get; }

        public IActorRef Subscriber { get; }


        public UnsubscribeCounter(CounterType counterType, IActorRef subscriber)
        {
            CounterType = counterType;
            Subscriber = subscriber;
        }
    }

    #endregion
}