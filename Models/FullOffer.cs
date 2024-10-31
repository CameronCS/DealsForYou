using System.Globalization;

namespace DealsForYou.Models {
    public class OUser {
        public int ID {
            get; set;
        }
        public string Username {
            get; set;
        }
        public string FirstName {
            get; set;
        }
        public string LastName {
            get; set;
        }
        public string Email {
            get; set;
        }
        public string Cell {
            get; set;
        }
    }

    public class OOffer {
        public int ID {
            get; set;
        }
        public int UserId {
            get; set;
        }
        public int CarId {
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
        public int OfferAmount {
            get; set;
        }

        public string FAmount {
            get {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = " ";
                var str_price = $"R{this.OfferAmount.ToString("N0", culture)}";
                return str_price;
            }
        }

        public int Months {
            get; set;
        }
        public int Interest {
            get; set;
        }
        public int Monthly {
            get; set;
        }
        public string FMonthly {
            get {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = " ";
                var str_price = $"R{this.Monthly.ToString("N0", culture)}";
                return str_price;
            }
        }
        public int Total {
            get; set;
        }
        public string FTotal {
            get {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = " ";
                var str_price = $"R{this.Total.ToString("N0", culture)}";
                return str_price;
            }
        }
    }

    public class OCar {
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
        } // Use ImageModel here
    }

    public class FullOffer {
        public OUser User {
            get; set;
        }
        public OOffer Offer {
            get; set;
        }
        public OCar Car {
            get; set;
        }
    }
}