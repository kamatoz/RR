using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.IO;
using CsvHelper;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace RR
{
    public class PriceItems
    {

        public int ID { get; set; }
        private string _vendor;
        public string Vendor
        {
            get => _vendor;
            set
            {
                _vendor = value;
                SearchVendor = new string(value.Where(x => char.IsLetterOrDigit(x)).ToArray()).ToUpper();
            }
        }
        private string _number;
        public string Number
        {
                    get => _number;
                    set
                    {
                        _number = value;
                        SearchNumber = new string (value.Where(x => char.IsLetterOrDigit(x)).ToArray()).ToUpper();
            }
        }
        public string SearchVendor { get; set; }
        public string SearchNumber { get; set; }
        private string _description;
        public string Description 
        {
            get => _description;
            set => _description = value.Length > 512 ? value.Substring(0, 512) : value;
        }
        public decimal Price { get; set; } = 0.0M;
        public int Count { get; set; }

        public PriceItems() { }

    }
}
