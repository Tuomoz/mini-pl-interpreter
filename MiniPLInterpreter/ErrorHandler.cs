using System.Collections.Generic;

namespace Interpreter
{
    public enum ErrorTypes { LexicalError, SyntaxError, SemanticError };

    public class ErrorHandler
    {
        private Queue<Error> Errors = new Queue<Error>();

        public bool HasErrors { get { return Errors.Count > 0; } }

        public ErrorHandler()
        {
        }

        public void AddError(string errorMessage, ErrorTypes errorType)
        {
            Errors.Enqueue(new Error(errorMessage, errorType));
        }

        public Error[] GetErrors()
        {
            return Errors.ToArray();
        }
    }

    public class Error
    {
        public readonly ErrorTypes ErrorType;
        public readonly string ErrorMessage;

        public Error(string errorMessage, ErrorTypes errorType)
        {
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", ErrorType, ErrorMessage);
        }
    }
}
