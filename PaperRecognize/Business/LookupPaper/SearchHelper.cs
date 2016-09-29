
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaperRecognize.Business.LookupPaper
{
    public class SearchHelper
    {
        private LevenshteinAnalyser analyser = null;
        private Filter filter = null;
       

        public SearchHelper()
        {            
            analyser = new LevenshteinAnalyser();
            filter = new Filter();
        }

        public string[] Search(string s, string[] items, int number = 1, float threshold = 0.85f)
        {
            if (string.IsNullOrWhiteSpace(s) || items == null || items.Length == 0)
                return new string[0];

            int s_length = s.Length;
            int upper = (int)Math.Ceiling(s_length * (1.0 - threshold) * 1);

            var s_ary = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            
            var filter_str = (from item in items.AsParallel()
                              where filter.filter_str(s, item, upper, s_ary, item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray())
                              select item)
                              .ToArray();
            
            var q = (from item in filter_str.AsParallel()
                     let sim = analyser.GetLikenessValue(s, item) 
                     where sim >= threshold
                     orderby sim descending, item
                     select item
                    ).Take(number);

            return q.ToArray();
        }
        
    }
}
