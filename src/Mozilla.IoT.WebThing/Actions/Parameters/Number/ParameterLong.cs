﻿using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.Number
{
    /// <summary>
    /// Represent <see cref="long"/> action parameter.
    /// </summary>
    public readonly struct ParameterLong : IActionParameter
    {
        private readonly long? _minimum;
        private readonly long? _maximum;
        private readonly long? _multipleOf;
        private readonly long[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterLong"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepted null value.</param>
        /// <param name="minimum">The minimum value to be assign.</param>
        /// <param name="maximum">The maximum value to be assign.</param>
        /// <param name="multipleOf">The multiple of value to be assign.</param>
        /// <param name="enums">The possible values this action parameter could have.</param>
        public ParameterLong(bool isNullable, long? minimum, long? maximum, long? multipleOf, long[]? enums)
        {
            CanBeNull = isNullable;
            _minimum = minimum;
            _maximum = maximum;
            _multipleOf = multipleOf;
            _enums = enums;
        }

        /// <inheritdoc/>
        public bool CanBeNull { get; }

        /// <inheritdoc/>
        public bool TryGetValue(JsonElement element, out object? value)
        {
            value = null;
            if (CanBeNull && element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (element.ValueKind != JsonValueKind.Number)
            {
                return false;
            }

            if (!element.TryGetInt64(out var jsonValue))
            {
                return false;
            }

            if (_minimum.HasValue && jsonValue < _minimum.Value)
            {
                return false;
            }

            if (_maximum.HasValue && jsonValue > _maximum.Value)
            {
                return false;
            }

            if (_multipleOf.HasValue && jsonValue % _multipleOf.Value != 0)
            {
                return false;
            }

            if (_enums != null && _enums.Length > 0 && !_enums.Contains(jsonValue))
            {
                return false;
            }

            value = jsonValue;
            return true;
        }
    }
}
