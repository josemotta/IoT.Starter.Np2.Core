using System;
using Microsoft.SPOT;

namespace HttpLibrary
{
    /// <summary>
    /// ErrorEventArgs class for holding server error arguments
    /// </summary>
    public class ErrorEventArgs
    {
        private string event_message;
        /// <summary>
        /// Gets the error message
        /// </summary>
        public string EventMessage
        {
            get { return this.event_message; }
        }
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="event_message">Error message</param>
        public ErrorEventArgs(string event_message)
        {
            this.event_message = event_message;
        }
    }
}
