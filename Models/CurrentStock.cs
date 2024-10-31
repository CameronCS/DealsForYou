using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DealsForYou.Models {
    public class CurrentStock {
        public int ID {
            get; set;
        }
        public string Make {
            get; set;
        }

        public string Model {
            get; set;
        }

        public int Year {
            get; set;
        }

        public string Vin {
            get; set;
        }

        public string License {
            get; set;
        }

        public int Price {
            get; set;
        }

        public string FPrice {
            get {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = " ";
                var str_price = $"R{this.Price.ToString("N0", culture)}";
                return str_price;
            }
        }
        public ImageModel Image {
            get; set;
        }
    }
}
