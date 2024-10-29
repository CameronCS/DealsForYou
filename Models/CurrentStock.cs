using System.ComponentModel.DataAnnotations;

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

        public string Vin {
            get; set;
        }

        public string License {
            get; set;
        }

        public int Price {
            get; set;
        }

        public ImageModel Image {
            get; set;
        }
    }
}
