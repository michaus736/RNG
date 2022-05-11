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
        private static readonly int N = (Extractor.BUFFER_SIZE / 2) / 8 * 256;//??
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
            List<decimal> O = new List<decimal>();
            int n = Extractor.BUFFER_SIZE / 2;

            data = data.Select(x => (byte)(x & pattern3LSB)).ToArray();

            double[] x=new double[8];
            ulong[] z = new ulong[8];
            Array.Copy(X_INITIAL,x,X_INITIAL.Length);

            int counter = 0;

            while (O.Count < N)
            {
                string num = "";

                for (int i = 0; i < L; i++)
                {
                    x[i] = ((0.071428571 * data[counter]) + x[i]) * 0.666666667;//?? co z t w x_t^i i co to jest y przy r we wzorze
                    counter++;
                    if(x[i]<0||x[i]>1)throw new ArithmeticException("no zjebało się na amen");
                }


                for(int t = 0; t < GAMMA-1; t++)
                {
                    for(int i = 0; i < L-1; i++)
                    {
                        x[i] = (1 - EPSILON) * TentMap(x[i]) + (EPSILON / 2) * TentMap(x[i % L]) + TentMap(x[(i - 1)%L]);
                    }
                }
                for (int i = 0; i < L - 1; i++) 
                {
                     string value= x[i].ToString();
                    z[i] = UInt64.Parse(value);

                }
                for (int j = 0; j <(L/2-1); j++) 
                {
                    z[j] = z[j] ^ swap(z[j+(L/2)]);
                }
                for (int j = 0; j <= 3; j++)
                {
                   string t= z[j].toString();
                    num += t;

                }
                int x = num.toInt();
                O.Add(x);
            }
        }

        private ulong swap(ulong v)
        {
            string tmp="";
           var z= v.ToString();
            ulong x;
            for (int i = 0; i <= 31; i++) 
            {
                tmp += z[i];
            }
            for (int i = 63; i >= 32; i--)
            {
                tmp += z[i];
            }
            x = UInt64.Parse(tmp);

            return x;
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
