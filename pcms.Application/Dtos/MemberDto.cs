﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application.Dto
{
    public class MemberDto
    {
        public string MemberId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool IsLegibleforBenefit { get; set; } = false;
        public string RSAPin { get; set; }
        public DateTime? RegistrationDate { get; set; }

    }
}
