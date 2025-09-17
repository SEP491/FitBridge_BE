using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Helpers;

public static class EmailContentBuilder
{
    public static string BuildRegistrationConfirmationEmail(string confirmationLink, string fullName)
    {
        return $@"
<!doctype html>
<html lang=""vi"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Xác nhận email - FitBridge</title>
  </head>
  <body style=""margin:0;padding:0;background:#f6f7f9;"">
    <!-- Preheader (ẩn) -->
    <div style=""display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;"">
      Xác nhận email để hoàn tất đăng ký và bắt đầu sử dụng FitBridge.
    </div>

    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background:#f6f7f9;padding:24px 0;"">
      <tr>
        <td align=""center"">
          <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                 style=""width:600px;max-width:600px;background:#ffffff;border:1px solid #eceef1;border-radius:14px;box-shadow:0 6px 24px rgba(0,0,0,0.06);overflow:hidden;"">
            
            <!-- Header -->
            <tr>
              <td style=""background:linear-gradient(90deg,#FF914D 0%,#ED2A46 100%);padding:28px 24px;text-align:center;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#fff;font-size:24px;font-weight:800;letter-spacing:.2px;"">
                  FitBridge
                </div>
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#ffe9e4;font-size:12px;opacity:.9;letter-spacing:.08em;text-transform:uppercase;margin-top:6px;"">
                  Xác nhận email của bạn
                </div>
              </td>
            </tr>

            <!-- Greeting & Body -->
            <tr>
              <td style=""padding:28px 28px 8px 28px;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#0f172a;font-size:16px;line-height:1.7;"">
                  <p style=""margin:0 0 14px 0;"">Xin chào <strong>{fullName}</strong>,</p>
                  <p style=""margin:0 0 14px 0;"">
                    Cảm ơn bạn đã đăng ký tài khoản trên <strong>FitBridge</strong>.  
                    Vui lòng nhấn nút dưới đây để <strong>xác nhận địa chỉ email</strong> và hoàn tất đăng ký.
                  </p>
                </div>
              </td>
            </tr>

            <!-- CTA -->
            <tr>
              <td align=""center"" style=""padding:10px 28px 20px 28px;"">
                <a href=""{confirmationLink}""
                   style=""display:inline-block;background:linear-gradient(90deg,#FF914D 0%,#ED2A46 100%);
                          color:#ffffff;text-decoration:none;font-family:Arial,Helvetica,sans-serif;
                          font-size:16px;font-weight:800;padding:12px 26px;border-radius:10px;"">
                  Xác nhận email
                </a>
              </td>
            </tr>

            <!-- Security note -->
            <tr>
              <td style=""padding:10px 28px 22px 28px;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:14px;line-height:1.7;"">
                  <p style=""margin:12px 0 0 0;"">Nếu bạn không thực hiện đăng ký, vui lòng bỏ qua email này.</p>
                </div>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""background:#fafbfc;padding:16px 24px;text-align:center;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#6b7280;font-size:12px;line-height:1.6;"">
                  © {DateTime.UtcNow:yyyy} FitBridge. Tất cả quyền được bảo lưu.
                </div>
              </td>
            </tr>

          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";
    }


    // public static string BuildGymOwnerInformationEmail(string account, string password, string loginLink)
    // {
    //     return $@"
    //     <html>
    //         <head>
    //             <style>
    //                 body {{
    //                     font-family: Arial, sans-serif;
    //                     background-color: #f4f4f4;
    //                     margin: 0;
    //                     padding: 0;
    //                 }}

    //                 .container {{
    //                     max-width: 600px;
    //                     margin: 40px auto;
    //                     background-color: #ffffff;
    //                     border-radius: 8px;
    //                     border: 2px solid #000000;
    //                     box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    //                     overflow: hidden;
    //                 }}

    //                 .header {{
    //                     background: linear-gradient(90deg, #2196F3 0%, #1976D2 100%);
    //                     color: #ffffff;
    //                     text-align: center;
    //                     padding: 20px 0;
    //                 }}

    //                 .header h1 {{
    //                     margin: 0;
    //                     font-size: 24px;
    //                 }}

    //                 .content {{
    //                     padding: 30px 20px;
    //                     line-height: 1.6;
    //                     color: #333333;
    //                 }}

    //                 .content p {{
    //                     margin: 16px 0;
    //                 }}

    //                 .info-box {{
    //                     background-color: #f9f9f9;
    //                     border: 1px solid #ddd;
    //                     padding: 15px;
    //                     border-radius: 6px;
    //                     margin: 20px 0;
    //                 }}

    //                 .info-box p {{
    //                     margin: 8px 0;
    //                     font-size: 16px;
    //                 }}

    //                 .button {{
    //                     display: inline-block;
    //                     padding: 12px 25px;
    //                     font-size: 16px;
    //                     color: #ffffff;
    //                     background-color: #2196F3;
    //                     text-decoration: none;
    //                     border-radius: 6px;
    //                     transition: background-color 0.3s ease;
    //                 }}

    //                 .button:hover {{
    //                     background-color: #1976D2;
    //                 }}

    //                 .footer {{
    //                     text-align: center;
    //                     font-size: 12px;
    //                     color: #777777;
    //                     padding: 20px;
    //                 }}
    //             </style>
    //         </head>
    //         <body>
    //             <div class=""container"">
    //                 <div class=""header"">
    //                     <h1>Your Account Information</h1>
    //                 </div>
    //                 <div class=""content"">
    //                     <p>Dear User,</p>
    //                     <p>Your account has been successfully created. Below are your login details:</p>

    //                     <div class=""info-box"">
    //                         <p><strong>Account:</strong> {account}</p>
    //                         <p><strong>Password:</strong> {password}</p>
    //                     </div>

    //                     <p>You can log in to your account using the button below:</p>
    //                     <p>
    //                         <a href=""{loginLink}"" class=""button"">Login to Your Account</a>
    //                     </p>

    //                     <p>For security reasons, we recommend that you change your password after your first login.</p>
    //                     <p>Best regards,<br/><strong>Mystery Minis</strong></p>
    //                 </div>
    //                 <div class=""footer"">
    //                     <p>&copy; {DateTime.UtcNow.Year} Mystery Minis. All rights reserved.</p>
    //                 </div>
    //             </div>
    //         </body>
    //     </html>
    //     ";
    // }

    // public static string BuildPtInformationEmail(string account, string password)
    // {
    //     return $@"
    //     <html>
    //         <head>
    //             <style>
    //                 body {{
    //                     font-family: Arial, sans-serif;
    //                     background-color: #f4f4f4;
    //                     margin: 0;
    //                     padding: 0;
    //                 }}

    //                 .container {{
    //                     max-width: 600px;
    //                     margin: 40px auto;
    //                     background-color: #ffffff;
    //                     border-radius: 8px;
    //                     border: 2px solid #000000;
    //                     box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    //                     overflow: hidden;
    //                 }}

    //                 .header {{
    //                     background: linear-gradient(90deg, #2196F3 0%, #1976D2 100%);
    //                     color: #ffffff;
    //                     text-align: center;
    //                     padding: 20px 0;
    //                 }}

    //                 .header h1 {{
    //                     margin: 0;
    //                     font-size: 24px;
    //                 }}

    //                 .content {{
    //                     padding: 30px 20px;
    //                     line-height: 1.6;
    //                     color: #333333;
    //                 }}

    //                 .content p {{
    //                     margin: 16px 0;
    //                 }}

    //                 .info-box {{
    //                     background-color: #f9f9f9;
    //                     border: 1px solid #ddd;
    //                     padding: 15px;
    //                     border-radius: 6px;
    //                     margin: 20px 0;
    //                 }}

    //                 .info-box p {{
    //                     margin: 8px 0;
    //                     font-size: 16px;
    //                 }}

    //                 .button {{
    //                     display: inline-block;
    //                     padding: 12px 25px;
    //                     font-size: 16px;
    //                     color: #ffffff;
    //                     background-color: #2196F3;
    //                     text-decoration: none;
    //                     border-radius: 6px;
    //                     transition: background-color 0.3s ease;
    //                 }}

    //                 .button:hover {{
    //                     background-color: #1976D2;
    //                 }}

    //                 .footer {{
    //                     text-align: center;
    //                     font-size: 12px;
    //                     color: #777777;
    //                     padding: 20px;
    //                 }}
    //             </style>
    //         </head>
    //         <body>
    //             <div class=""container"">
    //                 <div class=""header"">
    //                     <h1>Your Account Information</h1>
    //                 </div>
    //                 <div class=""content"">
    //                     <p>Dear User,</p>
    //                     <p>Your account has been successfully created. Below are your login details:</p>

    //                     <div class=""info-box"">
    //                         <p><strong>Account:</strong> {account}</p>
    //                         <p><strong>Password:</strong> {password}</p>
    //                     </div>

    //                     <p>You can log in to your account using the button below:</p>

    //                     <p>For security reasons, we recommend that you change your password after your first login.</p>
    //                     <p>Best regards,<br/><strong>Mystery Minis</strong></p>
    //                 </div>
    //                 <div class=""footer"">
    //                     <p>&copy; {DateTime.UtcNow.Year} Mystery Minis. All rights reserved.</p>
    //                 </div>
    //             </div>
    //         </body>
    //     </html>
    //     ";
    // }

    public static string BuildGymOwnerInformationEmail(ApplicationUser user, string password, string loginLink)
    {
        return $@"
<!doctype html>
<html lang=""vi"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông tin đăng nhập - FitBridge</title>
  </head>
  <body style=""margin:0;padding:0;background:#f6f7f9;"">
    <!-- Preheader (ẩn) -->
    <div style=""display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;"">
      Tài khoản dành cho Chủ phòng gym đã được tạo. Vui lòng đăng nhập trên web để bắt đầu.
    </div>

    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background:#f6f7f9;padding:24px 0;"">
      <tr>
        <td align=""center"">
          <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                 style=""width:600px;max-width:600px;background:#ffffff;border:1px solid #eceef1;border-radius:14px;box-shadow:0 6px 24px rgba(0,0,0,0.06);overflow:hidden;"">

            <!-- Header -->
            <tr>
              <td style=""background:linear-gradient(90deg,#FF914D 0%,#ED2A46 100%);padding:28px 24px;text-align:center;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#fff;font-size:22px;font-weight:800;letter-spacing:.2px;"">
                  FitBridge
                </div>
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#ffe9e4;font-size:12px;opacity:.95;letter-spacing:.06em;text-transform:uppercase;margin-top:6px;"">
                  Thông tin đăng nhập
                </div>
              </td>
            </tr>

            <!-- Body -->
            <tr>
              <td style=""padding:26px 28px 10px 28px;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#0f172a;font-size:16px;line-height:1.7;"">
                  <p style=""margin:0 0 12px 0;"">Xin chào {user.FullName},</p>
                  <p style=""margin:0 0 12px 0;"">
                    Tài khoản <strong>Chủ phòng gym</strong> của bạn trên <strong>FitBridge</strong> đã được tạo thành công. 
                    Vui lòng sử dụng thông tin dưới đây để đăng nhập trên web:
                  </p>
                </div>
              </td>
            </tr>

            <!-- Credential box -->
            <tr>
              <td style=""padding:0 28px 8px 28px;"">
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                       style=""background:#fafafa;border:1px solid #e8eaed;border-radius:10px;"">
                  <tr>
                    <td style=""padding:14px 16px;font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:15px;line-height:1.7;"">
                      <div style=""margin:0 0 6px 0;""><strong>Tài khoản:</strong> {user.Email} / {user.PhoneNumber}</div>
                      <div style=""margin:0;""><strong>Mật khẩu:</strong> {password}</div>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- CTA: Web login -->
            <tr>
              <td align=""center"" style=""padding:14px 28px 20px 28px;"">
                <a href=""{loginLink}""
                   style=""display:inline-block;background:linear-gradient(90deg,#FF914D 0%,#ED2A46 100%);
                          color:#ffffff;text-decoration:none;font-family:Arial,Helvetica,sans-serif;
                          font-size:16px;font-weight:800;padding:12px 26px;border-radius:10px;"">
                  Đăng nhập trên web
                </a>
              </td>
            </tr>

            <!-- Tips & security -->
            <tr>
              <td style=""padding:0 28px 22px 28px;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:14px;line-height:1.7;"">
                  <p style=""margin:12px 0 0 0;"">Vì lý do bảo mật, vui lòng đổi mật khẩu sau lần đăng nhập đầu tiên.</p>
                </div>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""background:#fafbfc;padding:16px 24px;text-align:center;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#6b7280;font-size:12px;line-height:1.6;"">
                  © {DateTime.UtcNow:yyyy} FitBridge. Tất cả quyền được bảo lưu.
                </div>
              </td>
            </tr>

          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";
    }

    public static string BuildPtInformationEmail(ApplicationUser user, string password, string role)
{
    return $@"
<!doctype html>
<html lang=""vi"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Thông tin đăng nhập - FitBridge ({role})</title>
  </head>
  <body style=""margin:0;padding:0;background:#f6f7f9;"">
    <!-- Preheader (ẩn) -->
    <div style=""display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;"">
      Tài khoản {role} trên FitBridge đã được tạo. Hãy mở ứng dụng FitBridge để đăng nhập.
    </div>

    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background:#f6f7f9;padding:24px 0;"">
      <tr>
        <td align=""center"">
          <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                 style=""width:600px;max-width:600px;background:#ffffff;border:1px solid #eceef1;border-radius:14px;box-shadow:0 6px 24px rgba(0,0,0,0.06);overflow:hidden;"">

            <!-- Header -->
            <tr>
              <td style=""background:linear-gradient(90deg,#FF914D 0%,#ED2A46 100%);padding:28px 24px;text-align:center;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#fff;font-size:22px;font-weight:800;letter-spacing:.2px;"">
                  FitBridge
                </div>
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#ffe9e4;font-size:12px;opacity:.95;letter-spacing:.06em;text-transform:uppercase;margin-top:6px;"">
                  Thông tin đăng nhập ({role})
                </div>
              </td>
            </tr>

            <!-- Body -->
            <tr>
              <td style=""padding:26px 28px 10px 28px;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#0f172a;font-size:16px;line-height:1.7;"">
                  <p style=""margin:0 0 12px 0;"">Xin chào {user.FullName},</p>
                  <p style=""margin:0 0 12px 0;"">
                    Tài khoản <strong>{role}</strong> của bạn trên <strong>FitBridge</strong> đã được tạo thành công. 
                    Vui lòng mở <strong>ứng dụng FitBridge</strong> trên điện thoại để đăng nhập bằng thông tin dưới đây:
                  </p>
                </div>
              </td>
            </tr>

            <!-- Credential box -->
            <tr>
              <td style=""padding:0 28px 8px 28px;"">
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
                       style=""background:#fafafa;border:1px solid #e8eaed;border-radius:10px;"">
                  <tr>
                    <td style=""padding:14px 16px;font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:15px;line-height:1.7;"">
                      <div style=""margin:0 0 6px 0;""><strong>Tài khoản:</strong> {user.Email} / {user.PhoneNumber}</div>
                      <div style=""margin:0;""><strong>Mật khẩu:</strong> {password}</div>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- App reminder (không link) -->
            <tr>
              <td align=""center"" style=""padding:14px 28px 18px 28px;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#6b7280;font-size:12px;line-height:1.6;margin-top:8px;"">
                  (Nếu chưa cài đặt, vui lòng tìm “FitBridge” trên cửa hàng ứng dụng của bạn.)
                </div>
              </td>
            </tr>

            <!-- Tips & security -->
            <tr>
              <td style=""padding:0 28px 22px 28px;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#111827;font-size:14px;line-height:1.7;"">
                  <p style=""margin:12px 0 0 0;"">Vì lý do bảo mật, hãy đổi mật khẩu sau lần đăng nhập đầu tiên.</p>
                </div>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""background:#fafbfc;padding:16px 24px;text-align:center;border-top:1px solid #eef0f2;"">
                <div style=""font-family:Arial,Helvetica,sans-serif;color:#6b7280;font-size:12px;line-height:1.6;"">
                  © {DateTime.UtcNow:yyyy} FitBridge. Tất cả quyền được bảo lưu.
                </div>
              </td>
            </tr>

          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";
}



}
