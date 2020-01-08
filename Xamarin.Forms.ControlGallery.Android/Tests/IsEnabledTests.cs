﻿using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	[TestFixture]
	public class IsEnabledTests : PlatformTestFixture
	{
		static IEnumerable TestCases
		{
			get
			{
				// Generate IsEnabled = true cases
				foreach (var element in BasicElements)
				{
					var typeName = element.GetType().Name;
					yield return new TestCaseData(element)
						.SetName($"{typeName}_IsEnabled_True")
						.SetCategory(typeName);
				}

				// Generate IsEnabled = false cases
				foreach (var element in BasicElements)
				{
					var typeName = element.GetType().Name;
					yield return new TestCaseData(element)
						.SetName($"{typeName}_IsEnabled_False")
						.SetCategory(typeName);
				}
			}
		}

		[Test, Category("IsEnabled"), TestCaseSource(nameof(TestCases))]
		[Description("VisualElement enabled should match renderer enabled")]
		public void EnabledConsistent(VisualElement element)
		{
			using (var renderer = GetRenderer(element))
			{
				var expected = element.IsEnabled;
				var nativeView = renderer.View;

				ParentView(nativeView);

				// Check the container control
				Assert.That(renderer.View.Enabled, Is.EqualTo(expected));

				// Check the actual control
				var control = GetNativeControl(element);
				Assert.That(control.Enabled, Is.EqualTo(expected));

				UnparentView(nativeView);
			}
		}
	}
}