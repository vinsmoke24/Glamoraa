﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class Userdto
	{
        public string Username { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 
        public bool IsActive { get; set; }
    }
}