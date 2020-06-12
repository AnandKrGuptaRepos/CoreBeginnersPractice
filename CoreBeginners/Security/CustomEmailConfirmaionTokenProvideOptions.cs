using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CoreBeginners.Security
{
    public class CustomEmailConfirmaionTokenProvideOptions :DataProtectionTokenProviderOptions
    {
    }
}
