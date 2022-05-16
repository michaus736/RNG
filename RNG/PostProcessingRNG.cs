using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNG
{
    internal class PostProcessingRNG
    {

        /// <summary>
        /// L - CCML size
        /// </summary>
        private static readonly int L = 8;
        /// <summary>
        /// ALPHA - control parameter of chaotic system
        /// </summary>
        private static readonly double ALPHA = 1.99999;
        /// <summary>
        /// EPSILON - coupling constant
        /// </summary>
        private static readonly double EPSILON = 0.05;
        /// <summary>
        /// N - size of required TRNS(bits)
        /// </summary>
        private static readonly int N = 1048576;//??
        /// <summary>
        /// GAMMA - number of iterations
        /// </summary>
        private static readonly int GAMMA = 5;//??
        /// <summary>
        /// pattern to mask 3 least significant bits
        /// </summary>
        private static readonly byte pattern3LSB = 0b00000111;
        /// <summary>
        /// X_INITIAL - 8-item array of CCML initial states
        /// </summary>
        private static readonly double[] X_INITIAL = {
            0.141592,//x_0^0
            0.653589,//x_0^1
            0.793238,//x_0^2
            0.462643,//x_0^3
            0.383279,//x_0^4
            0.502884,//x_0^5
            0.197169,//x_0^6
            0.399375 //x_0^7
        };


        //samples array reference
        byte[] data;


        Extractor extractor;

        public PostProcessingRNG()
        {
            extractor = new();
            data = createSamples();
            var histogram = data.CreateHistogramFromArray();
            histogram.WriteHistogramToFile("ExtractorHistogram.txt");
            Parse();

        }

        public byte[] createSamples()
        {
            extractor.GetSamples();
            extractor.Parse();
            return extractor.getBuffer();
        }

        public void Parse()
        {
            //output list of random 256-bit numbers
            List<byte> O = new List<byte>();
            int n = Extractor.BUFFER_SIZE / 2;
            data = data.Select(x => (byte)(x & pattern3LSB)).ToArray();

            double[] c = new double[8];
            long[] z = new long[8];

            Array.Copy(X_INITIAL, c, X_INITIAL.Length);

            double[,] x = new double[GAMMA, 8];
            for (int i = 0; i < L; i++)
            {
                x[0, i] = c[i];
            }
            int counter = 0;


            while (O.Count < 100000)
            {
                string num = "";

                for (int i = 0; i < L - 1; i++)
                {
                    int t = 0;
                    x[t, i] = ((0.071428571 * data[counter]) + x[t, i]) * 0.666666667;//?? co z t w x_t^i i co to jest y przy r we wzorze
                    counter++;
                    //if(x[i]<0||x[i]>1)throw new ArithmeticException("no zjebało się na amen");
                }


                for (int t = 0; t < GAMMA - 1; t++)
                {
                    for (int i = 0; i <= L - 1; i++)
                    {

                        int index1 = i % L;
                        int index2 = (i - 1) % L;
                        if (i == 0)
                            index2 = i % L;
                        else
                            index2 = (i - 1) % L;
                        x[t + 1, i] = (1 - EPSILON) * TentMap(x[t, i]) + (EPSILON / 2) * TentMap(x[t, index1]) + TentMap(x[t, index2]);
                    }
                }
                for (int i = 0; i <= L - 1; i++)
                {
                    z[i] = (long)(x[GAMMA - 1, i]);
                    x[0, i] = x[GAMMA - 1, i];

                }
                for (int j = 0; j <= ((L / 2) - 1); j++)
                {
                    int f = j + (L / 2);
                    z[j] = z[j] ^ Swap(z[f]);
                }
                for (int i = 0; i < 4; i++)
                {

                    for (int j = 0; j < 4; j++)
                    {
                        /*long afterShift = z[i] >>= (8 * j);
                        long mask = 0b11111111;
                        byte temp = (byte)(afterShift & mask);*/

                        long table = z[i] >>= 8 * j;
                        byte temp = (byte)(table % 255);
                       temp =(byte) HashingCode(temp);
                        O.Add(temp);

                    }

                    //  O.Add();
                }
            }
            var histogram = O.CreateHistogramFromArray();
            histogram.WriteHistogramToFile("PreprocessingHistogram.txt");
        }

        private long HashingCode(long z)
        {
            long value = 56732344513432;

            for (int i = 0; i < 32; i++)
            {
                z = z * value + i + value * z - z * i;
                value = z % (i + 1);
            }
            for (int i = 0; i < z % 8; i++)
            {

                z = (i+15) * value + z*(value%(i+3));
                z =-value;
            }
            return z;
        }
        private long Swap(long v)
        {
            byte[] bytes = BitConverter.GetBytes(v);

            Array.Reverse(bytes);
            v = BitConverter.ToInt64(bytes, 0);
            return v;
        }

        double TentMap(double x)
        {
            if (x < 0.5)
                return ALPHA * x;
            else
                return ALPHA * (1 - x);
        }

    }
}
