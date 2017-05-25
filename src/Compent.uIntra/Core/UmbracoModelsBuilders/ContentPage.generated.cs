//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.5.96
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;
using Umbraco.Web;

namespace Compent.uIntra.Core.UmbracoModelsBuilders
{
	/// <summary>Content Page</summary>
	[PublishedContentModel("contentPage")]
	public partial class ContentPage : BasePage, INavigationComposition
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "contentPage";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public ContentPage(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<ContentPage, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Grid
		///</summary>
		[ImplementPropertyType("grid")]
		public Newtonsoft.Json.Linq.JToken Grid
		{
			get { return this.GetPropertyValue<Newtonsoft.Json.Linq.JToken>("grid"); }
		}

		///<summary>
		/// Is hide from Left Navigation
		///</summary>
		[ImplementPropertyType("isHideFromLeftNavigation")]
		public bool IsHideFromLeftNavigation
		{
			get { return NavigationComposition.GetIsHideFromLeftNavigation(this); }
		}

		///<summary>
		/// Is hide from Sub Navigation
		///</summary>
		[ImplementPropertyType("isHideFromSubNavigation")]
		public bool IsHideFromSubNavigation
		{
			get { return NavigationComposition.GetIsHideFromSubNavigation(this); }
		}

		///<summary>
		/// Navigation Name
		///</summary>
		[ImplementPropertyType("navigationName")]
		public string NavigationName
		{
			get { return NavigationComposition.GetNavigationName(this); }
		}
	}
}
