﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<HasChildQueryDescriptor<object>>))]
	public interface IHasChildQuery : IQuery
	{
		[JsonProperty("type")]
		TypeName Type { get; set; }

		[JsonProperty("score_type")]
		[JsonConverter(typeof (StringEnumConverter))]
		ChildScoreType? ScoreType { get; set; }

		[JsonProperty("min_children")]
		int? MinChildren { get; set; }

		[JsonProperty("max_children")]
		int? MaxChildren { get; set; }

		[JsonProperty("query")]
		[JsonConverter(typeof(CompositeJsonConverter<ReadAsTypeJsonConverter<QueryContainerDescriptor<object>>, CustomJsonConverter>))]
		IQueryContainer Query { get; set; }

		[JsonProperty("inner_hits")]
		[JsonConverter(typeof(ReadAsTypeJsonConverter<InnerHits>))]
		IInnerHits InnerHits { get; set; }
	}
	
	public class HasChildQuery : QueryBase, IHasChildQuery
	{
		bool IQuery.Conditionless => IsConditionless(this);
		public TypeName Type { get; set; }
		public ChildScoreType? ScoreType { get; set; }
		public int? MinChildren { get; set; }
		public int? MaxChildren { get; set; }
		public IQueryContainer Query { get; set; }
		public IInnerHits InnerHits { get; set; }

		protected override void WrapInContainer(IQueryContainer c) => c.HasChild = this;
		internal static bool IsConditionless(IHasChildQuery q) => q.Query == null || q.Query.IsConditionless;
	}

	public class HasChildQueryDescriptor<T> 
		: QueryDescriptorBase<HasChildQueryDescriptor<T>, IHasChildQuery>
		, IHasChildQuery where T : class
	{
		bool IQuery.Conditionless => HasChildQuery.IsConditionless(this);
		TypeName IHasChildQuery.Type { get; set; }
		ChildScoreType? IHasChildQuery.ScoreType { get; set; }
		int? IHasChildQuery.MinChildren { get; set; }
		int? IHasChildQuery.MaxChildren { get; set; }
		IQueryContainer IHasChildQuery.Query { get; set; }
		IInnerHits IHasChildQuery.InnerHits { get; set; }

		public HasChildQueryDescriptor()
		{
			((IHasChildQuery)this).Type = TypeName.Create<T>();
		}

		public HasChildQueryDescriptor<T> Query(Func<QueryContainerDescriptor<T>, QueryContainer> selector) => 
			Assign(a => a.Query = selector(new QueryContainerDescriptor<T>()));

		public HasChildQueryDescriptor<T> Type(string type) => Assign(a => a.Type = type);

		public HasChildQueryDescriptor<T> Score(ChildScoreType? scoreType) => Assign(a => a.ScoreType = scoreType);

		public HasChildQueryDescriptor<T> MinChildren(int minChildren) => Assign(a => a.MinChildren = minChildren);

		public HasChildQueryDescriptor<T> MaxChildren(int maxChildren) => Assign(a => a.MaxChildren = maxChildren);

		public HasChildQueryDescriptor<T> InnerHits() => Assign(a => a.InnerHits = new InnerHits());

		public HasChildQueryDescriptor<T> InnerHits(Func<InnerHitsDescriptor<T>, IInnerHits> selector) =>
			Assign(a => a.InnerHits = selector(new InnerHitsDescriptor<T>()));
	}
}