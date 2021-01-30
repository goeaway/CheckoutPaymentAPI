using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Models
{
    /// <summary>
    /// Success/Error response union so we can handle both in requests
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class Either <TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isLeft;

        public TLeft SuccessOrDefault => Match(s => s, e => default);
        public TRight ErrorOrDefault => Match(s => default, e => e);

        public Either(TLeft successResponse)
        {
            _left = successResponse;
            _isLeft = true;
        }

        public Either(TRight errorResponse)
        {
            _right = errorResponse;
            _isLeft = false; // explicit set for easier reading
        }

        // Allows us to return just the success portion of the object when using this type, instead of having to
        // new ApiResonseDTO<TSuccess, TError>();
        public static implicit operator Either<TLeft, TRight>(TLeft success) 
            => new Either<TLeft, TRight>(success);

        // Allows us to return just the error portion of the object when using this type, instead of having to
        // new ApiResonseDTO<TSuccess, TError>();
        public static implicit operator Either<TLeft, TRight>(TRight error) 
            => new Either<TLeft, TRight>(error);

        /// <summary>
        /// Practically merges the success and error branches into a common type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftFunc"></param>
        /// <param name="rightFunc"></param>
        /// <returns></returns>
        public T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
        {
            if(leftFunc == null)
            {
                throw new ArgumentNullException(nameof(leftFunc));
            }

            if(rightFunc == null)
            {
                throw new ArgumentNullException(nameof(rightFunc));
            }

            return _isLeft ? leftFunc(_left) : rightFunc(_right);
        }
    }
}
