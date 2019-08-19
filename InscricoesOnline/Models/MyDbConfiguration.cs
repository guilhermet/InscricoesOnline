using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Models
{
    public class MyDbConfiguration : DbConfiguration
    {
        public MyDbConfiguration()
        {
            this.AddInterceptor(new NVarcharInterceptor());
        }
    }
}