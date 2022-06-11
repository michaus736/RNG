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
        PostProcessingRNG postProcessing;
        public RSA(PostProcessingRNG postProcessing)
        {
            this.postProcessing = postProcessing;
            data=postProcessing.GetRandomData();
            doRSA();
        }

        private void doRSA()
        {
            BigInteger key;
            BigInteger secondKey;

        }
        bool isPrime(BigInteger value) { 
        
        }
    }
}
