using Microsoft.AspNetCore.Mvc;
using AspNetCoreDemo.RestfulWebApiDemo.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreDemo.RestfulWebApiDemo.Controllers
{
    public class CustomerController
    {
        public ActionResult<CustomerPage> Index(int? id)
        {
            return new CustomerPage();
        }
    }
}
