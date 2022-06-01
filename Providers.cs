using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace RR
{
    abstract class Providers
    {
        abstract public List<PriceItems> Convert(Stream file);
    }

    class Provider
    {
        public List<PriceItems> PriceItemsList;

        public Provider(Providers theone, Stream file)
        {
            PriceItemsList = theone.Convert(file);
        }
    }

    #region Доставим во время

    class DeliverOnTime : Providers
    {
        public string Brand { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        private static MapperConfiguration GetAMConfig()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<DeliverOnTime, PriceItems>()
                    .ForMember("Vendor", opt => opt.MapFrom(src => src.Brand)));
        }

        private sealed class CSVMap : ClassMap<PriceItems>
        {
            public CSVMap()
            {
                Map(x => x.Vendor).Name("Бренд");
                Map(x => x.Number).Name("Каталожный номер");
                Map(x => x.Description).Name("Описание");
                Map(x => x.Price).Convert(row => ConvertPrice(row.Row.GetField(6)));//.TypeConverter<ProvidersDecimalConverter>().Name("Цена, руб.");
                Map(x => x.Count).Convert(row => ConvertCount(row.Row.GetField("Наличие")));
            }

            /// <summary>
            /// Конвертация цены в decimal
            /// </summary>
            /// <param name="fromCSV"></param>
            /// <returns></returns>
            private static decimal ConvertPrice(string fromCSV)
            {
                var numberStyle = NumberStyles.Number;
                if (decimal.TryParse(fromCSV, numberStyle, CultureInfo.InvariantCulture, out var d))
                {
                    return d / 100;
                }
                else
                {
                    return decimal.Zero;
                }
            }

            /// <summary>
            /// Обработка количества
            /// </summary>
            /// <param name="fromCSV"></param>
            /// <returns></returns>
            private static int ConvertCount(string fromCSV)
            {
                int result = 0;
                switch (fromCSV)
                {
                    case var someVal when new Regex(@"^[><].+$").IsMatch(someVal):
                        result = int.Parse(fromCSV.Substring(1, fromCSV.Length - 1));
                        break;
                    case var someVal when new Regex(@"^[0-9]+-[0-9]+$").IsMatch(someVal):
                        result = int.Parse(fromCSV.Split('-').Last());
                        break;
                    default:
                        if (int.TryParse(fromCSV, out var i))
                        {
                            result = i;
                        }
                        else
                        {
                            result = 0;
                        }
                        break;
                }

                return result;
            }
        }

        /// <summary>
        /// Конвертация прайса для загрузки в БД
        /// </summary>
        /// <param name="file">Файл прайса</param>
        /// <returns></returns>
        public override List<PriceItems> Convert(Stream file)
        {
            List<PriceItems> records = new List<PriceItems>();
            file.Position = 0;
            try
            {
                using (TextReader fileReader = new StreamReader(file))
                using (var csv = new CsvReader(fileReader, new CsvConfiguration(CultureInfo.InvariantCulture) { 
                    BadDataFound = null, 
                    Delimiter = ";", 
                    HasHeaderRecord = true
                }))
                {
                    csv.Context.RegisterClassMap<CSVMap>();
                    records = csv.GetRecords<PriceItems>().ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return records;
        }



    }
    #endregion

}
