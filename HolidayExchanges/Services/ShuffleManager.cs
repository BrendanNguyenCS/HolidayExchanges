using HolidayExchanges.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace HolidayExchanges.Services
{
    public static class ShuffleManager
    {
        /// <summary>
        /// Returns <paramref name="list"/> with its elements shuffled using <see
        /// cref="RNGCryptoServiceProvider"/> and its <see cref="RandomNumberGenerator"/> by getting
        /// a random number and then swapping that random index with a looped value.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CsprngShuffle(this List<User> list)
        {
            if (list == null) throw new ArgumentNullException();

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                User value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Returns <paramref name="list"/> with its elements shuffled using <see cref="Guid"/>.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <remarks>
        /// FYI: This is not a true shuffling as it is considered "ordering by random" because of
        ///      <see cref="Guid.NewGuid"/> only guarantees a unique <see cref="Guid"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static void GuidShuffle(this List<User> list)
        {
            if (list == null) throw new ArgumentNullException();
            list = list.OrderBy(x => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Returns <paramref name="list"/> with its elements shuffled using <see
        /// cref="MoreEnumerable.Shuffle"/> from the <see cref="MoreLinq"/> namespace and NuGet package.
        /// </summary>
        /// <param name="list">The sequence from which to return random elements.</param>
        /// <returns>A <see cref="List{User}"/> with its elements randomized.</returns>
        /// <remarks>
        /// A new random number generator from the <see cref="Random"/> class is created when this
        /// method is called. Only a partial Fisher-Yates shuffle
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>

        public static List<User> LinqShuffle(this List<User> list)
        {
            if (list == null) throw new ArgumentNullException();
            return list.Shuffle().ToList();
        }
    }
}