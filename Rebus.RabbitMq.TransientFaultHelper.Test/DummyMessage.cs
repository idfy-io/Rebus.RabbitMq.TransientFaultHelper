using System;

namespace Rebus.RabbitMq.TransientFaultHelper.Test
{
    public class DummyMessage
    {
        public string Title { get; set; }

        public int Count { get; set; }
        public Guid Id { get; set; }
    }
}