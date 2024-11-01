namespace DealsForYou.Models {
    public class AddedInvoices {
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
        public string InvName {
            get; set;
        }
        public byte[] File {
            get; set;
        }
        public string FileType {
            get; set;
        }
    }
}
