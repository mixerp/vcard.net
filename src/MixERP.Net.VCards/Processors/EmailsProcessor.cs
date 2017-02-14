﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using MixERP.Net.VCards.Extensions;
using MixERP.Net.VCards.Lookups;
using MixERP.Net.VCards.Models;
using MixERP.Net.VCards.Types;

namespace MixERP.Net.VCards.Processors
{
    internal static class EmailsProcessor
    {
        internal static string ToVCardToken(this IEnumerable<Email> value, VCardVersion version)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            foreach (var email in value)
            {
                string type = email.Type.ToVCardString();

                string key = "EMAIL";

                if (version == VCardVersion.V4)
                {
                    if (email.Preference > 0)
                    {
                        key = key + ";PREF=" + email.Preference;
                    }
                }

                builder.Append(GroupProcessor.Serialize(key, version, type, true, email.EmailAddress));
            }

            return builder.ToString();
        }

        internal static void Parse(Token token, ref VCard vcard)
        {
            var email = new Email();
            var preference = token.AdditionalKeyMembers.FirstOrDefault(x => x.Key == "PREF");
            var type = token.AdditionalKeyMembers.FirstOrDefault(x => x.Key == "TYPE");

            email.Preference = preference.Value.ConvertTo<int>();
            email.Type = EmailTypeLookup.Parse(type.Value);
            email.EmailAddress = token.Values[0];

            var emails = (List<Email>) vcard.Emails ?? new List<Email>();
            emails.Add(email);
            vcard.Emails = emails;
        }
    }
}