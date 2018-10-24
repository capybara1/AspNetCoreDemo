using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreDemo.RestfulWebApiDemo.Resources
{
    public class CustomerPage : Page
    {
        public IList<Customer> MyProperty { get; set; }
    }
}
