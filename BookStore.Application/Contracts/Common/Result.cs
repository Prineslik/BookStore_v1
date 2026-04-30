using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.Application.Contracts.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public Error Error { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = Error.None;
        }

        private Result(Error error)
        {
            IsSuccess = false;
            Error = error;
            Value = default!;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(Error error) => new(error);

        public TResult Match<TResult>(Func<T, TResult> success, Func<Error, TResult> failure)
        {
            return IsSuccess ? success(Value) : failure(Error);
        }
    }
}
