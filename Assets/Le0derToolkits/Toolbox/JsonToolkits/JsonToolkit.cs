using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Le0der.Toolbox
{
    public static class JsonToolkit
    {
        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The JSON string representation of the object.</returns>
        public static string SerializeObject<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (JsonSerializationException e)
            {
                var newMessage = $"Failed to serialize object of type {nameof(T)}, exception message: {e.Message}";
                throw new JsonSerializationException(newMessage, e);
            }
        }

        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="Formatting">Indicates how the output should be formatted.</typeparam>
        /// <returns>The JSON string representation of the object.</returns>
        public static string SerializeObject<T>(T obj, Formatting formatting)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, formatting);
            }
            catch (JsonSerializationException e)
            {
                var newMessage = $"Failed to serialize object of type {nameof(T)}, exception message: {e.Message}";
                throw new JsonSerializationException(newMessage, e);
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the JSON string is null or empty.</exception>
        public static T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                var errorMessage = $"Failed to deserialize json to object of type {nameof(T)}. Json string is null or empty";
                LogErrorFormat(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException e)
            {
                var newMessage = $"Failed to deserialize json to object of type {nameof(T)}, exception message: {e.Message}";
                throw new JsonSerializationException(newMessage, e);
            }
        }

        /// <summary>
        /// Logs an error message with the specified format and arguments.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        private static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }
    }
}