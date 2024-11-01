namespace DealsForYou.Models {
    public class UserInvoice {
        public int Id {
            get; set;
        }

        public string InvoiceName {
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
