﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult WebStoreBlog() => View();
    }
}
