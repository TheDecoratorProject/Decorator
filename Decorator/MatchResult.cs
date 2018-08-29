using System;
using System.Reflection;
using Decorator;
using Decorator.Attributes;

namespace Decorator {
    internal class MatchResult {
        public MatchResult(IMatchClass minfo, Type typ, MethodInfo methInf)
        {
            this.MatchClass = minfo;
            this.Type = typ;
            this.MethodInfo = methInf;
        }

        public IMatchClass MatchClass { get; }
        public Type Type { get; }
        public MethodInfo MethodInfo { get; }
    }
}