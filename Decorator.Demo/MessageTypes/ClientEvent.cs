using Decorator.Attributes;

namespace Decorator.Demo
{
    public class ClientEvent : ClientExistsEvent
    {
        [Required]
        [Position(2)]
        public bool JoinState { get; set; }
    }
}