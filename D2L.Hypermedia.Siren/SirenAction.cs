﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace D2L.Hypermedia.Siren {

	public class SirenAction : ISirenAction {

		private readonly string m_name;
		private readonly string[] m_class;
		private readonly IEnumerable<ISirenField> m_fields;
		private readonly string m_type;
		private readonly string m_title;
		private readonly Uri m_href;
		private readonly string m_method;

		public SirenAction(
			string name,
			Uri href,
			string[] @class = null,
			string method = null,
			string title = null,
			string type = null,
			IEnumerable<ISirenField> fields = null
		) {
			m_name = name;
			m_class = @class ?? new string[0];
			m_method = method;
			m_href = href;
			m_title = title;
			m_type = type;
			m_fields = fields ?? new List<ISirenField>();
		}

		[JsonProperty( "name" )]
		public string Name {
			get { return m_name; }
		}

		[JsonProperty( "class", NullValueHandling = NullValueHandling.Ignore )]
		public string[] Class {
			get { return m_class; }
		}

		[JsonProperty( "method", NullValueHandling = NullValueHandling.Ignore )]
		public string Method {
			get { return m_method; }
		}

		[JsonProperty( "href" )]
		public Uri Href {
			get { return m_href; }
		}

		[JsonProperty( "title", NullValueHandling = NullValueHandling.Ignore )]
		public string Title {
			get { return m_title; }
		}

		[JsonProperty( "type", NullValueHandling = NullValueHandling.Ignore )]
		public string Type {
			get { return m_type; }
		}

		[JsonProperty( "fields", NullValueHandling = NullValueHandling.Ignore )]
		[JsonConverter( typeof(HypermediaFieldConverter) )]
		public IEnumerable<ISirenField> Fields {
			get { return m_fields; }
		}

		public bool Matches(
			out string message,
			string name = null,
			string[] @class = null,
			string method = null,
			Uri href = null,
			string title = null,
			string type = null,
			IEnumerable<ISirenField> fields = null
		) {
			return MatchingHelpers.Matches( name, m_name, out message )
				&& MatchingHelpers.Matches( method, m_method, out message )
				&& MatchingHelpers.Matches( title, m_title, out message )
				&& MatchingHelpers.Matches( type, m_type, out message )
				&& MatchingHelpers.Matches( href, m_href, out message )
				&& MatchingHelpers.Matches( @class, m_class, out message )
				&& MatchingHelpers.Matches( fields, m_fields, out message );
		}

		public bool ShouldSerializeClass() {
			return Class.Length > 0;
		}

		public bool ShouldSerializeFields() {
			return Fields.Any();
		}

		bool IEquatable<ISirenAction>.Equals( ISirenAction other ) {
			if( other == null ) {
				return false;
			}

			bool name = m_name == other.Name;
			bool @class = m_class.OrderBy( x => x ).SequenceEqual( other.Class.OrderBy( x => x ) );
			bool method = m_method == other.Method;
			bool href = m_href == other.Href;
			bool title = m_title == other.Title;
			bool type = m_type == other.Type;
			bool fields = m_fields.OrderBy( x => x ).SequenceEqual( other.Fields.OrderBy( x => x ) );

			return name && @class && method && href && title && type && fields;
		}

		int IComparable<ISirenAction>.CompareTo( ISirenAction other ) {
			if( other == null ) {
				return 1;
			}

			return string.CompareOrdinal( m_name, other.Name );
		}

		int IComparable.CompareTo( object obj ) {
			ISirenAction @this = this;
			return @this.CompareTo( (ISirenAction)obj );
		}

		public override bool Equals( object obj ) {
			ISirenAction action = obj as ISirenAction;
			ISirenAction @this = this;
			return action != null && @this.Equals( action );
		}

		public override int GetHashCode() {
			return m_name.GetHashCode()
				^ string.Join( ",", m_class ).GetHashCode()
				^ m_method?.GetHashCode() ?? 0
				^ m_href?.GetHashCode() ?? 0
				^ m_title?.GetHashCode() ?? 0
				^ m_type?.GetHashCode() ?? 0
				^ m_fields.Select( x => x.GetHashCode() ).GetHashCode();
		}

	}

	public class HypermediaActionConverter : JsonConverter {

		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer ) {
			serializer.Serialize( writer, value );
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) {
			return serializer.Deserialize<SirenAction[]>( reader );
		}

		public override bool CanConvert( Type objectType ) {
			return objectType == typeof( SirenAction );
		}

	}

}