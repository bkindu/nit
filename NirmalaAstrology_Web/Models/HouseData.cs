using System;
using System.Collections.Generic;

namespace NirmalaAstrology_Web.Services
{
public class HouseData
    {
        public int HouseNumber { get; set; }
        public int Rashi { get; set; }
        public string RashiName { get; set; }
        public List<string> Planets { get; set; }
        public HouseData() { Planets = new List<string>(); }
    }
}
