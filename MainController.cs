using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System.IO;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace RR
{
    public class MainController : Controller
    {
        private readonly PostrgresContext _context;

        public MainController(PostrgresContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReadEmail(int provider, string email_server, int? email_port, bool? email_ssl, string email_login, string email_pass)
        {
            using (var client = new ImapClient())
            {
                var mail_server = email_server ?? "imap.gmail.com";
                var mail_port = email_port ?? 993;
                var mail_ssl = email_ssl ?? true;
                var mail_login = email_login ?? "chayukov.rr@gmail.com";
                var mail_pass = email_pass ?? "njjzhacflwnbzdep";
                // В идеале хранить настройки каждого ящика в БД, а не передавать каждый раз
                await client.ConnectAsync(mail_server, mail_port, mail_ssl);
                await client.AuthenticateAsync(mail_login, mail_pass);
                IMailFolder inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);
                foreach (var uid in inbox.Search(SearchQuery.NotSeen))
                {
                    var message = inbox.GetMessage(uid);
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                    var attachs = message.Attachments.Where(x => (x.ContentDisposition?.FileName ?? x.ContentType.Name).EndsWith(".csv"));
                    foreach (var att in attachs)
                    {
                        MemoryStream ms = new MemoryStream();
                        if (att is MessagePart)
                        {
                            var rfc822 = (MessagePart)att;
                            rfc822.Message.WriteTo(ms);
                        }
                        else
                        {
                            var part = (MimePart)att;
                            part.Content.DecodeTo(ms);
                        }
                        var data = new Provider(new DeliverOnTime(), ms).PriceItemsList;
                        await AddItems(data);
                    }
                }

                await client.DisconnectAsync(true);
            }
            
            return Content("");
        }

        private async Task AddItems(List<PriceItems> items)
        {
            if (ModelState.IsValid)
            {
                await _context.PriceItems.AddRangeAsync(items.ToArray());
                await _context.SaveChangesAsync();
            }
        }
    }
}
