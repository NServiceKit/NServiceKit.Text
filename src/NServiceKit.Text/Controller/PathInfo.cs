//
// https://github.com/NServiceKit/NServiceKit.Text
// NServiceKit.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.Text.Controller
{
    /// <summary>Class to hold.</summary>
	public class PathInfo
	{
        /// <summary>Gets the name of the controller.</summary>
        /// <value>The name of the controller.</value>
		public string ControllerName { get; private set; }

        /// <summary>Gets the name of the action.</summary>
        /// <value>The name of the action.</value>
		public string ActionName { get; private set; }

        /// <summary>Gets the arguments.</summary>
        /// <value>The arguments.</value>
		public List<string> Arguments { get; private set; }

        /// <summary>Gets options for controlling the operation.</summary>
        /// <value>The options.</value>
		public Dictionary<string, string> Options { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Controller.PathInfo class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="arguments"> The arguments.</param>
		public PathInfo(string actionName, params string[] arguments)
			: this(actionName, arguments.ToList(), null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.Text.Controller.PathInfo class.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="arguments"> The arguments.</param>
        /// <param name="options">   Options for controlling the operation.</param>
		public PathInfo(string actionName, List<string> arguments, Dictionary<string, string> options)
		{
			ActionName = actionName;
			Arguments = arguments ?? new List<string>();
			Options = options ?? new Dictionary<string, string>();
		}

        /// <summary>Gets the first argument.</summary>
        /// <value>The first argument.</value>
		public string FirstArgument
		{
			get
			{
				return this.Arguments.Count > 0 ? this.Arguments[0] : null;
			}
		}

        /// <summary>Gets argument value.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="index">Zero-based index of the.</param>
        /// <returns>The argument value.</returns>
		public T GetArgumentValue<T>(int index)
		{
			return TypeSerializer.DeserializeFromString<T>(this.Arguments[index]);
		}

        /// <summary>
        /// Parses the specified path info.
        /// e.g.
        ///     MusicPage/arg1/0/true?debug&amp;showFlows=3 => PathInfo
        ///         .ActionName = 'MusicPage'
        ///         .Arguments = ['arg1','0','true']
        ///         .Options = { debug:'True', showFlows:'3' }
        /// </summary>
        /// <param name="pathUri">The path url.</param>
        /// <returns>A PathInfo.</returns>
		public static PathInfo Parse(string pathUri)
		{
			var actionParts = pathUri.Split(new[] { "://" }, StringSplitOptions.None);
			var controllerName = actionParts.Length == 2
									? actionParts[0]
									: null;

			var pathInfo = actionParts[actionParts.Length - 1];

			var optionMap = new Dictionary<string, string>();

			var optionsPos = pathInfo.LastIndexOf('?');
			if (optionsPos != -1)
			{
				var options = pathInfo.Substring(optionsPos + 1).Split('&');
				foreach (var option in options)
				{
					var keyValuePair = option.Split('=');

					optionMap[keyValuePair[0]] = keyValuePair.Length == 1
													? true.ToString()
													: keyValuePair[1].UrlDecode();
				}
				pathInfo = pathInfo.Substring(0, optionsPos);
			}

			var args = pathInfo.Split('/');
			var pageName = args[0];

			return new PathInfo(pageName, args.Skip(1).ToList(), optionMap) {
				ControllerName = controllerName
			};
		}
	}
}