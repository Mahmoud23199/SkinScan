using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_BL.Contracts
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
        Task SendEmailKitAsync(string toEmail, string subject, string message);
    }
}
