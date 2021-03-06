using System.Collections.Generic;
using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.Builders
{
    /// <summary>
    /// Create <see cref="ThingResponse"/>.
    /// </summary>
    public interface IThingResponseBuilder
    {
        /// <summary>
        /// Set <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to be set.</param>
        /// <returns></returns>
        IThingResponseBuilder SetThing(Thing thing);

        /// <summary>
        /// Set <see cref="ThingOption"/>
        /// </summary>
        /// <param name="option">The <see cref="ThingOption"/> to be set.</param>
        /// <returns></returns>
        IThingResponseBuilder SetThingOption(ThingOption option);
        
        /// <summary>
        /// Add event.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="eventInfo">Extra information about event</param>
        void Add(EventInfo @event, ThingEventAttribute? eventInfo);

        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="attribute"></param>
        /// <param name="information">The <see cref="Information"/> about property</param>
        void Add(PropertyInfo property, ThingPropertyAttribute? attribute, Information information);
        
        /// <summary>
        /// Add action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="attribute"></param>
        void Add(MethodInfo action, ThingActionAttribute? attribute);
        
        /// <summary>
        /// Add property.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="attribute"></param>
        /// <param name="information">The <see cref="Information"/> about parameter</param>
        void Add(ParameterInfo parameter, ThingParameterAttribute? attribute, Information information);

        /// <summary>
        /// Build the <see cref="ThingResponse"/>.
        /// </summary>
        /// <returns>New <see cref="ThingResponse"/>.</returns>
        Dictionary<string, object?> Build();
    }
}
