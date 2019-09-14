using System.Runtime.Serialization;

namespace VDMP.App.Exception
{
    /**
     * Exception raised when the data that is mirrored between the application and database is inconsistent.
     * Ex: The user attempts to modify a element in the application that is no longer stored in the database.
     */
    public class ApplicationAndDatabaseMismatchException : System.Exception
    {
        public ApplicationAndDatabaseMismatchException()
        {
        }

        public ApplicationAndDatabaseMismatchException(string message) : base(message)
        {
        }

        public ApplicationAndDatabaseMismatchException(string message, System.Exception innerException) : base(message,
            innerException)
        {
        }

        protected ApplicationAndDatabaseMismatchException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}