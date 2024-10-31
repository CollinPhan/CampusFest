using System.Net;

namespace Backend.Cores.Exceptions
{
    public class BaseServiceException: Exception
    {
        public BaseServiceException(string message): base(message) {}

        public BaseServiceException(string message, Exception innerException): base(message, innerException) { }

        public BaseServiceException AddData(string key, object value)
        {
            this.Data.Add(key, value);
            return this;
        }
    }
}
