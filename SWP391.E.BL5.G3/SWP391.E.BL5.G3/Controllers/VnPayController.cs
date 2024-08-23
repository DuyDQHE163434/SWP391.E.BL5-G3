using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Models;
using SWP391.E.BL5.G3.ViewModels;

namespace SWP391.E.BL5.G3.Controllers
{
    public class VnPayController : Controller
    {

        private readonly traveltestContext _context;

        public VnPayController(traveltestContext traveltestContext)
        {
            this._context = traveltestContext;
        }

        //[HttpGet("ReturnUrl")]
        public IActionResult ReturnUrl(
            string vnp_TmnCode,
            long vnp_Amount,
            string vnp_BankCode,
            string vnp_BankTranNo,
            string vnp_CardType,
            string vnp_PayDate,
            string vnp_OrderInfo,
            long vnp_TransactionNo,
            string vnp_ResponseCode,
            string vnp_TransactionStatus,
            string vnp_TxnRef,
            string vnp_SecureHashType,
            string vnp_SecureHash) {
            // Tạo một đối tượng model với dữ liệu giả lập hoặc thực tế
            var model = new VnpRedirectModel
            {
                VnpTmnCode = vnp_TmnCode,
                VnpAmount = vnp_Amount,
                VnpBankCode = vnp_BankCode,
                VnpBankTranNo = vnp_BankTranNo,
                VnpCardType = vnp_CardType,
                VnpPayDate = vnp_PayDate,
                VnpOrderInfo = vnp_OrderInfo,
                VnpTransactionNo = vnp_TransactionNo,
                VnpResponseCode = vnp_ResponseCode,
                VnpTransactionStatus = vnp_TransactionStatus,
                VnpTxnRef = vnp_TxnRef,
                VnpSecureHashType = vnp_SecureHashType,
                VnpSecureHash = vnp_SecureHash
            };
            if(vnp_TransactionStatus == "00")
            {
                var bookingId = int.Parse(vnp_OrderInfo);
                var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == bookingId);
                booking.Status = (int)BookingStatusEnum.Confirmed;
                _context.Update(booking);
                _context.SaveChanges();
            }
            // Truyền model đến view
            return View(model);
        }
    }
}
