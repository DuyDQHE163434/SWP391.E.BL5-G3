using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace SWP391.E.BL5.G3.DTOs
{
    public class PayLinkRequest
    {
        [JsonProperty("vnp_Version")]
        public string VnpVersion { get; set; }

        [JsonProperty("vnp_Command")]
        public string VnpCommand { get; set; }

        [JsonProperty("vnp_TmnCode")]
        public string VnpTmnCode { get; set; }

        [JsonProperty("vnp_Amount")]
        public long VnpAmount { get; set; } // Số tiền thanh toán

        [JsonProperty("vnp_BankCode")]
        public string VnpBankCode { get; set; } // Mã phương thức thanh toán

        [JsonProperty("vnp_CreateDate")]
        public string VnpCreateDate { get; set; } // Thời gian phát sinh giao dịch

        [JsonProperty("vnp_CurrCode")]
        public string VnpCurrCode { get; set; } // Đơn vị tiền tệ

        [JsonProperty("vnp_IpAddr")]
        public string VnpIpAddr { get; set; } // Địa chỉ IP

        [JsonProperty("vnp_Locale")]
        public string VnpLocale { get; set; } // Ngôn ngữ giao diện

        [JsonProperty("vnp_OrderInfo")]
        public string VnpOrderInfo { get; set; } // Thông tin mô tả nội dung thanh toán

        [JsonProperty("vnp_OrderType")]
        public string VnpOrderType { get; set; } // Mã danh mục hàng hóa

        [JsonProperty("vnp_ReturnUrl")]
        public string VnpReturnUrl { get; set; } // URL thông báo kết quả giao dịch

        [JsonProperty("vnp_ExpireDate")]
        public string VnpExpireDate { get; set; } // Thời gian hết hạn thanh toán

        [JsonProperty("vnp_TxnRef")]
        public string VnpTxnRef { get; set; } // Mã tham chiếu của giao dịch

        [JsonProperty("vnp_SecureHash")]
        public string VnpSecureHash { get; set; } // Mã kiểm tra (checksum)
    }
}
