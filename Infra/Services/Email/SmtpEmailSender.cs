﻿using Domain.Payloads.Email.Shared;
using Infra.Interfaces.Services.Email;
using System.Net.Mail;

namespace Infra.Services.Email;

public class SmtpEmailSender(SmtpClient smtpClient, EmailOptions options) : IEmailSender
{
    public async Task SendEmailAsync<TData>(
        EmailPayload<TData> payload,
        string body,
        CancellationToken cancellationToken = default) where TData : class
    {
        var message = new MailMessage(options.FromAddress, payload.To, payload.Subject, body)
        {   
            IsBodyHtml = true
        };

        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
