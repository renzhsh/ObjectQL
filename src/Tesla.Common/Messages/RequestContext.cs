using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinhe.Messages
{
    public class RequestApiContext
    {
        public RequestApiContext(RequestData request)
        {
            this.CurrentRequest = request;
        }

        public RequestData CurrentRequest
        {
            private set; get;
        }

        private ResponseData _currentResponse;
        public ResponseData Reponse
        {
            get
            {
                if (_currentResponse == null)
                {
                    _currentResponse = new ResponseData();
                }
                return _currentResponse;
            }
        }
    } 
}
