﻿using System;

namespace Xels.Bitcoin.Features.RPC.Exceptions
{
    public class RPCServerException : Exception
    {
        public RPCServerException(RPCErrorCode errorCode, string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public RPCErrorCode ErrorCode { get; set; }
    }
}
