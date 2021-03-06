using System;
using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Properties.Number
{
    /// <summary>
    /// Represent <see cref="decimal"/> property.
    /// </summary>
    public readonly struct PropertyDecimal : IProperty
    {
        private readonly Thing _thing;
        private readonly Func<Thing, object?> _getter;
        private readonly Action<Thing, object?> _setter;

        private readonly bool _isNullable;
        private readonly decimal? _minimum;
        private readonly decimal? _maximum;
        private readonly decimal? _multipleOf;
        private readonly decimal[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="PropertyDecimal"/>.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/>.</param>
        /// <param name="getter">The method to get property.</param>
        /// <param name="setter">The method to set property.</param>
        /// <param name="isNullable">If property accepted null value.</param>
        /// <param name="minimum">The minimum value to be assign.</param>
        /// <param name="maximum">The maximum value to be assign.</param>
        /// <param name="multipleOf">The multiple of value to be assign.</param>
        /// <param name="enums">The possible values this property could have.</param>
        public PropertyDecimal(Thing thing, Func<Thing, object?> getter, Action<Thing, object?> setter, 
             bool isNullable, decimal? minimum, decimal? maximum, decimal? multipleOf, decimal[]? enums)
        {
            _thing = thing ?? throw new ArgumentNullException(nameof(thing));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter ?? throw new ArgumentNullException(nameof(setter));
            _isNullable = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
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
            
            if (element.ValueKind != JsonValueKind.Number)
            {
                return SetPropertyResult.InvalidValue;
            }
            
            if(!element.TryGetDecimal(out var value))
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_minimum.HasValue && value < _minimum.Value)
            {
                return SetPropertyResult.InvalidValue;
            }
            
            if (_maximum.HasValue && value > _maximum.Value)
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_multipleOf.HasValue && value % _multipleOf.Value != 0)
            {
                return SetPropertyResult.InvalidValue;
            }

            if (_enums != null && _enums.Length > 0 && !_enums.Contains(value))
            {
                return SetPropertyResult.InvalidValue;
            }

            _setter(_thing, value);
            return SetPropertyResult.Ok;
        }
    }
}
