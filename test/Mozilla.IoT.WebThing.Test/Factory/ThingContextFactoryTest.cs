using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Builders;
using Mozilla.IoT.WebThing.Events;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Properties;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Factory
{
    public class ThingContextFactoryTest
    {
        private readonly ThingContextFactory _factory;
        private readonly IThingResponseBuilder _response;
        private readonly IActionBuilder _action;
        private readonly IEventBuilder _event;
        private readonly IPropertyBuilder _property;
        
        public ThingContextFactoryTest()
        {
            _response = Substitute.For<IThingResponseBuilder>();
            _event = Substitute.For<IEventBuilder>();
            _property = Substitute.For<IPropertyBuilder>();
            _action = Substitute.For<IActionBuilder>();
            
            _factory = new ThingContextFactory(_event, _property, _response, _action);
            
            _event
                .Build()
                .Returns(new Dictionary<string, EventCollection>());

            _response
                .Build()
                .Returns(new Dictionary<string, object>());

            _action
                .Build()
                .Returns(new Dictionary<string, ActionCollection>());

            _property
                .Build()
                .Returns(new Dictionary<string, IProperty>());
        }

        [Fact]
        public void CreateWithEvent()
        {
            var thing = new EventThing();
            var option = new ThingOption();

            var context = _factory.Create(thing, option);

            context.Should().NotBeNull();
            
            _event
                .Received(1)
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
            
            _response
                .Received(1)
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
            
            _event
                .Received(1)
                .Build();

            _property
                .Received(1)
                .Build();

            _action
                .Received(1)
                .Build();

            _response
                .Received(1)
                .Build();

            _property
                .DidNotReceive()
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<Information>());

            _action
                .DidNotReceive()
                .Add(Arg.Any<MethodInfo>(), Arg.Any<ThingActionAttribute>());

            _action
                .DidNotReceive()
                .Add(Arg.Any<ParameterInfo>(), Arg.Any<Information>());
        }
        
        [Fact]
        public void CreateWithProperty()
        {
            var thing = new PropertyThing();
            var option = new ThingOption();

            var context = _factory.Create(thing, option);

            context.Should().NotBeNull();

            _property
                .Received(1)
                .Build();
            
            _action
                .Received(1)
                .Build();
            
            _response
                .Received(1)
                .Build();
            
            _event
                .Received(1)
                .Build();

            _property
                .Received(5)
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<Information>());
            
            _response
                .Received(5)
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<ThingPropertyAttribute>(), Arg.Any<Information>());
            
            _event
                .DidNotReceive()
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
            
            _action
                .DidNotReceive()
                .Add(Arg.Any<MethodInfo>(), Arg.Any<ThingActionAttribute>());

            _action
                .DidNotReceive()
                .Add(Arg.Any<ParameterInfo>(), Arg.Any<Information>());
        }
        
        [Fact]
        public void CreateWithActions()
        {
            var thing = new ActionThing();
            var option = new ThingOption();

            var context = _factory.Create(thing, option);

            context.Should().NotBeNull();

            _property
                .Received(1)
                .Build();
            
            _action
                .Received(1)
                .Build();
            
            _response
                .Received(1)
                .Build();
            
            _event
                .Received(1)
                .Build();

            _property
                .DidNotReceive()
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<Information>());
            
            _response
                .DidNotReceive()
                .Add(Arg.Any<PropertyInfo>(), Arg.Any<ThingPropertyAttribute>(), Arg.Any<Information>());
            
            _event
                .DidNotReceive()
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
            
            _response
                .DidNotReceive()
                .Add(Arg.Any<EventInfo>(), Arg.Any<ThingEventAttribute>());
            
            _action
                .Received(2)
                .Add(Arg.Any<MethodInfo>(), Arg.Any<ThingActionAttribute>());

            _action
                .Received(2)
                .Add(Arg.Any<ParameterInfo>(), Arg.Any<Information>());
            
            _response
                .Received(2)
                .Add(Arg.Any<MethodInfo>(), Arg.Any<ThingActionAttribute>());

            _response
                .Received(2)
                .Add(Arg.Any<ParameterInfo>(), Arg.Any<ThingParameterAttribute>(), Arg.Any<Information>());
        }
        
        public class EventThing : Thing
        {
            public delegate void NotValidHandler(object? sender);
            public override string Name => "event-thing";

            public event EventHandler<int> Int;
            
            [ThingEvent(Ignore = true)]
            public event EventHandler<string> Ignore;

            public event NotValidHandler NotValid;
        }
        
        public class PropertyThing : Thing
        {
            public override string Name => "property-thing";

            public PropertyThing()
            {
                ReadOnly3 = Guid.NewGuid();
            }
            
            [ThingProperty(Name = "bool2")]
            public bool? Bool { get; set; }

            [ThingProperty(Ignore = true)]
            public Guid Ignore { get; set; }

            [ThingProperty(IsReadOnly = true)] 
            public Guid ReadOnly { get; set; }
            
            public Guid ReadOnly2 { get; private set; }
            public Guid ReadOnly3 { get; }
            
            public string Value { get; set; }
        }

        public class ActionThing : Thing
        {
            public override string Name => "action-thing";

            [ThingAction(Ignore = true)]
            public void Ignore()
            {

            }


            [ThingAction(Name = "test")]
            public void Some()
            {

            }

            public void Some2(
                [ThingParameter(Name = "something")]string @string,
                bool? enable,
                [FromServices]object other,
                CancellationToken cancellationToken)
            {
                
            }
        }
    }
}
