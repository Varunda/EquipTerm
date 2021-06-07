using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Code {

    public class Quartile {

        /// <summary>
        /// Minimum value
        /// </summary>
        public decimal Min { get; set; }

        /// <summary>
        /// First quartile data point, the 25th % data point
        /// </summary>
        public decimal QuartileOne { get; set; }

        /// <summary>
        /// Median value
        /// </summary>
        public decimal Median { get; set; }

        /// <summary>
        /// 3rd quartile data point, the 25th % data point
        /// </summary>
        public decimal QuartileThree { get; set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public decimal Max { get; set; }

        /// <summary>
        ///     Generate quartile data for a list of decimals. 
        ///     Code from: https://stackoverflow.com/questions/14683467/finding-the-first-and-third-quartiles
        /// </summary>
        /// <param name="data">List of decimals to use</param>
        public static Quartile Create(IEnumerable<decimal> data) {
            Quartile quart = new Quartile();

            int count = data.Count();

            if (count == 0) {
                return quart;
            }

            quart.Min = data.ElementAt(0);
            quart.Max = data.ElementAt(count - 1);

            if (count == 1) {
                quart.QuartileOne = data.ElementAt(0);
                quart.Median = data.ElementAt(0);
                quart.QuartileThree = data.ElementAt(0);

                return quart;
            }

            int middle = count / 2;

            quart.Median = data.ElementAt(middle);

            if (count % 2 == 0) {
                quart.Median = (data.ElementAt(middle - 1) + data.ElementAt(middle)) / 2m;

                int midmid = middle / 2;

                if (midmid % 2 == 0) {
                    quart.QuartileOne = (data.ElementAt(midmid - 1) + data.ElementAt(midmid)) / 2m;
                    quart.QuartileThree = (data.ElementAt(middle + midmid - 1) + data.ElementAt(middle + midmid)) / 2m;
                } else {
                    quart.QuartileOne = data.ElementAt(midmid);
                    quart.QuartileThree = data.ElementAt(middle + midmid);
                }
            } else if ((count - 1) % 4 == 0) {
                int n = (count - 1) / 4;
                quart.QuartileOne = (data.ElementAt(n - 1) * 0.25m) + (data.ElementAt(n) * 0.75m);
                quart.QuartileThree = (data.ElementAt(n * 3) * 0.75m) + (data.ElementAt(3 * n + 1) * 0.25m);
            } else if ((count - 3) % 4 == 0) {
                int n = (count - 3) / 4;
                quart.QuartileOne = (data.ElementAt(n) * 0.75m) + (data.ElementAt(n + 1) * 0.25m);
                quart.QuartileThree = (data.ElementAt(3 * n + 1) * 0.25m) + (data.ElementAt(3 * n + 2) * 0.75m);
            }

            return quart;
        }

    }
}
