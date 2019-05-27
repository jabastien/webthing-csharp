using System.Collections.Generic;

namespace Mozilla.IoT.WebThing
{
    public class MultipleThings : IThingType
    {
        private readonly IList<Thing> _things;
        public MultipleThings(IList<Thing> things, string name)
        {
            _things = things;
            Name = name;
        }

        public Thing this[int index]
        {
            get
            {
                if (index < 0 || index >= _things.Count) 
                {
                    return null;
                }

                return _things[index];
            }
        }

        public IEnumerable<Thing> Things => _things;
        public string Name { get; }
    }
}