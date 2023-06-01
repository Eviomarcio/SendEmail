using SendEmail.Entities;

var provider = "";
var userName = "";
var password = "";

var outlook = new Email(provider, userName, password);

outlook.SendEmail(
    emailsTo: new List<string>
    {
        "Um ou uma lista de emails para enviar"
    },
    subject: "Assunto do Email",
    body: "Corpo do email.",
    attachments: new List<string>
    {
        @"Path dos anexos."
    });