using System.Globalization;

namespace DealsForYou.Utils {
    public class Currency {
        public static string GetCurrency(int price) {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = " ";
            var str_price = $"R{price.ToString("N0", culture)}";
            return str_price;
        }
    }
}
