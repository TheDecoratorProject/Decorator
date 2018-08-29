using System;

namespace Decorator.Attributes {
    public interface IMessageRequirementAttribute {
        bool MeetsRequirements(Message msg);
    }
}