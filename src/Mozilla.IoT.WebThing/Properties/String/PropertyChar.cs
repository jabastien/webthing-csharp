using System;
using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.String
{
    /// <summary>
    /// Represent <see cref="char"/> property.
    /// </summary>
    public readonly struct PropertyChar : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object?> _getter;
        private readonly Action<Thing, object?> _setter;

        private readonly bool _isNullable;
        private readonly char[]? _enums;
        
        /// <summary>
        /// Initialize a new instance of <see cref="PropertyChar"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        /// <param name="setter">The method to set property.</param>
        /// <param name="isNullable">If property accepted null value.</param>
        /// <param name="enums">The possible values this property could have.</param>
        public PropertyChar(Thing thing, Func<Thing, object?> getter, Action<Thing, object?> setter, 
            bool isNullable, char[]? enums)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _enums = enums;
        }
        
        /// <inheritdoc/>
        public object? GetValue()
            => _getter(_thing);

        /// <inheritdoc/>
        public SetPropertyResult SetValue(JsonElement element)
        {
            if (_isNullable && element.ValueKind == JsonValueKind.Null)
            {
                _setter(_thing, null);
                return SetPropertyResult.Ok;
            }
            
            if (element.ValueKind != JsonValueKind.String)
            {
                return SetPropertyResult.InvalidValue;
            }

            var @string = element.GetString();

            if (@string.Length != 1)
            {
                return SetPropertyResult.InvalidValue;
            }
            
            var value = @string[0];
            
            if (_enums != null && _enums.Length > 0 &&  !_enums.Contains(value))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter(_thing, value);
            return SetPropertyResult.Ok;
        }
    }
}
