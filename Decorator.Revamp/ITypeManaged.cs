using System;
using System.Collections.Generic;

namespace Decorator {

	public interface IManagedType {
		uint Position { get; }

		TypeRequiredness State { get; }

		Dictionary<uint, Type> PositionTypes { get; }
	}

	public enum TypeRequiredness {
		Required, Optional
	}

	public class ManagedType : IManagedType {

		public ManagedType(uint pos, bool req) {
			this.Position = pos;

			this.State = req ? TypeRequiredness.Required : TypeRequiredness.Optional;

			this.PositionTypes = new Dictionary<uint, Type>();
		}

		public uint Position { get; }

		public TypeRequiredness State { get; }

		public Dictionary<uint, Type> PositionTypes { get; }
	}
}