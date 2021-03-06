﻿using System;
using System.Linq;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions.Parameters.String
{
    /// <summary>
    /// Represent <see cref="DateTime"/> action parameter.
    /// </summary>
    public readonly struct ParameterDateTime : IActionParameter
    {
        private readonly DateTime[]? _enums;

        /// <summary>
        /// Initialize a new instance of <see cref="ParameterDateTime"/>.
        /// </summary>
        /// <param name="isNullable">If action parameter accepts null value.</param>
        /// <param name="enums">The possible values this action parameter can have.</param>
        public ParameterDateTime(bool isNullable, DateTime[]? enums)
        {
            CanBeNull = isNullable;
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

            if (element.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            if (!element.TryGetDateTime(out var jsonValue))
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
