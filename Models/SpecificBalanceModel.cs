namespace DealsForYou.Models {
    public class SpecificBalanceModel {
        public int ID {
            get; set;
        }

        public int Current {
            get; set;
        }

        public int Total {
            get; set;
        }

        public int CarYear {
            get; set;
        }

        public string CarMake {
            get; set;
        }

        public string CarModel {
            get; set;
        }

        public int FileName {
            get; set;
        }

        public string InvName {
            get; set;
        }

        public byte[] ImageData {
            get; set;
        }

        public string FileType {
            get; set;
        }
    }
}
