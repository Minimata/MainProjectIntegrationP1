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
        const double alpha = 0.9;
        double expAncientValue;

        public DataSmoother()
        {
            expAncientValue = 0;
        }

        public void updateValue(double d)
        {
            dataToSmooth = d;
        }

        public double UpdateExponential()
        {
            double retour = alpha * expAncientValue + (1 - alpha) * dataToSmooth;
            expAncientValue = dataToSmooth;
            return retour;
        }
    }
}
