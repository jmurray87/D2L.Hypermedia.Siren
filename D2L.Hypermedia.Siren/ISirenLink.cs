﻿using System;

namespace D2L.Hypermedia.Siren {

	public interface ISirenLink : ISirenSerializable, IEquatable<ISirenLink>, IComparable<ISirenLink>, IComparable {

		string[] Rel { get; }

		string[] Class { get; }

		Uri Href { get; }

		string Title { get; }

		string Type { get; }

	}

}