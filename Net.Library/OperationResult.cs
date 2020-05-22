using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Library
{
    /// <summary>
    /// Result of operation
    /// </summary>
    public enum Result { OK, Fail };


    public class OperationResult
    {
        /// <summary>
        /// Contains result of operation
        /// </summary>
        public Result Result;

        /// <summary>
        /// Contains message with details sent after compliting operation
        /// </summary>
        public string Message;

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="result">Result of operation</param>
        /// <param name="message">Message with details of operation</param>
        public OperationResult(Result result, string message)
        {
            Result = result;
            Message = message;
        }
    }
}
