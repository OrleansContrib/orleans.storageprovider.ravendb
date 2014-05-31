﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orleans;

namespace Orleans.StorageProvider.RavenDB.TestInterfaces
{
    [Serializable]
    public class PersonalAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderType Gender { get; set; }
    }
}
