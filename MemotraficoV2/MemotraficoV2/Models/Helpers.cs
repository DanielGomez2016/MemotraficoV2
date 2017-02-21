using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemotraficoV2.Models
{
    public class Helpers
    {
        public static string LabelSpan(string target, string text)
        {
            return String.Format("<label for='{0}'>{1}<span class='required'>*</span></label>", target, text);
        }
    }
}