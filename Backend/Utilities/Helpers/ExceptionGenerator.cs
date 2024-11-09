using Backend.Cores.Exceptions;

namespace Backend.Utilities.Helpers
{
    public static class ExceptionGenerator
    {
        /// <summary>
        /// Generate and throw a new <seealso cref="Exception"/> during runtime. 
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="data">The exception optional data</param>
        public static void GenerateGenericException(string message, Dictionary<string, object> data = null!)
        {
            Exception exception = new Exception(message);

            if (data != null)
            {
                foreach (var item in data)
                {
                    exception.Data.Add(item.Key, item.Value);
                }
            }

            throw exception;
        }

        /// <summary>
        /// A simple way to throw a new exception during runtime with optional data.
        /// </summary>
        /// <typeparam name="T">The type of the exception, this must be inherit from System.Exception class</typeparam>
        /// <param name="message">The exception message</param>
        /// <param name="data">The exception optional data</param>
        public static void GenerateGenericException<T>(string message, Dictionary<string, object> data = null!) where T : BaseServiceException
        {
            BaseServiceException exception = new BaseServiceException(message);

            if (data != null)
            {
                foreach (var item in data)
                {
                    exception.AddData(item.Key, item.Value);
                }
            }
            
            throw exception;
        }

        /// <summary>
        /// Generate a standard Exception which can be used for exception handler to catch specific exception.
        /// </summary>
        /// <typeparam name="T">The type of the exception, this must be inherit from System.Exception class</typeparam>
        /// <param name="message">The exception message</param>
        /// <param name="error">The error type</param>
        /// <param name="type">The type of error for returning HTTP status code on Controller layer (if any)
        /// .currently supporting Invalid, Unathorized, Aunauthenticated, </param>
        /// <param name="summary">Summarization of the error.</param>
        /// <param name="detail">Error details, this can be used to dump stack trace and other debugging information.</param>
        /// <param name="value">The value which cause the error</param>
        /// <param name="data">The exception optional data</param>
        public static void GenericServiceException<T>(string message, string error = "Exception", string type = "Invalid", string summary = "", string detail = "", object value = null!, Dictionary<string, object> data = null!) where T : BaseServiceException
        {
            BaseServiceException exception = (T)Activator.CreateInstance(typeof(T), new object[] { message })!  ;

            exception.AddData("error", error);
            exception.AddData("type", type);
            exception.AddData("summary", summary);
            exception.AddData("detail", detail);
            exception.AddData("value", value);


            if (data != null)
            {
                foreach (var item in data)
                {
                    if (!exception.Data.Contains(item))
                    {
                        exception.AddData(item.Key, item.Value);
                    }
                }
            }

            throw exception;
        }
    }
}
