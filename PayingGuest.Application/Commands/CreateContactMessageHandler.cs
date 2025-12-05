using MediatR;
using PayingGuest.Application.Commands.Contact;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System.Net;
using System.Net.Mail;

namespace PayingGuest.Application.Handlers
{
    public class CreateContactMessageHandler : IRequestHandler<CreateContactMessageCommand, bool>
    {
        private readonly IContactRepository _repository;

        public CreateContactMessageHandler(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new ContactMessage
            {
                YourName = request.Data.YourName,
                EmailAddress = request.Data.EmailAddress,
                Subject = request.Data.Subject,
                Message = request.Data.Message,
                CreatedOn = DateTime.UtcNow
            };

            await _repository.AddAsync(message);

            // Send email to admin
            await SendEmailToAdmin(request);
            return true;
        }
        private async Task SendEmailToAdmin(CreateContactMessageCommand request)
        {
            var adminEmail = "sabarimca7@gmail.com";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dhanalakshmi15201@gmail.com", "lzwg usqw psez jmoz"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                Subject = request.Data.Subject,
                Body = $"Name: {request.Data.YourName}\nEmail: {request.Data.EmailAddress}\n\nMessage:\n{request.Data.Message}",
                From = new MailAddress("dhanalakshmi15201@gmail.com", "Paying Guest Website")
            };

            // ⭐⭐⭐ Show user's email as REPLY-TO ⭐⭐⭐
            mail.ReplyToList.Add(new MailAddress(request.Data.EmailAddress));

            mail.To.Add(adminEmail);

            await smtpClient.SendMailAsync(mail);
            await SendConfirmationEmailToUser(request.Data.EmailAddress, request.Data.YourName);
        }

       // EMAIL TO USER
        private async Task SendConfirmationEmailToUser(string userEmail, string userName)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dhanalakshmi15201@gmail.com", "lzwg usqw psez jmoz"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                Subject = "Thank you for contacting us",
                Body = $"Hi {userName} ,\n\n" +
                       "Thank you for reaching out to us.\n" +
                       "We will contact you shortly.\n\n" +
                       "Regards,\n" +
                       "Karthick PG",
                From = new MailAddress("dhanalakshmi15201@gmail.com", "Paying Guest")
            };

            mail.To.Add(userEmail);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
    
