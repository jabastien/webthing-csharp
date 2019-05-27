using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test
{
    public class ActionTest
    {
        private readonly Fixture _fixture;
        private readonly JsonDiffPatch _jdp; 

        public ActionTest()
        {
            _fixture = new Fixture();
            _jdp = new JsonDiffPatch();
        }
        
        [Fact]
        public void AsActionDescription()
        {
            var action = new TestAction(_fixture.Create<Thing>(), null)
            {
                HrefPrefix = _fixture.Create<string>()
            };

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""test"": {{
                    ""href"": ""{action.HrefPrefix + action.Href}"",
                    ""timeRequested"": ""{action.TimeRequested:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""status"": ""created""
                }}
            }}");

            JObject description = action.AsActionDescription();

            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsActionDescription_With_Input()
        {
            var action = new TestAction(_fixture.Create<Thing>(), new JObject(
                new JProperty("level", 50),
                new JProperty("duration", 2000)))
            {
                HrefPrefix = _fixture.Create<string>()
            };

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""test"": {{
                    ""input"": {{
                        ""level"": 50,
                        ""duration"": 2000
                    }},
                    ""href"": ""{action.HrefPrefix + action.Href}"",
                    ""timeRequested"": ""{action.TimeRequested:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""status"": ""created""
                }}
            }}");

            JObject description = action.AsActionDescription();

            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public async Task AsActionDescription_With_Pending()
        {
            var action = new TestAction(_fixture.Create<Thing>(), new JObject(
                new JProperty("level", 50),
                new JProperty("duration", 2000)))
            {
                HrefPrefix = _fixture.Create<string>(), 
                Wait = true
            };


            Task performanceTask = action.StartAsync(CancellationToken.None);
            
            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""test"": {{
                    ""input"": {{
                        ""level"": 50,
                        ""duration"": 2000
                    }},
                    ""href"": ""{action.HrefPrefix + action.Href}"",
                    ""timeRequested"": ""{action.TimeRequested:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""status"": ""pending""
                }}
            }}");
            

            JObject description = action.AsActionDescription();

            True(JToken.DeepEquals(json, description));

            action.Wait = false;
            await performanceTask;
            
            json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""test"": {{
                    ""input"": {{
                        ""level"": 50,
                        ""duration"": 2000
                    }},
                    ""href"": ""{action.HrefPrefix + action.Href}"",
                    ""timeRequested"": ""{action.TimeRequested:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""timeCompleted"": ""{action.TimeCompleted:yyyy-MM-ddTHH:mm:ss.fffffffZ}"",
                    ""status"": ""completed""
                }}
            }}");
            
            description = action.AsActionDescription();
            True(JToken.DeepEquals(json, description));
        }
        
        private class TestAction : Mozilla.IoT.WebThing.Action
        {

            internal volatile bool Wait = false;

            public override string Id { get; } = Guid.NewGuid().ToString();
            public override string Name { get; } = "test";

            protected override async Task PerformActionAsync(CancellationToken cancellation)
            {
                while (Wait)
                {
                    await Task.Delay(10);
                }
            }

            public TestAction(Thing thing, JObject input) 
                : base(thing, input)
            {
                
            }
        }
    }
    
    
}