using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutPaymentAPI.Client
{
    public class MonthYear
    {
        public MonthYear(int month, int year)
        {
            Month = month;
            Year = year;
        }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
