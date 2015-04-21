#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Network;

#endregion

namespace RandomUtilities
{
    public sealed class RNG
    {
        private uint z;
        private uint w;
        private uint jsr;
        private uint jcong;
        private uint x = 0;
        private uint y = 0;
        private byte c = 0;
        private uint[] t;

        private static readonly RNG _rng = Create();

        private static RNG Create()
        {
            Random r = new Random();
            return new RNG((uint)r.Next(), (uint)r.Next(), (uint)r.Next(), (uint)r.Next());
        }

        private RNG(uint i1, uint i2, uint i3, uint i4)
        {
            z = i1;
            w = i2;
            jsr = i3;
            jcong = i4;

            t = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                t[i] = KISS();
            }
        }

        // This returns an integer in the range of a uint
        public static double RandomInt()
        {
            return _rng.KISS();
        }

        // This returns a floating point value in the range 0.0 - 1.0
        public static double RandomFloat()
        {
            return _rng.UNI();
        }

        public static double RandomFloat(double a, double b)
        {
            return Math.Round(_rng.UNI() * (b - a) + a,3);
        }

        // Returns 0 or 1 with equal likelihood
        public static double RandomBinary()
        {
            return _rng.UNI() < 0.5 ? 0.0 : 1.0;
        }

        // Returns 1 with probability p
        public static double RandomBinary(double p)
        {
            return _rng.UNI() < p ? 1.0 : 0.0;
        }

        // Returns a random integer
        public static int RandomInt(double max)
        {
            return (int)Math.Floor(RandomFloat() * (max+1));
        }
        public static double RandomInt(double min, double max)
        {
            return RandomInt(max - min) + min;
           // Random r = new Random();
            //return r.Next()%(max-min+1)+min;
        }

        // Returns an element from a collection
        public static T Choose<T>(IIndexable<T> collection)
        {
            return collection[RandomInt(collection.Length)];
        }
        public static T Choose<T>(T[] collection)
        {
            return collection[RandomInt(collection.Length)];
        }
        
        // These methods implement the above
        private uint znew()
        {
	        return ((z = 36969 * (z & 65535) + (z >> 16)) << 16);
        }
        private uint wnew()
        {
	        return ((w = 18000 * (w & 65535) + (w >> 16)) & 65535);
        }
        private uint MWC()
        {
	        return znew() + wnew();
        }
        private uint SHR3()
        {
	        return jsr = (jsr = (jsr = jsr ^ (jsr << 17)) ^ (jsr >> 13)) ^ (jsr << 5);
        }
        private uint CONG()
        {
	        return jcong = 69069 * jcong + 1234567;
        }
        private uint KISS()
        {
	        return (MWC() ^ CONG()) + SHR3();
        }
        private uint LFIB4()
        {
	        return t[c] = t[c] + t[c + 58] + t[c + 119] + t[++c + 178];
        }
        private uint SWB()
        {
	        return t[c + 237] = (x = t[c + 15]) - (y = t[++c] + (x < y ? 1U : 0U));
        }
        private double UNI()
        {
	        return KISS() * 2.328306e-10;
        }
        private double VNI()
        {
	        return (int)(KISS()) * 4.656613e-10;
        }

    }
}
