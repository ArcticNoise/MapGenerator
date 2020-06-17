using System;

namespace MapGenerator.ErrorHandling
{
    public interface IErrorHandler
    {
        void HandleError(Exception exception);
    }
}