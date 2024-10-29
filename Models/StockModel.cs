using System.ComponentModel.DataAnnotations;

namespace DealsForYou.Models {
    public class StockModel {
        [Required(ErrorMessage = "Make is required.")]
        public string Make {
            get; set;
        }

        [Required(ErrorMessage = "Model is required.")]
        public string Model {
            get; set;
        }

        [Required(ErrorMessage = "VIN is required.")]
        public string Vin {
            get; set;
        }

        [Required(ErrorMessage = "License is required.")]
        public string License {
            get; set;
        }

        [Required(ErrorMessage = "Price is required.")]
        public string Price {
            get; set;
        }

        [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile UploadedFile {
            get; set;
        }
    }
}
