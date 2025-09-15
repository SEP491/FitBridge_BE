namespace FitBridge_Application.Helpers;

public static class EmailContentBuilder
{
    public static string BuildRegistrationConfirmationEmail(string confirmationLink, string fullName)
    {
        return $@"
        <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}

                    .container {{
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        border: 2px solid #000000; /* Black border added */
                        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}

                    .header {{
                        background: linear-gradient(90deg, #4CAF50 0%, #43A047 100%);
                        color: #ffffff;
                        text-align: center;
                        padding: 20px 0;
                    }}

                    .header h1 {{
                        margin: 0;
                        font-size: 24px;
                    }}

                    .content {{
                        padding: 30px 20px;
                        line-height: 1.6;
                        color: #333333;
                    }}

                    .content p {{
                        margin: 16px 0;
                    }}

                    .button {{
                        display: inline-block;
                        padding: 12px 25px;
                        font-size: 16px;
                        color: #ffffff;
                        background-color: #4CAF50;
                        text-decoration: none;
                        border-radius: 6px;
                        transition: background-color 0.3s ease;
                    }}

                    .button:hover {{
                        background-color: #45a049;
                    }}

                    .content p:last-child {{
                        margin-bottom: 0;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">
                        <h1>Welcome to Our Service!</h1>
                    </div>
                    <div class=""content"">
                        <p>Dear <strong>{fullName}</strong>,</p>
                        <p>Thank you for registering with us. Please confirm your account by clicking the button below:</p>
                        <p>
                            <a href=""{confirmationLink}"" class=""button"">
                                Confirm Your Email
                            </a>
                        </p>
                        <p>If you did not register for this account, please ignore this email.</p>
                        <p>Best regards,<br/><strong>Mystery Minis</strong></p>
                    </div>
                </div>
            </body>
        </html>
        ";
    }

    public static string BuildAccountInformationEmail(string account, string password, string loginLink)
    {
        return $@"
        <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}

                    .container {{
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        border: 2px solid #000000;
                        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}

                    .header {{
                        background: linear-gradient(90deg, #2196F3 0%, #1976D2 100%);
                        color: #ffffff;
                        text-align: center;
                        padding: 20px 0;
                    }}

                    .header h1 {{
                        margin: 0;
                        font-size: 24px;
                    }}

                    .content {{
                        padding: 30px 20px;
                        line-height: 1.6;
                        color: #333333;
                    }}

                    .content p {{
                        margin: 16px 0;
                    }}

                    .info-box {{
                        background-color: #f9f9f9;
                        border: 1px solid #ddd;
                        padding: 15px;
                        border-radius: 6px;
                        margin: 20px 0;
                    }}

                    .info-box p {{
                        margin: 8px 0;
                        font-size: 16px;
                    }}

                    .button {{
                        display: inline-block;
                        padding: 12px 25px;
                        font-size: 16px;
                        color: #ffffff;
                        background-color: #2196F3;
                        text-decoration: none;
                        border-radius: 6px;
                        transition: background-color 0.3s ease;
                    }}

                    .button:hover {{
                        background-color: #1976D2;
                    }}

                    .footer {{
                        text-align: center;
                        font-size: 12px;
                        color: #777777;
                        padding: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">
                        <h1>Your Account Information</h1>
                    </div>
                    <div class=""content"">
                        <p>Dear User,</p>
                        <p>Your account has been successfully created. Below are your login details:</p>

                        <div class=""info-box"">
                            <p><strong>Account:</strong> {account}</p>
                            <p><strong>Password:</strong> {password}</p>
                        </div>

                        <p>You can log in to your account using the button below:</p>
                        <p>
                            <a href=""{loginLink}"" class=""button"">Login to Your Account</a>
                        </p>

                        <p>For security reasons, we recommend that you change your password after your first login.</p>
                        <p>Best regards,<br/><strong>Mystery Minis</strong></p>
                    </div>
                    <div class=""footer"">
                        <p>&copy; {DateTime.UtcNow.Year} Mystery Minis. All rights reserved.</p>
                    </div>
                </div>
            </body>
        </html>
        ";
    }

}
