using System.Collections.Generic;

namespace VechimeSoftware
{
    public class Person
    {
        public int ID { get; set; }
        public string CNP { get; set; }
        public string Serie { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public List<Perioada> Perioade { get; set; }
    }
}
