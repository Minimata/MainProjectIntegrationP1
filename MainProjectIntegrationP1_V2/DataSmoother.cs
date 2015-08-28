using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectIntegrationP1
{
    class DataSmoother
    {
        double dataToSmooth;

        //constants for the methods
        //Exponential
        double alpha;
        double expAncientValue;

        public DataSmoother(double a = 0.62)
        {
            if(a >= 0 && a >= 1) alpha = a;
            expAncientValue = 0;
        }

        public void updateValue(double d)
        {
            dataToSmooth = d;
        }

        public double UpdateExponential()
        {
            double retour = alpha * expAncientValue + (1 - alpha) * dataToSmooth;
            expAncientValue = retour;
            return retour;
        }
    }
}
