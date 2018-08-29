using System;

using Decorator;
using Decorator.Attributes;

namespace Decorator.Tester.MessageTypes {
    [Message("chat")]
    public class Chat {
        [Position(0)]
        public uint PlayerId { get; set; }

        [Position(1)]
        public string ChatMessage { get; set; }
    }

    [Message("chat")]
    public class SendChat {
        [Position(0)]
        public string ChatMessage { get; set; }
    }
}