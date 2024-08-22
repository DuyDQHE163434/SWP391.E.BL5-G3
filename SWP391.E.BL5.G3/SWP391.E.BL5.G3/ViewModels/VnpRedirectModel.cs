using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace SWP391.E.BL5.G3.ViewModels
{
    public class VnpRedirectModel : PageModel
    {
        [JsonProperty("vnp_TmnCode")]
        public string VnpTmnCode { get; set; } // Mã website của merchant

        [JsonProperty("vnp_Amount")]
        public long VnpAmount { get; set; } // Số tiền thanh toán

        [JsonProperty("vnp_BankCode")]
        public string VnpBankCode { get; set; } // Mã Ngân hàng thanh toán

        [JsonProperty("vnp_BankTranNo")]
        public string VnpBankTranNo { get; set; } // Mã giao dịch tại Ngân hàng (tùy chọn)

        [JsonProperty("vnp_CardType")]
        public string VnpCardType { get; set; } // Loại tài khoản/thẻ khách hàng sử dụng (tùy chọn)

        [JsonProperty("vnp_PayDate")]
        public string VnpPayDate { get; set; } // Thời gian thanh toán (tùy chọn)

        [JsonProperty("vnp_OrderInfo")]
        public string VnpOrderInfo { get; set; } // Thông tin mô tả nội dung thanh toán

        [JsonProperty("vnp_TransactionNo")]
        public long VnpTransactionNo { get; set; } // Mã giao dịch ghi nhận tại hệ thống VNPAY

        [JsonProperty("vnp_ResponseCode")]
        public string VnpResponseCode { get; set; } // Mã phản hồi kết quả thanh toán

        [JsonProperty("vnp_TransactionStatus")]
        public string VnpTransactionStatus { get; set; } // Trạng thái của giao dịch tại VNPAY

        [JsonProperty("vnp_TxnRef")]
        public string VnpTxnRef { get; set; } // Mã tham chiếu giao dịch

        [JsonProperty("vnp_SecureHashType")]
        public string VnpSecureHashType { get; set; } // Loại mã băm sử dụng (tùy chọn)

        [JsonProperty("vnp_SecureHash")]
        public string VnpSecureHash { get; set; } // Mã kiểm tra (checksum)
        public string VnpStatusString { get => VnpTransactionStatus == "00" ? "Giao dịch thành công" : "Giao dịch thất bại"; }
    }
}
