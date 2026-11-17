using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.Application.Contracts.Common
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T _value { get; set; }
        public Error Error { get; }

        private Result(T value)
        {
            IsSuccess = true;
            _value = value;
            Error = Error.None;
        }

        private Result(Error error)
        {
            IsSuccess = false;
            Error = error;
            _value = default!;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(Error error) => new(error);

        public T Value => IsSuccess
           ? _value!
           : throw new InvalidOperationException("No value for failed result.");
    }

    public sealed class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        private Result()
        {
            IsSuccess = true;
            Error = Error.None;
        }

        private Result(Error error)
        {
            IsSuccess = false;
            Error = error;
        }

        public static Result Success() => new Result();
        public static Result Failure(Error error) => new(error);
    }
}
