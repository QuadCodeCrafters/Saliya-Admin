using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApp.MVVM.Model
{
    public class user
    {

        // Method to return sample data
        public List<double> GetLineData()
        {
            return new List<double> { 3, 5, 7, 4, 6, 8, 9 };
           
        }



        public List<double> GetBarData()
        {
            return new List<double> { 50, 30, 80, 60 };
        }
    }
}
