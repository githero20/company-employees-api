﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public sealed class RefreshTokenBadRequest : BadRequestException
    {
        public RefreshTokenBadRequest()
            : base("Invalid client request. Refresh token is invalid.")
        {
        }
    }
}