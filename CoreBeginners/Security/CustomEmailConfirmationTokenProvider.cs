using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace CoreBeginners.Security
{
    public class CustomEmailConfirmationTokenProvider<TUser>:DataProtectorTokenProvider<TUser> where TUser:class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                        IOptions<CustomEmailConfirmaionTokenProvideOptions> options,
                                        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                        :base( dataProtectionProvider, options, logger)
                                       
        {

        }
    }
}
