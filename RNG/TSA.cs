using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RNG
{
    public class RSA
    {
        byte[] data;
        BigInteger key;
        BigInteger secondKey;
        PostProcessingRNG postProcessing;
        public RSA(PostProcessingRNG postProcessing)
        {
            this.postProcessing = postProcessing;
            data = postProcessing.getRandomData();
            doRSA();
        }
        // data = postProcessing.getRandomData();
        public void doRSA()
        {
            key = initKey(data);
            secondKey = getSecond(data, key);
            Console.WriteLine("done, key= " + key);
            Console.WriteLine("done, key= " + secondKey);
        }
        BigInteger initKey(byte[] data)
        {
            BigInteger dataToCrypt = 0;

            for (int j = 0; j < data.Length / 256; j++)
            {
                string x = "";
                for (int i = j * 255; i < j * 255 + 256; i++)
                {
                    x += data[i];
                }
                dataToCrypt = BigInteger.Parse(x);
                if (IsPrime(dataToCrypt))
                {
                    return dataToCrypt;
                    break;
                }
            }
            return dataToCrypt;
        }
        BigInteger getSecond(byte[] data, BigInteger tocheck)
        {
            BigInteger dataToCrypt = 0;
            for (int j = 0; j < data.Length / 256; j++)
            {
                string x = "";
                for (int i = j * 255; i < j * 255 + 256; i++)
                {
                    x += data[i];
                }
                dataToCrypt = BigInteger.Parse(x);
                if (IsPrime(dataToCrypt) && dataToCrypt != tocheck)
                {

                    return dataToCrypt;
                    break;
                }
            }
            return dataToCrypt;
        }
        private static ThreadLocal<Random> s_Gen = new ThreadLocal<Random>(
     () => {
         return new Random();
     }
   );

        // Random generator (thread safe)
        private static Random Gen
        {
            get
            {
                return s_Gen.Value;
            }
        }
        static bool IsPrime(BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    Gen.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1)
                        return false;
                    if (x == value - 1)
                        break;
                }

                if (x != value - 1)
                    return false;
            }

            return true; //it is probably prime
            /*   var sqrt = Math.Sqrt(number);
               if ((number % 2) == 0) return false;
               for (var i = 3; i <= sqrt; i += 2)
                   if ((number % i) == 0) return false;
               return true;
               if (number <= 1) return false;
               if (number == 2) return true;
               if (number % 2 == 0) return false;
               for (int a = 2; a <= number / 2; a++)
               {
                   if (number % a == 0)
                   {
                       return false;
                   }

               }
               return true;*/
        }

    }
}

