using System;
using TodoApi.Models;

namespace TodoApi.Models
{
    public class TodoItem
    { 

        public long Id { get; set; }
        public String Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
