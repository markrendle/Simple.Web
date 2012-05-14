using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web
{
    /// <summary>
    /// Represents a Handler that specifies a View for HTML rendering.
    /// </summary>
    /// <remarks>
    /// This interface should only be implemented when a View is used by multiple Handlers and they don't all specify the same Output type, if any.
    /// If a View is used by a single Handler, use the @handler directive at the top of the View code to associate it.
    /// If a View is used by multiple handlers with the same Output type, allow the automatic @model resolution to find the View.
    /// </remarks>
    //public interface ISpecifyView
    //{
    //    string ViewPath { get; }
    //}
}
