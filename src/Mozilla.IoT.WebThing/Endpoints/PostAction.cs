﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;
using Mozilla.IoT.WebThing.Descriptor;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class PostAction
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<PostAction>>();

            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");
            logger.LogInformation($"Post Action is calling: [[thing: {thingId}]");

            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);

            if (thing == null)
            {
                logger.LogInformation($"Post Action: Thing not found [[thing: {thingId}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var reader = services.GetRequiredService<IHttpBodyReader>();
            var json = await reader.ReadAsync<IDictionary<string, object>>();

            if (json == null)
            {
                logger.LogInformation("Post Action: Body not found");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (!json.Keys.Any())
            {
                logger.LogInformation("Post Action: Body is empty");
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var result = new Dictionary<string, object>();
            var name = route.GetValue<string>("name");
            if (thing.ActionsTypeInfo.ContainsKey(name) && json.TryGetValue(name, out var token))
            {
                var input = GetInput(token);
                var activator = services.GetService<IActionActivator>();

                var action = activator.CreateInstance(httpContext.RequestServices,
                    thing, name, input as IDictionary<string, object>);

                if (action != null)
                {
                    thing.Actions.Add(action);
                    var descriptor = services.GetService<IDescriptor<Action>>();
                    result.Add(name, descriptor.CreateDescription(action));
                    var block = services.GetService<ChannelWriter<Action>>();
                    await block.WriteAsync(action).ConfigureAwait(false);
                }
            }

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            await writer.WriteAsync(result, httpContext.RequestAborted);
        }

        private static object GetInput(object token)
        {
            if (token is IDictionary<string, object> dictionary && dictionary.ContainsKey("input"))
            {
                return dictionary["input"];
            }

            return new object();
        }
    }
}
