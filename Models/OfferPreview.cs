using System.Globalization;

namespace DealsForYou.Models {
    public class OfferPreview {
        public int ID {
            get; set;
        }

        public int UserId {
            get; set;
        }

        public int CarId {
            get; set;
        }

        public int Amount {
            get; set;
        }

        public int Total {
            get; set;
        }

        public string FirstName {
            get; set;
        }
        public string LastName {
            get; set;
        }

        public string Username {
            get; set;
        }

        public int Year {
            get; set;
        }

        public string Make {
            get; set;
        }

        public string Model {
            get; set;
        }

        public ImageModel Image {
            get; set;
        }
    }
}
