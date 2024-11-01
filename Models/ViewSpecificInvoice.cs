namespace DealsForYou.Models {
    public class ViewSpecificInvoice {
        public int ID {
            get; set;
        }

        public string FileName {
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
