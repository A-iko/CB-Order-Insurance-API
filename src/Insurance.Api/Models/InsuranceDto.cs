﻿using System.Text.Json.Serialization;

namespace Insurance.Api.Models
{   
    public class InsuranceDto
    {        
        public int ProductId { get; set; }
        public decimal InsuranceValue { get; set; }
    }
}
