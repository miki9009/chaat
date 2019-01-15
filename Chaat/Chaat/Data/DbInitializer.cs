using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Chaat.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext appContext)
        {
            appContext.Database.EnsureCreated();

        }
    }
}
