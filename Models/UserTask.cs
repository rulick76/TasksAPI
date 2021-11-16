using System;

namespace TasksAPI.Models
{
    public class UserTask
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool InProgress { get; set; }
        public string Strted { get; set; }

        public string Ended { get; set; }
    }
}